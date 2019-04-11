﻿using ForceFeedback.Core.Feedbacks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace ForceFeedback.Core
{
    public class ForceFeedbackMachine
    {
        private readonly Configuration _config;

        public ForceFeedbackMachine(string solutionFilePath, string projectFilePath, string sourceFilePath) {
            _config = new ConfigurationProvider(solutionFilePath, projectFilePath, sourceFilePath).Compile();
        }

        internal ForceFeedbackMachine(Configuration config) {
            _config = config;
        }
        
        
        public IEnumerable<IFeedback> RequestFeedbackForMethodCodeBlock(string methodName, int methodLineCount) {
            if (methodLineCount < 1 || _config.Rules.Length < 1 || methodLineCount < _config.Rules[0].Lines) return new IFeedback[0];

            return new[] {
                new DrawColoredBackgroundFeedback(_config.Rules[0].BackgroundColor, _config.Rules[0].BackgroundTransparency) 
            };
        }

        public IEnumerable<IFeedback> RequestFeedbackAfterMethodCodeChange(string methodName, int methodLineCount)
        {
            if (methodLineCount < 1 || _config.Rules.Length < 1 || methodLineCount < _config.Rules[0].Lines) return new IFeedback[0];

            var feedbacks = new List<IFeedback>();
            feedbacks.Add(new DrawColoredBackgroundFeedback(_config.Rules[0].BackgroundColor, _config.Rules[0].BackgroundTransparency));
            // TODO: Noise distance has to be taken into account for noise chars!
            if (_config.Rules[0].Delay > 0)
                feedbacks.Add(new DelayKeyboardInputsFeedback(_config.Rules[0].Delay));
            return feedbacks;

            /* TODO: Provide tactile feedback
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
            */

        }

        public IEnumerable<IFeedback> RequestFeedbackBeforeMethodCodeChange(string methodName, int methodLineCount) {
            return new List<IFeedback>();
        }
    }
}
