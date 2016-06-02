using System.Data.SqlTypes;
using System.Windows.Media;

namespace ForceFeedback.Rules.Configuration
{
    internal class LongMethodLimitConfiguration
    {
        public LongMethodLimitConfiguration(int lines, Color color, double transparency, ReplacePattern replacePattern)
        {
            Lines = lines;
            Transparency = transparency;
            ReplacePattern = replacePattern;

            var colorValue = color;
            colorValue.A = (byte)(255d * transparency);

            Color = colorValue;
        }

        public int Lines
        {
            get;
        }

        public Color Color
        {
            get;
        }

        public double Transparency
        {
            get;
        }

        public ReplacePattern ReplacePattern
        {
            get;
        }
    }
}
