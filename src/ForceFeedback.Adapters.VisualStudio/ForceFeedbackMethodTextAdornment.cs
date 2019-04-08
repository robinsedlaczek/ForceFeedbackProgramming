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
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using System.Windows;
using ForceFeedback.Core;
using System.Threading;
using ForceFeedback.Core.Feedbacks;

namespace ForceFeedback.Adapters.VisualStudio
{
    /// <summary>
    /// MethodTooLongTextAdornment places red boxes behind all the "a"s in the editor window
    /// </summary>
    internal sealed class ForceFeedbackMethodTextAdornment
    {
        #region Private Fields

        private readonly ITextDocumentFactoryService _textDocumentFactory;
        private readonly IAdornmentLayer _layer;
        private readonly IWpfTextView _view;
        private ITextDocument _textDocument;
        private IList<CodeBlockOccurrence> _codeBlockOccurrences;
        private readonly ForceFeedbackContext _forceFeedbackContext;
        private readonly ForceFeedbackMachine _feedbackMachine;

        private readonly string[] AllowedCharactersInChanges = new[]
        {
            "\r", "\n", "\r\n",
            " ", "\"", "'", ".", ",", "@", "$", "(", ")", "{", "}", "[", "]", "&", "|", "\\", "%", "+", "-", "*", "/", ";", ":", "_", "?", "!",
            "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z",
            "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z",
            "0", "1", "2", "3", "4", "5", "6", "7", "8", "9"
        };

        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the <see cref="ForceFeedbackMethodTextAdornment"/> class.
        /// </summary>
        /// <param name="view">Text view to create the adornment for</param>
		public ForceFeedbackMethodTextAdornment(IWpfTextView view, ITextDocumentFactoryService textDocumentFactory)
        {
			_view = view ?? throw new ArgumentNullException(nameof(view));
			_textDocumentFactory = textDocumentFactory ?? throw new ArgumentNullException(nameof(textDocumentFactory));

			var res = _textDocumentFactory.TryGetTextDocument(_view.TextBuffer, out _textDocument);

            _codeBlockOccurrences = new List<CodeBlockOccurrence>();

            _layer = view.GetAdornmentLayer("MethodTooLongTextAdornment");
			
            _view.LayoutChanged += OnLayoutChanged;
            _view.TextBuffer.Changed += OnTextBufferChanged;
            _view.TextBuffer.Changing += OnTextBufferChanging;

			_textDocumentFactory = textDocumentFactory;

            var project = _textDocument?.TextBuffer?.CurrentSnapshot?.GetOpenDocumentInCurrentContextWithChanges()?.Project;

            _forceFeedbackContext = new ForceFeedbackContext();
            _forceFeedbackContext.Project = project?.Name;
            _forceFeedbackContext.Assembly = project?.AssemblyName;
            _forceFeedbackContext.FilePath = _textDocument.FilePath;

            _feedbackMachine = new ForceFeedbackMachine(_forceFeedbackContext);
		}

        private void OnTextBufferChanging(object sender, TextContentChangingEventArgs e)
        {
            var longMethodOccurence = _codeBlockOccurrences
                .Where(occurence => occurence.Block.FullSpan.IntersectsWith(_view.Caret.Position.BufferPosition.Position))
                .Select(occurence => occurence)
                .FirstOrDefault();

            UpdateContextByCodeBlock(longMethodOccurence.Block);

            _forceFeedbackContext.CaretPosition = _view.Caret.Position.BufferPosition.Position;

            var feedbacks = _feedbackMachine.RequestFeedbackBeforeMethodCodeChange();

            foreach (var feedback in feedbacks)
            {
                if (feedback is InsertTextFeedback)
                    InsertText(feedback);
                else if (feedback is DelayKeyboardInputsFeedback)
                    DelayKeyboardInput(feedback);
                else if (feedback is PreventKeyboardInputsFeedback)
                    e.Cancel();
            }
        }

        #endregion

        #region Event Handler

        private void OnTextBufferChanged(object sender, TextContentChangedEventArgs e)
        {
            if (!InteresstingChangeOccured(e))
                return;

            var change = e.Changes[0];

            var longMethodOccurence = _codeBlockOccurrences
                .Where(occurence => occurence.Block.FullSpan.IntersectsWith(change.NewSpan.Start))
                .Select(occurence => occurence)
                .FirstOrDefault();

            UpdateContextByCodeBlock(longMethodOccurence.Block);
            UpdateContextByTextChange(change);

            var feedbacks = _feedbackMachine.RequestFeedbackAfterMethodCodeChange();

            foreach (var feedback in feedbacks)
            {
                if (feedback is InsertTextFeedback)
                    InsertText(feedback);
                else if (feedback is DelayKeyboardInputsFeedback)
                    DelayKeyboardInput(feedback);
            }
        }

        private static void DelayKeyboardInput(IFeedback feedback)
        {
            var delayKeyboardInputsFeedback = feedback as DelayKeyboardInputsFeedback;

            Thread.Sleep(delayKeyboardInputsFeedback.Milliseconds);
        }

        private void UpdateContextByTextChange(ITextChange change)
        {
            _forceFeedbackContext.InsertedText = change.NewText;
            _forceFeedbackContext.InsertedAt = change.OldPosition;
            _forceFeedbackContext.ReplacedText = change.OldText;
            _forceFeedbackContext.CaretPosition = change.NewPosition;
        }

        private void InsertText(IFeedback feedback)
        {
            var insertTextFeedback = feedback as InsertTextFeedback;

            if (!_view.TextBuffer.CheckEditAccess())
                throw new Exception("Cannot edit text buffer.");

            var edit = _view.TextBuffer.CreateEdit(EditOptions.None, null, "ForceFeedback");
            var inserted = edit.Insert(insertTextFeedback.Position, insertTextFeedback.Text);

            if (!inserted)
                throw new Exception($"Cannot insert '{insertTextFeedback.Text}' at position {insertTextFeedback.Position} in text buffer.");

            edit.Apply();
        }

        private bool InteresstingChangeOccured(TextContentChangedEventArgs e)
        {
            if (WasChangeCausedByForceFeedback(e.EditTag) || e.Changes.Count == 0)
                return false;

            var change = e.Changes[0];

            // [RS] We trim the new text when checking for allowed characters, if the text has more than one character. This is, e.g. 
            //      if the user inserted a linefeed and the IDE created whitespaces automatically for indention of the next line.
            //      In this case, we want to ignore the generated leading whitespaces. 
            //      In the case the user entered a whitespace directly, we do not want to trim it away. So we check the new text length. 
            return AllowedCharactersInChanges.Contains(change.NewText.Length == 1 ? change.NewText : change.NewText.Trim(' '));
        }

        private static bool WasChangeCausedByForceFeedback(object editTag)
        {
            return editTag != null && editTag.ToString() == "ForceFeedback";
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
                var codeBlocks = await CollectBlockSyntaxNodesAsync(e.NewSnapshot);

                AnalyzeCodeBlockOccurrences(codeBlocks);
                CreateBackgroundVisualsForCodeBlocks();
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
        /// and the corresponding limit configuration is put together in an instance of  <see cref="CodeBlockOccurrence">LongCodeBlockOccurrence</see>.
        /// </summary>
        /// <param name="codeBlocks">The list of block syntaxes that will be analyzed.</param>
        private void AnalyzeCodeBlockOccurrences(IEnumerable<BlockSyntax> codeBlocks)
        {
            if (codeBlocks == null)
                throw new ArgumentNullException(nameof(codeBlocks));

            _codeBlockOccurrences.Clear();

            foreach (var codeBlock in codeBlocks)
            {
                UpdateContextByCodeBlock(codeBlock);

                var feedbacks = _feedbackMachine.RequestFeedbackForMethodCodeBlock();
                var occurence = new CodeBlockOccurrence(codeBlock, feedbacks);

                _codeBlockOccurrences.Add(occurence);
            }
        }

        private void UpdateContextByCodeBlock(BlockSyntax codeBlock)
        {
            var linesOfCode = codeBlock
                .WithoutLeadingTrivia()
                .WithoutTrailingTrivia()
                .GetText()
                .Lines
                .Count;

            var methodName = (codeBlock.Parent as MethodDeclarationSyntax)?.Identifier.ValueText;

            _forceFeedbackContext.MethodName = methodName;
            _forceFeedbackContext.LineCount = linesOfCode;
        }

        /// <summary>
        /// This method collects syntax nodes of code blocks that have too many lines of code.
        /// </summary>
        /// <param name="newSnapshot">The text snapshot containing the code to analyze.</param>
        /// <returns>Returns a list with the code block syntax nodes.</returns>
        private async Task<IEnumerable<BlockSyntax>> CollectBlockSyntaxNodesAsync(ITextSnapshot newSnapshot)
        {
            if (newSnapshot == null)
                throw new ArgumentNullException(nameof(newSnapshot));

            var currentDocument = newSnapshot.GetOpenDocumentInCurrentContextWithChanges();

            var syntaxRoot = await currentDocument.GetSyntaxRootAsync();

            var codeBlocks = syntaxRoot
                .DescendantNodes(node => true)
                .Where(node => node.IsSyntaxBlock() 
                    && (   node.Parent.IsMethod()
                        || node.Parent.IsConstructor()
                        || node.Parent.IsSetter()
                        || node.Parent.IsGetter()))
                .Select(block => block as BlockSyntax);

            return codeBlocks;
        }

        /// <summary>
        /// Adds a background behind the code block that have too many lines.
        /// </summary>
        private void CreateBackgroundVisualsForCodeBlocks()
        {
            if (_codeBlockOccurrences == null)
                return;

            foreach (var occurrence in _codeBlockOccurrences)
            {
                if (occurrence.Feedbacks == null || !occurrence.Feedbacks.Any(feedback => feedback is DrawColoredBackgroundFeedback))
                    continue;

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
        /// <param name="codeBlockOccurence">The occurence of the code block for which the visual will be created.</param>
        /// <returns>Returns the image that is the visual adornment (code block background).</returns>
        private Image CreateAndPositionCodeBlockBackgroundVisual(Rect adornmentBounds, CodeBlockOccurrence codeBlockOccurence)
        {
            if (adornmentBounds == null)
                throw new ArgumentNullException(nameof(adornmentBounds));

            if (codeBlockOccurence == null)
                throw new ArgumentNullException(nameof(codeBlockOccurence));

            var backgroundGeometry = new RectangleGeometry(adornmentBounds);
            var feedback = codeBlockOccurence.Feedbacks.Where(f => f is DrawColoredBackgroundFeedback).FirstOrDefault() as DrawColoredBackgroundFeedback;
            var backgroundColor = Color.FromArgb(feedback.BackgroundColor.A, feedback.BackgroundColor.R, feedback.BackgroundColor.G, feedback.BackgroundColor.B);

            var backgroundBrush = new SolidColorBrush(backgroundColor);
            backgroundBrush.Freeze();

            var outlineColor = Color.FromArgb(feedback.OutlineColor.A, feedback.OutlineColor.R, feedback.OutlineColor.G, feedback.OutlineColor.B);

            var outlinePenBrush = new SolidColorBrush(outlineColor);
            outlinePenBrush.Freeze();

            var outlinePen = new Pen(outlinePenBrush, 0.5);
            outlinePen.Freeze();

            var drawing = new GeometryDrawing(backgroundBrush, outlinePen, backgroundGeometry);
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

            var geometry = _view.TextViewLines.GetMarkerGeometry(snapshotSpan, false, new Thickness(0));

            if (geometry == null)
                return Rect.Empty;

            var top = geometry.Bounds.Top;
            var height = geometry.Bounds.Bottom - geometry.Bounds.Top;

            return new Rect(_view.ViewportLeft, top, _view.ViewportWidth, height);
        }

		#endregion
	}
}
