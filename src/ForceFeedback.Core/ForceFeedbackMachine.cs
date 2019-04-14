using System.Collections.Generic;
using ForceFeedback.Core.adapters.configuration;
using ForceFeedback.Core.domain;
using ForceFeedback.Core.Feedback;

namespace ForceFeedback.Core
{
    public class ForceFeedbackMachine
    {
        private readonly Configuration _config;
        private readonly FeedbackGenerator _feedbackGen;
        

        public ForceFeedbackMachine(string solutionFilePath, string projectFilePath, string sourceFilePath)
        : this(new ConfigurationProvider(solutionFilePath, projectFilePath, sourceFilePath).Configuration){}

        internal ForceFeedbackMachine(Configuration config) {
            _config = config;
            _feedbackGen = new FeedbackGenerator();
        }
        
        
        public IEnumerable<IFeedback> ProduceVisualFeedback(string methodName, int methodLineCount)
            => _config.TryFindRule(methodLineCount, out var rule) 
                ? _feedbackGen.Visual_feedback(rule) 
                : _feedbackGen.No_feedback;

        
        public IEnumerable<IFeedback> ProduceTotalFeedback(string methodName, int methodLineCount)
            => _config.TryFindRule(methodLineCount, out var rule) 
                ? _feedbackGen.Total_feedback(methodName, rule)
                : _feedbackGen.No_feedback ;
    }
}
