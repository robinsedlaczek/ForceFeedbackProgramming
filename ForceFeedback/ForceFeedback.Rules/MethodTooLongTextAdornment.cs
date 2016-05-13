//------------------------------------------------------------------------------
// <copyright file="MethodTooLongTextAdornment.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using System.Windows;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio;

namespace ForceFeedback.Rules
{
    /// <summary>
    /// MethodTooLongTextAdornment places red boxes behind all the "a"s in the editor window
    /// </summary>
    internal sealed class MethodTooLongTextAdornment
    {
        #region Private Fields

        private readonly IAdornmentLayer _layer;
        private readonly IWpfTextView _view;
        private readonly IVsEditorAdaptersFactoryService _adapterService;

        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the <see cref="MethodTooLongTextAdornment"/> class.
        /// </summary>
        /// <param name="view">Text view to create the adornment for</param>
        public MethodTooLongTextAdornment(IWpfTextView view, IVsEditorAdaptersFactoryService adapterService)
        {
            if (view == null)
                throw new ArgumentNullException(nameof(view));

            if (adapterService == null)
                throw new ArgumentNullException(nameof(adapterService));

            _layer = view.GetAdornmentLayer("MethodTooLongTextAdornment");

            _view = view;
            _view.LayoutChanged += OnLayoutChanged;
            _adapterService = adapterService;
        }

        #endregion

        #region Event Handler

        /// <summary>
        /// Handles whenever the text displayed in the view changes by adding the adornment to any reformatted lines
        /// </summary>
        /// <remarks><para>This event is raised whenever the rendered text displayed in the <see cref="ITextView"/> changes.</para>
        /// <para>It is raised whenever the view does a layout (which happens when DisplayTextLineContainingBufferPosition is called or in response to text or classification changes).</para>
        /// <para>It is also raised whenever the view scrolls horizontally or when its size changes.</para>
        /// </remarks>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        internal async void OnLayoutChanged(object sender, TextViewLayoutChangedEventArgs e)
        {
            try
            {
                var methodBlockSyntaxNodes = await CollectMethodBlockSyntaxNodes(e.NewSnapshot);

                CreateVisualsForMethodsWithTooManyLines(methodBlockSyntaxNodes);
            }
            catch
            {
                // [RS] Maybe we should handle this exception a bit more faithfully. For now, we ignore the exceptions here 
                //      and wait for the next LayoutChanged event
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// This method collects syntax nodes of method bodies that have too many lines of code.
        /// </summary>
        /// <param name="newSnapshot">The text snapshot containing the code to analyze.</param>
        /// <returns>Returns a list with the method body nodes.</returns>
        private async Task<IEnumerable<BlockSyntax>> CollectMethodBlockSyntaxNodes(ITextSnapshot newSnapshot)
        {
            if (newSnapshot == null)
                throw new ArgumentNullException(nameof(newSnapshot));

            var currentDocument = newSnapshot.GetOpenDocumentInCurrentContextWithChanges();

            var syntaxRoot = await currentDocument.GetSyntaxRootAsync();

            var tooLongMethodDeclarations = syntaxRoot
                .DescendantNodes(node => true, false)
                .Where(node => node.Kind() == SyntaxKind.MethodDeclaration && (node as MethodDeclarationSyntax).Body.WithoutLeadingTrivia().WithoutTrailingTrivia().GetText().Lines.Count > Config.LongMethodLineCountThreshold)
                .Select(methodNode => (methodNode as MethodDeclarationSyntax).Body);

            return tooLongMethodDeclarations;
        }

        /// <summary>
        /// Adds a background behind the method bodies that have too many lines.
        /// </summary>
        /// <param name="methodBlockSyntaxNodes">A list of syntax nodes of method bodies that are too long.</param>
        private void CreateVisualsForMethodsWithTooManyLines(IEnumerable<BlockSyntax> methodBlockSyntaxNodes)
        {
            if (methodBlockSyntaxNodes == null)
                throw new ArgumentNullException(nameof(methodBlockSyntaxNodes));

            foreach (var methodBlockNode in methodBlockSyntaxNodes)
            {
                var snapshotSpan = new SnapshotSpan(_view.TextSnapshot, Span.FromBounds(methodBlockNode.Span.Start, methodBlockNode.Span.Start + methodBlockNode.Span.Length));
                var adornmentBounds = CalculateBounds(methodBlockNode, snapshotSpan);
                var image = CreateAndPositionMethodBackgroundVisual(adornmentBounds);

                _layer.RemoveMatchingAdornments(adornment => adornment.VisualSpan?.Span == snapshotSpan);
                _layer.AddAdornment(AdornmentPositioningBehavior.TextRelative, snapshotSpan, null, image, null);
            }
        }

        /// <summary>
        /// This method creates the visual for a method background and moves it to the correct position.
        /// </summary>
        /// <param name="adornmentBounds">The bounds of the rectangular adornment.</param>
        /// <returns>Returns the image that is the visual adornment (method background).</returns>
        private Image CreateAndPositionMethodBackgroundVisual(Rect adornmentBounds)
        {
            if (adornmentBounds == null)
                throw new ArgumentNullException(nameof(adornmentBounds));

            var backgroundGeometry = new RectangleGeometry(adornmentBounds);

            var drawing = new GeometryDrawing(Config.LongMethodBackgroundBrush, Config.LongMethodBorderPen, backgroundGeometry);
            drawing.Freeze();

            var drawingImage = new DrawingImage(drawing);
            drawingImage.Freeze();

            var image = new Image
            {
                Source = drawingImage
            };

            Canvas.SetLeft(image, adornmentBounds.Left);
            Canvas.SetTop(image, adornmentBounds.Top);

            return image;
        }

        /// <summary>
        /// This method calculates the bounds of the method background adornment.
        /// </summary>
        /// <param name="methodBlockNode">The syntax node that represents the block of a method that has too many lines of code.</param>
        /// <param name="snapshotSpan">The span of text that is associated with the background adornment.</param>
        /// <returns>Returns the calculated bounds of the method adornment.</returns>
        private Rect CalculateBounds(BlockSyntax methodBlockNode, SnapshotSpan snapshotSpan)
        {
            if (methodBlockNode == null)
                throw new ArgumentNullException(nameof(methodBlockNode));

            if (snapshotSpan == null)
                throw new ArgumentNullException(nameof(snapshotSpan));

            var nodes = new List<SyntaxNode>(methodBlockNode.ChildNodes());
            nodes.Add(methodBlockNode);

            var nodesFirstCharacterPositions = nodes.Select(node => node.Span.Start);
            var coordinatesOfCharacterPositions = new List<double>();

            foreach (var position in nodesFirstCharacterPositions)
            {
                var point = CalculateScreenCoordinatesForCharacterPosition(position);
                coordinatesOfCharacterPositions.Add(point.x);
            }

            // [RS] In the case we cannot find the screen coordinates for a character position, we simply skip and return empty bounds.
            if (coordinatesOfCharacterPositions == null || coordinatesOfCharacterPositions.Count == 0)
                return Rect.Empty;

            var viewOffset = VisualTreeHelper.GetOffset(_view.VisualElement);

            var left = coordinatesOfCharacterPositions
                .Select(coordinate => coordinate)
                .Min() - viewOffset.X;
            
            var geometry = _view.TextViewLines.GetMarkerGeometry(snapshotSpan, true, new Thickness(0));
            
            if (geometry == null)
                return Rect.Empty;

            var top = geometry.Bounds.Top;
            var width = geometry.Bounds.Right - geometry.Bounds.Left; // - viewOffset.X;
            var height = geometry.Bounds.Bottom - geometry.Bounds.Top;
            
            return new Rect(left, top, width, height);
        }

        /// <summary>
        /// This method tries to calculate the screen coordinates of a specific character position in the stream.
        /// </summary>
        /// <param name="position">The position of the character in the stream.</param>
        /// <returns>Returns a point representing the coordinates.</returns>
        private POINT CalculateScreenCoordinatesForCharacterPosition(int position)
        {
            try
            {
                var line = 0;
                var column = 0;
                var point = new POINT[1];
                var textView = _adapterService.GetViewAdapter(_view as ITextView);
                var result = textView.GetLineAndColumn(position, out line, out column);
                
                // [RS] If the line and column of a text position from the stream cannot be calculated, we simply return a zero-point.
                //      Maybe we should handle the error case slightly more professional by write some log entries or so.
                if (result != VSConstants.S_OK)
                    return new POINT() { x = 0, y = 0 };

                result = textView.GetPointOfLineColumn(line, column, point);

                return point[0];
            }
            catch
            {
                // [RS] In any case of error we simply return a zero-point.
                //      Maybe we should handle this exception slightly more professional by write some log entries or so.
                return new POINT() { x = 0, y = 0 };
            }
        }

        #endregion
    }
}
