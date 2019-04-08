using ForceFeedback.Core.Feedbacks;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace ForceFeedback.Core
{
    public class ForceFeedbackMachine
    {
        private string _solutionFilePath;
        private string _projectFilePath;
        private string _sourceFilePath;

        public ForceFeedbackMachine(string solutionFilePath, string projectFilePath, string sourceFilePath)
        {
            _solutionFilePath = solutionFilePath;
            _projectFilePath = projectFilePath;
            _sourceFilePath = sourceFilePath;
        }

        public List<IFeedback> RequestFeedbackForMethodCodeBlock(string methodName, int methodLineCount)
        {
            if (methodLineCount <= 3)
                return new List<IFeedback>();

            var backgroundColor = Color.Blue;
            var outlineColor = Color.White;

            if (methodLineCount > 15)
            {
                backgroundColor = Color.FromArgb(200, 0, 0, 255);
                outlineColor = Color.Red;
            }
            else if (methodLineCount > 10)
                backgroundColor = Color.FromArgb(150, 0, 0, 255);
            else if (methodLineCount > 5)
                backgroundColor = Color.FromArgb(100, 0, 0, 255);
            else if (methodLineCount > 3)
                backgroundColor = Color.FromArgb(50, 0, 0, 255);

            return new List<IFeedback>
            {
                new DrawColoredBackgroundFeedback(backgroundColor, outlineColor)
            };
        }

        public List<IFeedback> RequestFeedbackAfterMethodCodeChange(string methodName, int methodLineCount, int caretPosition)
        {
            var result = new List<IFeedback>();

            if (methodLineCount < 15)
                return result;

            const string noiseCharacters = "⌫♥♠♦◘○☺☻♀►♂↨◄↕";
            var random = new Random();
            var index = random.Next(0, noiseCharacters.Length);

            result.Add(new InsertTextFeedback($"{noiseCharacters[index]}"));

            // [RS] Add per line 100 ms delay. :)
            if (methodLineCount > 20)
                result.Add(new DelayKeyboardInputsFeedback((methodLineCount) - 20 * 100));

            return result;
        }

        public List<IFeedback> RequestFeedbackBeforeMethodCodeChange(string methodName, int methodLineCount)
        {
            var result = new List<IFeedback>();

            if (methodLineCount > 25)
                result.Add(new PreventKeyboardInputsFeedback());

            return result;
        }
    }
}
