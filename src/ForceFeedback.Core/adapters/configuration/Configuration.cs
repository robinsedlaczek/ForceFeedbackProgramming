using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace ForceFeedback.Core
{
    class Configuration
    {
        public class Rule {
            public Rule(int lines, 
                Color backgroundColor, double backgroundTransparency, 
                int noiseDistance, int noiseLevel, int delay) {
                this.Lines = lines;
                this.BackgroundColor = backgroundColor;
                this.BackgroundTransparency = backgroundTransparency;
                this.NoiseDistance = noiseDistance;
                this.NoiseLevel = noiseLevel;
                this.Delay = delay;
            }
            
            public int Lines { get; }
            
            public Color BackgroundColor { get; }
            public double BackgroundTransparency { get; }

            public int NoiseDistance { get; }
            public int NoiseLevel { get; }
            public int Delay { get; }
        }

        public Configuration(IEnumerable<Rule> rules) {
            this.Rules = rules.OrderBy(r => r.Lines).ToArray();
        }
        
        public Rule[] Rules { get; }
        
        
        /*
         * The matching rule is the one with the highest value for the lines property
         * which is still <= methodLineCount.
         */
        public bool TryFindRule(int methodLineCount, out Rule rule) {
            rule = null;

            /* Running the check from the last to the first rule guarantees the highest
             * rule lines value is found.
             */
            for(var i=Rules.Length-1; i>=0; i--)
                if (Rules[i].Lines <= methodLineCount) {
                    rule = Rules[i];
                    break;
                }
            
            return rule != null;
        }
    }
}