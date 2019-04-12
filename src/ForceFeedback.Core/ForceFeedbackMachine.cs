using ForceFeedback.Core.Feedbacks;
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

        internal ForceFeedbackMachine(Configuration config) { _config = config; }
        
        
        public IEnumerable<IFeedback> RequestFeedbackForMethodCodeBlock(string methodName, int methodLineCount)
            => _config.TryFindRule(methodLineCount, out var rule) 
                ? Visual_feedback(rule) 
                : No_feedback;

        
        public IEnumerable<IFeedback> RequestFeedbackAfterMethodCodeChange(string methodName, int methodLineCount)
            => _config.TryFindRule(methodLineCount, out var rule) 
                ? Visual_feedback(rule).Concat(Tactile_feedback(methodName, rule))
                : new IFeedback[0] ;

        public IEnumerable<IFeedback> RequestFeedbackBeforeMethodCodeChange(string methodName, int methodLineCount)
            => new List<IFeedback>();
        
        

        private IEnumerable<IFeedback> No_feedback => new IFeedback[0];
        
        private IEnumerable<IFeedback> Visual_feedback(Configuration.Rule rule) {
            yield return new DrawColoredBackgroundFeedback(rule.BackgroundColor, rule.BackgroundTransparency);
        }

        private IEnumerable<IFeedback> Tactile_feedback(string methodName, Configuration.Rule rule)
            => Tactile_delay_feedback(rule).Concat(Tactile_noise_feedback(methodName, rule));

        private IEnumerable<IFeedback> Tactile_delay_feedback(Configuration.Rule rule) {
            if (rule.Delay <= 0) return No_feedback;
            return new[] { new DelayKeyboardInputsFeedback(rule.Delay) };
        }
        
        private IEnumerable<IFeedback> Tactile_noise_feedback(string methodName, Configuration.Rule rule)
            => No_feedback;
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
}
