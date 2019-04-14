using System.Collections.Generic;
using System.Linq;
using ForceFeedback.Core.adapters.configuration;
using ForceFeedback.Core.Feedback;
using ForceFeedback.Core.Feedback.Tactile;
using ForceFeedback.Core.Feedback.Visual;

namespace ForceFeedback.Core.domain
{
    class FeedbackGenerator
    {
        private readonly NoiseGenerator _noiseGenerator;
        
        public FeedbackGenerator() {
            _noiseGenerator = new NoiseGenerator();
        }
        
        
        public IEnumerable<IFeedback> No_feedback => new IFeedback[0];


        public IEnumerable<IFeedback> Total_feedback(string methodName, Configuration.FeedbackRule feedbackRule)
            => Visual_feedback(feedbackRule).Concat(Tactile_feedback(methodName, feedbackRule));
        
        public IEnumerable<IFeedback> Visual_feedback(Configuration.FeedbackRule feedbackRule) {
            yield return new Colorization(feedbackRule.BackgroundColor, feedbackRule.BackgroundColorTransparency);
        }

        
        private IEnumerable<IFeedback> Tactile_feedback(string methodName, Configuration.FeedbackRule feedbackRule)
            => Tactile_delay_feedback(feedbackRule).Concat(Tactile_noise_feedback(methodName, feedbackRule));

        
        private IEnumerable<IFeedback> Tactile_delay_feedback(Configuration.FeedbackRule feedbackRule) {
            if (feedbackRule.Delay <= 0) return No_feedback;
            return new[] { new Delay(feedbackRule.Delay) };
        }

        private IEnumerable<IFeedback> Tactile_noise_feedback(string methodName, Configuration.FeedbackRule feedbackRule)
            => _noiseGenerator.IsNoiseDue(methodName, feedbackRule.NoiseDistance) 
                ? new[] {new Noise(_noiseGenerator.Generate(feedbackRule.NoiseLevel))} 
                : No_feedback;
    }
}