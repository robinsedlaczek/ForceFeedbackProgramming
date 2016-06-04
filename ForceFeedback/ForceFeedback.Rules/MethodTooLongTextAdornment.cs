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
using ForceFeedback.Rules.Configuration;
using System.Text;

namespace ForceFeedback.Rules
{
    /// <summary>
    /// MethodTooLongTextAdornment places red boxes behind all the "a"s in the editor window
    /// </summary>
    internal sealed class MethodTooLongTextAdornment
    {
        #region Private Fields

        private IList<LongMethodOccurrence> _longMethodOccurrences;
        private readonly IAdornmentLayer _layer;
        private readonly IWpfTextView _view;
        private readonly IVsEditorAdaptersFactoryService _adapterService;
        private int _lastCaretBufferPosition;
        private int _numberOfKeystrokes;

        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the <see cref="MethodTooLongTextAdornment"/> class.
        /// </summary>
        /// <param name="view">Text view to create the adornment for</param>
        /// 
        /// !!! Missing parameter documentation !!!
        ///
        public MethodTooLongTextAdornment(IWpfTextView view, IVsEditorAdaptersFactoryService adapterService)
        {
            if (view == null)
                throw new ArgumentNullException(nameof(view));

            if (adapterService == null)
                throw new ArgumentNullException(nameof(adapterService));

            _lastCaretBufferPosition = 0;
            _numberOfKeystrokes = 0;
            _longMethodOccurrences = new List<LongMethodOccurrence>();

            _layer = view.GetAdornmentLayer("MethodTooLongTextAdornment");

            _view = view;
            _view.LayoutChanged += OnLayoutChanged;
            _view.TextBuffer.Changed += OnTextBufferChanged;

            _adapterService = adapterService;
        }

        #endregion

        #region Event Handler

        private void OnTextBufferChanged(object sender, TextContentChangedEventArgs e)
        {
            // [RS] We do nothing here if the change was caused by ourselves. 
            if (e.EditTag != null && e.EditTag.ToString() == "ForceFeedback")
                return;

            var allowedCharacters = new[]
            {
                "\r", "\n", "\r\n",
                "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z",
                "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z",
                "0", "1", "2", "3", "4", "5", "6", "7", "8", "9"
            };

            var change = e.Changes[0];
            // [RS] We trim the new entered text because ...
            var interestingChangeOccurred = e.Changes.Count > 0 && allowedCharacters.Contains(change.NewText.Trim(' '));

            if (!interestingChangeOccurred)
                return;

            var caretPosition = _view.Caret.Position.BufferPosition.Position;

            if (change.NewText == "\r\n" || change.NewText == "\r" || change.NewText == "\n")
            {
                _lastCaretBufferPosition = caretPosition + change.NewLength - 1;
                _numberOfKeystrokes = 0;
                return;
            }

            var longMethodOccurence = _longMethodOccurrences
                .Where(occurence => occurence.MethodDeclaration.FullSpan.IntersectsWith(change.NewSpan.Start))
                .Select(occurence => occurence)
                .FirstOrDefault();

            if (longMethodOccurence == null)
                return;

            if (caretPosition == _lastCaretBufferPosition + 1)
                _numberOfKeystrokes++;
            else
                _numberOfKeystrokes = 1;

            _lastCaretBufferPosition = caretPosition + change.NewLength - 1;

            if (_numberOfKeystrokes >= longMethodOccurence.LimitConfiguration.NoiseDistance)
            {
                if (!_view.TextBuffer.CheckEditAccess())
                    throw new Exception("Cannot edit text buffer.");

                var textToInsert = "⌫";

                var edit = _view.TextBuffer.CreateEdit(EditOptions.None, null, "ForceFeedback");
                var inserted = edit.Insert(change.NewPosition + 1, textToInsert.ToString());

                if (!inserted)
                    throw new Exception($"Cannot insert '{change.NewText}' at position {change.NewPosition} in text buffer.");

                edit.Apply();
            }
        }

        /// <summary>
        /// Handles whenever the text displayed in the view changes by adding the adornment to any reformatted lines.
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
                var methodDeclarations = await CollectMethodDeclarationSyntaxNodes(e.NewSnapshot);

                AnalyzeAndCacheLongMethodOccurrences(methodDeclarations);
                CreateVisualsForLongMethods();
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
        /// This method checks the given method declarations are too long based on the configured limits. If so, the method 
        /// declaration and the corresponding limit configuration is put together in an instance of  <see cref="LongMethodOccurrence">LongMethodOccurrence</see>.
        /// </summary>
        /// <param name="methodDeclarations">The list of method declarations that will be analyzed.</param>
        private void AnalyzeAndCacheLongMethodOccurrences(IEnumerable<MethodDeclarationSyntax> methodDeclarations)
        {
            if (methodDeclarations == null)
                throw new ArgumentNullException(nameof(methodDeclarations));

            _longMethodOccurrences.Clear();

            foreach (var methodDeclaration in methodDeclarations)
            {
                var linesOfCode = methodDeclaration.Body.WithoutLeadingTrivia().WithoutTrailingTrivia().GetText().Lines.Count;
                var correspondingLimitConfiguration = null as LongMethodLimitConfiguration;

                foreach (var limitConfiguration in ConfigurationManager.Configuration.MethodTooLongLimits.OrderBy(limit => limit.Lines))
                {
                    if (linesOfCode < limitConfiguration.Lines)
                        break;
                    
                    correspondingLimitConfiguration = limitConfiguration;
                }

                if (correspondingLimitConfiguration != null)
                {
                    var occurence = new LongMethodOccurrence(methodDeclaration, correspondingLimitConfiguration);
                    _longMethodOccurrences.Add(occurence);
                }
            }
        }

        /// <summary>
        /// This method collects syntax nodes of method declarations that have too many lines of code.
        /// </summary>
        /// <param name="newSnapshot">The text snapshot containing the code to analyze.</param>
        /// <returns>Returns a list with the method declaration nodes.</returns>
        private async Task<IEnumerable<MethodDeclarationSyntax>> CollectMethodDeclarationSyntaxNodes(ITextSnapshot newSnapshot)
        {
            if (newSnapshot == null)
                throw new ArgumentNullException(nameof(newSnapshot));

            var currentDocument = newSnapshot.GetOpenDocumentInCurrentContextWithChanges();

            var syntaxRoot = await currentDocument.GetSyntaxRootAsync();

            var tooLongMethodDeclarations = syntaxRoot
                .DescendantNodes(node => true, false)
                .Where(node => node.Kind() == SyntaxKind.MethodDeclaration)
                .Select(methodDeclaration => methodDeclaration as MethodDeclarationSyntax);

            return tooLongMethodDeclarations;
        }

        /// <summary>
        /// Adds a background behind the methods that have too many lines.
        /// </summary>
        private void CreateVisualsForLongMethods()
        {
            if (_longMethodOccurrences == null)
                return;

            foreach (var occurrence in _longMethodOccurrences)
            {
                var methodDeclaration = occurrence.MethodDeclaration;
                var snapshotSpan = new SnapshotSpan(_view.TextSnapshot, Span.FromBounds(methodDeclaration.Span.Start, methodDeclaration.Span.Start + methodDeclaration.Span.Length));
                var adornmentBounds = CalculateBounds(methodDeclaration, snapshotSpan);

                if (adornmentBounds.IsEmpty)
                    continue;

                var image = CreateAndPositionMethodBackgroundVisual(adornmentBounds, occurrence);

                if (image == null)
                    continue;

                _layer.RemoveAdornmentsByVisualSpan(snapshotSpan);
                _layer.AddAdornment(AdornmentPositioningBehavior.TextRelative, snapshotSpan, methodDeclaration, image, null);
            }
        }

        /// <summary>
        /// This method creates the visual for a method background and moves it to the correct position.
        /// </summary>
        /// <param name="adornmentBounds">The bounds of the rectangular adornment.</param>
        /// <param name="longMethodOccurence">The occurence of the method declaration for which the visual will be created.</param>
        /// <returns>Returns the image that is the visual adornment (method background).</returns>
        private Image CreateAndPositionMethodBackgroundVisual(Rect adornmentBounds, LongMethodOccurrence longMethodOccurence)
        {
            if (adornmentBounds == null)
                throw new ArgumentNullException(nameof(adornmentBounds));

            if (longMethodOccurence == null)
                throw new ArgumentNullException(nameof(longMethodOccurence));

            var backgroundGeometry = new RectangleGeometry(adornmentBounds);

            var backgroundBrush = new SolidColorBrush(longMethodOccurence.LimitConfiguration.Color);
            backgroundBrush.Freeze();

            var drawing = new GeometryDrawing(backgroundBrush, ConfigurationManager.LongMethodBorderPen, backgroundGeometry);
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
        /// <param name="methodDeclarationSyntaxNode">The syntax node that represents the method declaration that has too many lines of code.</param>
        /// <param name="snapshotSpan">The span of text that is associated with the background adornment.</param>
        /// <returns>Returns the calculated bounds of the method adornment.</returns>
        private Rect CalculateBounds(MethodDeclarationSyntax methodDeclarationSyntaxNode, SnapshotSpan snapshotSpan)
        {
            if (methodDeclarationSyntaxNode == null)
                throw new ArgumentNullException(nameof(methodDeclarationSyntaxNode));

            if (snapshotSpan == null)
                throw new ArgumentNullException(nameof(snapshotSpan));

            var nodes = new List<SyntaxNode>(methodDeclarationSyntaxNode.ChildNodes());
            nodes.Add(methodDeclarationSyntaxNode);

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

        private void LoadConfiguration()
        {
            //SettingsManager settingsManager = new ShellSettingsManager(_serviceProvider);
            //WritableSettingsStore userSettingsStore = settingsManager.GetWritableSettingsStore(SettingsScope.UserSettings);

            // Find out whether Notepad is already an External Tool.
            //int toolCount = userSettingsStore.GetInt32(("External Tools", "ToolNumKeys");
            //bool hasNotepad = false;
            //CompareInfo Compare = CultureInfo.InvariantCulture.CompareInfo;
            //for (int i = 0; i < toolCount; i++)
            //{
            //    if (Compare.IndexOf(userSettingsStore.GetString("External Tools", "ToolCmd" + i), "Notepad", CompareOptions.IgnoreCase) >= 0)
            //    {
            //        hasNotepad = true;
            //        break;
            //    }
            //}
        }

        #endregion
    }
}
