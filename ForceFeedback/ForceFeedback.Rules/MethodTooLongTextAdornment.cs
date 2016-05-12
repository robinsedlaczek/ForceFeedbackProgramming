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
        private readonly Pen _redPen;
        private readonly Brush _blueBrush;
        private readonly Brush _lightGrayBrush;
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

            _blueBrush = new SolidColorBrush(Color.FromArgb(0x20, 0x00, 0x00, 0xff));
            _blueBrush.Freeze();

            _lightGrayBrush = new SolidColorBrush(Color.FromArgb(0x20, 0x96, 0x96, 0x96));
            _lightGrayBrush.Freeze();

            var penBrush = new SolidColorBrush(Colors.Red);
            penBrush.Freeze();

            _redPen = new Pen(penBrush, 0.5);
            _redPen.Freeze();
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
            var methodBlockSyntaxNodes = await CollectMethodBlockSyntaxNodes(e.NewSnapshot);

            CreateVisualsForMethodsWithTooManyLines(methodBlockSyntaxNodes);
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
            var currentDocument = newSnapshot.GetOpenDocumentInCurrentContextWithChanges();

            var syntaxRoot = await currentDocument.GetSyntaxRootAsync();

            var tooLongMethodDeclarations = syntaxRoot
                .DescendantNodes(node => true, false)
                .Where(node => node.Kind() == SyntaxKind.MethodDeclaration && node.GetText().Lines.Count > 10)
                .Select(methodNode => (methodNode as MethodDeclarationSyntax).Body);

            return tooLongMethodDeclarations;
        }

        /// <summary>
        /// Adds a background behind the method bodies that have too many lines.
        /// </summary>
        /// <param name="methodBlockSyntaxNodes">A list of syntax nodes of method bodies that are too long.</param>
        private void CreateVisualsForMethodsWithTooManyLines(IEnumerable<BlockSyntax> methodBlockSyntaxNodes)
        {
            foreach (var methodBlockNode in methodBlockSyntaxNodes)
            {
                var snapshotSpan = new SnapshotSpan(_view.TextSnapshot, Span.FromBounds(methodBlockNode.Span.Start, methodBlockNode.Span.Start + methodBlockNode.Span.Length));
                var adornmentBounds = CalculateBounds(methodBlockNode, snapshotSpan);

                // Create geometry and visuals...
                var backgroundGeometry = new RectangleGeometry(adornmentBounds);

                var drawing = new GeometryDrawing(_lightGrayBrush, _redPen, backgroundGeometry);
                drawing.Freeze();

                var drawingImage = new DrawingImage(drawing);
                drawingImage.Freeze();

                var image = new Image
                {
                    Source = drawingImage
                };

                Canvas.SetLeft(image, adornmentBounds.Left);
                Canvas.SetTop(image, adornmentBounds.Top);

                _layer.RemoveMatchingAdornments(adornment => adornment.VisualSpan?.Span == snapshotSpan);
                //_layer.AddAdornment(AdornmentPositioningBehavior.TextRelative, snapshotSpan, null, image, null);
                _layer.AddAdornment(AdornmentPositioningBehavior.TextRelative, null, null, image, null);
            }
        }

        private Rect CalculateBounds(BlockSyntax methodBlockNode, SnapshotSpan snapshotSpan)
        {
            var line = 0;
            var column = 0;
            var point = new POINT[1];

            var nodes = new List<SyntaxNode>(methodBlockNode.ChildNodes());
            nodes.Add(methodBlockNode);

            var nodesFirstCharacterPositions = nodes.Select(node => node.Span.Start);
            var coordinatesOfCharacterPositions = new List<double>();

            foreach (var position in nodesFirstCharacterPositions)
            {
                var textView = _adapterService.GetViewAdapter(_view as ITextView);
                var result = textView.GetLineAndColumn(position, out line, out column);

                try
                {
                    result = textView.GetPointOfLineColumn(line, column, point);
                    coordinatesOfCharacterPositions.Add(point[0].x);
                }
                catch
                {
                    // Do nothing for now.
                }
            }

            if (coordinatesOfCharacterPositions == null || coordinatesOfCharacterPositions.Count == 0)
                return Rect.Empty;

            var left = coordinatesOfCharacterPositions
                .Select(coordinate => coordinate)
                .Min() - _view.ViewportLeft;

            var geometry = _view.TextViewLines.GetMarkerGeometry(snapshotSpan, true, new Thickness(0));

            if (geometry == null)
                return Rect.Empty;

            var top = geometry.Bounds.Top;
            var width = geometry.Bounds.Right - geometry.Bounds.Left;
            var height = geometry.Bounds.Bottom - geometry.Bounds.Top;

            return new Rect(left, top, width, height);
        }

        #endregion
    }
}
