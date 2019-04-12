using ForceFeedback.Core.Feedbacks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using ForceFeedback.Core.adapters.configuration;
using ForceFeedback.Core.domain;

namespace ForceFeedback.Core
{
    public class ForceFeedbackMachine
    {
        private readonly Configuration _config;
        private readonly NoiseFeedback _noiseFeedback;

        public ForceFeedbackMachine(string solutionFilePath, string projectFilePath, string sourceFilePath)
        : this(new ConfigurationProvider(solutionFilePath, projectFilePath, sourceFilePath).Compile()){}

        internal ForceFeedbackMachine(Configuration config) {
            _config = config;
            _noiseFeedback = new NoiseFeedback();
        }
        
        
        public IEnumerable<IFeedback> RequestFeedbackForMethodCodeBlock(string methodName, int methodLineCount)
            => _config.TryFindRule(methodLineCount, out var rule) 
                ? Visual_feedback(rule) 
                : No_feedback;

        
        public IEnumerable<IFeedback> RequestFeedbackAfterMethodCodeChange(string methodName, int methodLineCount)
            => _config.TryFindRule(methodLineCount, out var rule) 
                ? Visual_feedback(rule).Concat(Tactile_feedback(methodName, rule))
                : No_feedback ;

        public IEnumerable<IFeedback> RequestFeedbackBeforeMethodCodeChange(string methodName, int methodLineCount)
            => No_feedback;
        
        

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
            => _noiseFeedback.IsDue(methodName, rule.NoiseDistance) 
                ? new[] {new InsertTextFeedback(_noiseFeedback.Generate(rule.NoiseLevel))} 
                : No_feedback;
    }
}
