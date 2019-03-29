using System.Windows.Media;

namespace ForceFeedback.Adapters.VisualStudio.Configuration
{
    internal class LongMethodLimitConfiguration
    {
        public LongMethodLimitConfiguration(int lines, Color color, double transparency, int noiseDistance)
        {
            Lines = lines;
            Transparency = transparency;
            NoiseDistance = noiseDistance;

            var colorValue = color;
            colorValue.A = (byte)(255d - transparency * 255d);

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

        public int NoiseDistance
        {
            get;
        }
    }
}
