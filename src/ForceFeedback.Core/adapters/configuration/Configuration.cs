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
    }
}