using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace ForceFeedback.Core.adapters.configuration
{
    class Configuration
    {
        public class FeedbackRule {
            public FeedbackRule(int minimumNumberOfLinesInMethod, 
                Color backgroundColor, double backgroundColorTransparency, 
                int noiseDistance, int noiseLevel, int delay) {
                this.MinimumNumberOfLinesInMethod = minimumNumberOfLinesInMethod;
                this.BackgroundColor = backgroundColor;
                this.BackgroundColorTransparency = backgroundColorTransparency;
                this.NoiseDistance = noiseDistance;
                this.NoiseLevel = noiseLevel;
                this.Delay = delay;
            }
            
            public int MinimumNumberOfLinesInMethod { get; }
            
            public Color BackgroundColor { get; }
            public double BackgroundColorTransparency { get; }

            public int NoiseDistance { get; }
            public int NoiseLevel { get; }
            public int Delay { get; }
        }

        
        private readonly FeedbackRule[] _rules;
        
        
        public Configuration(IEnumerable<FeedbackRule> rules) {
            _rules = rules.OrderBy(r => r.MinimumNumberOfLinesInMethod).ToArray();
        }
        
        
        public bool TryFindRule(int methodLineCount, out FeedbackRule rule) {
            /*
             * The matching rule is the one with the highest value for the lines property
             * which is still <= methodLineCount.
             */
            rule = null;

            /* Running the check from the last to the first rule guarantees the highest
             * rule lines value is found.
             */
            for(var i=_rules.Length-1; i>=0; i--)
                if (_rules[i].MinimumNumberOfLinesInMethod <= methodLineCount) {
                    rule = _rules[i];
                    break;
                }
            
            return rule != null;
        }
    }
}