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
using ForceFeedback.Rules.Configuration;

namespace ForceFeedback.Rules
{
    /// <summary>
    /// MethodTooLongTextAdornment places red boxes behind all the "a"s in the editor window
    /// </summary>
    internal sealed class MethodTooLongTextAdornment
    {
        #region Private Fields

        private IList<LongCodeBlockOccurrence> _longCodeBlockOccurrences;
        private readonly IAdornmentLayer _layer;
        private readonly IWpfTextView _view;
        private int _lastCaretBufferPosition;
        private int _numberOfKeystrokes;

        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the <see cref="MethodTooLongTextAdornment"/> class.
        /// </summary>
        /// <param name="view">Text view to create the adornment for</param>
        public MethodTooLongTextAdornment(IWpfTextView view)
        {
            if (view == null)
                throw new ArgumentNullException(nameof(view));

            _lastCaretBufferPosition = 0;
            _numberOfKeystrokes = 0;
            _longCodeBlockOccurrences = new List<LongCodeBlockOccurrence>();

            _layer = view.GetAdornmentLayer("MethodTooLongTextAdornment");

            _view = view;
            _view.LayoutChanged += OnLayoutChanged;
            _view.TextBuffer.Changed += OnTextBufferChanged;
        }

        #endregion

        #region Event Handler

        private void OnTextBufferChanged(object sender, TextContentChangedEventArgs e)
        {
            // [RS] We do nothing here if the change was caused by ourselves or if there is no change at all. 
            var changeCausedByForceFeedback = e.EditTag != null && e.EditTag.ToString() == "ForceFeedback";

            if (changeCausedByForceFeedback || e.Changes.Count == 0)
                return;

            var allowedCharacters = new[]
            {
                "\r", "\n", "\r\n",
                " ", "\"", "'", ".", ",", "@", "$", "(", ")", "{", "}", "[", "]", "&", "|", "\\", "%", "+", "-", "*", "/", ";", ":", "_", "?", "!",
                "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z",
                "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z",
                "0", "1", "2", "3", "4", "5", "6", "7", "8", "9"
            };

            var change = e.Changes[0];
            // [RS] We trim the new text when checking for allowed characters, if the text has more than one character. This is, e.g. 
            //      if the user inserted a linefeed and the IDE created whitespaces automatically for indention of the next line.
            //      In this case, we want to ignore the generated leading whitespaces. 
            //      In the case the user entered a whitespace directly, we do not want to trim it away. So we check the new text length. 
            var interestingChangeOccurred = 
                e.Changes.Count > 0 && allowedCharacters.Contains(change.NewText.Length == 1 ? change.NewText : change.NewText.Trim(' '));

            if (!interestingChangeOccurred)
                return;

            var caretPosition = _view.Caret.Position.BufferPosition.Position;

            if (change.NewText == "\r\n" || change.NewText == "\r" || change.NewText == "\n")
            {
                _lastCaretBufferPosition = caretPosition + change.NewLength - 1;
                _numberOfKeystrokes = 0;
                return;
            }

            var longMethodOccurence = _longCodeBlockOccurrences
                .Where(occurence => occurence.Block.FullSpan.IntersectsWith(change.NewSpan.Start))
                .Select(occurence => occurence)
                .FirstOrDefault();

            if (longMethodOccurence == null || longMethodOccurence.LimitConfiguration.NoiseDistance <= 0)
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
                var inserted = edit.Insert(change.NewPosition + change.NewLength, textToInsert.ToString());

                if (!inserted)
                    throw new Exception($"Cannot insert '{change.NewText}' at position {change.NewPosition} in text buffer.");

                edit.Apply();

                _numberOfKeystrokes = 0;
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
                var codeBlocks = await CollectBlockSyntaxNodes(e.NewSnapshot);

                AnalyzeAndCacheLongCodeBlockOccurrences(codeBlocks);
                CreateVisualsForLongCodeBlock();
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
        /// This method checks if the given block syntaxes are too long based on the configured limits. If so, the block syntax 
        /// and the corresponding limit configuration is put together in an instance of  <see cref="LongCodeBlockOccurrence">LongCodeBlockOccurrence</see>.
        /// </summary>
        /// <param name="codeBlocks">The list of block syntaxes that will be analyzed.</param>
        private void AnalyzeAndCacheLongCodeBlockOccurrences(IEnumerable<BlockSyntax> codeBlocks)
        {
            if (codeBlocks == null)
                throw new ArgumentNullException(nameof(codeBlocks));

            _longCodeBlockOccurrences.Clear();

            foreach (var codeBlock in codeBlocks)
            {
                // [RS] Do nothing if there is no method body (e.g. if the method declaration is an expression-bodied member).
                //if (methodDeclaration.Body == null)
                //    continue;

                var linesOfCode = codeBlock.WithoutLeadingTrivia().WithoutTrailingTrivia().GetText().Lines.Count;
                var correspondingLimitConfiguration = null as LongMethodLimitConfiguration;

                foreach (var limitConfiguration in ConfigurationManager.Configuration.MethodTooLongLimits.OrderBy(limit => limit.Lines))
                {
                    if (linesOfCode < limitConfiguration.Lines)
                        break;
                    
                    correspondingLimitConfiguration = limitConfiguration;
                }

                if (correspondingLimitConfiguration != null)
                {
                    var occurence = new LongCodeBlockOccurrence(codeBlock, correspondingLimitConfiguration);
                    _longCodeBlockOccurrences.Add(occurence);
                }
            }
        }

        /// <summary>
        /// This method collects syntax nodes of code blocks that have too many lines of code.
        /// </summary>
        /// <param name="newSnapshot">The text snapshot containing the code to analyze.</param>
        /// <returns>Returns a list with the code block syntax nodes.</returns>
        private async Task<IEnumerable<BlockSyntax>> CollectBlockSyntaxNodes(ITextSnapshot newSnapshot)
        {
            if (newSnapshot == null)
                throw new ArgumentNullException(nameof(newSnapshot));

            var currentDocument = newSnapshot.GetOpenDocumentInCurrentContextWithChanges();

            var syntaxRoot = await currentDocument.GetSyntaxRootAsync();

            var tooLongCodeBlocks = syntaxRoot
                .DescendantNodes(node => true, false)
                .Where(node => node.Kind() == SyntaxKind.Block && (node.Parent.Kind() == SyntaxKind.MethodDeclaration || node.Parent.Kind() == SyntaxKind.ConstructorDeclaration || node.Parent.Kind() == SyntaxKind.SetAccessorDeclaration || node.Parent.Kind() == SyntaxKind.GetAccessorDeclaration))
                .Select(block => block as BlockSyntax);

            return tooLongCodeBlocks;
        }

        /// <summary>
        /// Adds a background behind the code block that have too many lines.
        /// </summary>
        private void CreateVisualsForLongCodeBlock()
        {
            if (_longCodeBlockOccurrences == null)
                return;

            foreach (var occurrence in _longCodeBlockOccurrences)
            {
                var codeBlockParentSyntax = occurrence.Block.Parent;
                var snapshotSpan = new SnapshotSpan(_view.TextSnapshot, Span.FromBounds(codeBlockParentSyntax.Span.Start, codeBlockParentSyntax.Span.Start + codeBlockParentSyntax.Span.Length));
                var adornmentBounds = CalculateBounds(codeBlockParentSyntax, snapshotSpan);

                if (adornmentBounds.IsEmpty)
                    continue;

                var image = CreateAndPositionCodeBlockBackgroundVisual(adornmentBounds, occurrence);

                if (image == null)
                    continue;

                _layer.RemoveAdornmentsByVisualSpan(snapshotSpan);
                _layer.AddAdornment(AdornmentPositioningBehavior.TextRelative, snapshotSpan, codeBlockParentSyntax, image, null);
            }
        }

        /// <summary>
        /// This method creates the visual for a code block background and moves it to the correct position.
        /// </summary>
        /// <param name="adornmentBounds">The bounds of the rectangular adornment.</param>
        /// <param name="longCodeBlockOccurence">The occurence of the code block for which the visual will be created.</param>
        /// <returns>Returns the image that is the visual adornment (code block background).</returns>
        private Image CreateAndPositionCodeBlockBackgroundVisual(Rect adornmentBounds, LongCodeBlockOccurrence longCodeBlockOccurence)
        {
            if (adornmentBounds == null)
                throw new ArgumentNullException(nameof(adornmentBounds));

            if (longCodeBlockOccurence == null)
                throw new ArgumentNullException(nameof(longCodeBlockOccurence));

            var backgroundGeometry = new RectangleGeometry(adornmentBounds);

            var backgroundBrush = new SolidColorBrush(longCodeBlockOccurence.LimitConfiguration.Color);
            backgroundBrush.Freeze();

            var drawing = new GeometryDrawing(backgroundBrush, ConfigurationManager.LongCodeBlockBorderPen, backgroundGeometry);
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
        /// This method calculates the bounds of the syntax node background adornment.
        /// </summary>
        /// <param name="syntaxNode">The syntax node that represents the block that has too many lines of code.</param>
        /// <param name="snapshotSpan">The span of text that is associated with the background adornment.</param>
        /// <returns>Returns the calculated bounds of the syntax node adornment.</returns>
        private Rect CalculateBounds(SyntaxNode syntaxNode, SnapshotSpan snapshotSpan)
        {
            if (syntaxNode == null)
                throw new ArgumentNullException(nameof(syntaxNode));

            if (snapshotSpan == null)
                throw new ArgumentNullException(nameof(snapshotSpan));

            var left = CalculateLeftPosition(ref snapshotSpan);

            if (left < 0)
                return Rect.Empty;

            var geometry = _view.TextViewLines.GetMarkerGeometry(snapshotSpan, true, new Thickness(0));

            if (geometry == null)
                return Rect.Empty;

            var top = geometry.Bounds.Top;
            var width = 800; // geometry.Bounds.Right - geometry.Bounds.Left;
            var height = geometry.Bounds.Bottom - geometry.Bounds.Top;

            return new Rect(left, top, width, height);
        }

        /// <summary>
        /// This method calculates the left-most position for the method background colorization. 
        /// </summary>
        /// <param name="snapshotSpan">The snapshot span of the method for which the position is calculated.</param>
        /// <returns>Returns the left-coordinate of the method.</returns>
        private double CalculateLeftPosition(ref SnapshotSpan snapshotSpan)
        {
            // [RS] We try to take the first character of the method to get the left-most position. If this does not work (e.g. if the character
            //      is out of view after scrolling), we take the last character of the method and try to calculate the left-most position for it.
            //      So we have always the left-most position, wheter the top or the bottom of the method is out of vie or not. If both are out of
            //      view, we won't colorize anything.

            var left = -1d;
            var firstCharacterSnapshotSpan = new SnapshotSpan(snapshotSpan.Start, 1);
            var firstCharacterSnapshotSpanGeometry = _view.TextViewLines.GetMarkerGeometry(firstCharacterSnapshotSpan, false, new Thickness(0));

            if (firstCharacterSnapshotSpanGeometry != null)
            {
                left = firstCharacterSnapshotSpanGeometry.Bounds.Left;
            }
            else
            {
                var lastCharacterSnapshotSpan = new SnapshotSpan(snapshotSpan.Start + snapshotSpan.Length - 1, 1);
                var lastCharacterSnapshotSpanGeometry = _view.TextViewLines.GetMarkerGeometry(lastCharacterSnapshotSpan, false, new Thickness(0));

                if (lastCharacterSnapshotSpanGeometry == null)
                    return -1;

                left = lastCharacterSnapshotSpanGeometry.Bounds.Left;
            }

            return left;
        }

        #endregion

    }
}
