using System.Windows.Media;

namespace ForceFeedback.Rules.Configuration
{
    internal class MethodTooLongLimit
    {
        public MethodTooLongLimit(int lines, Color? color, double transparency)
        {
            Lines = lines;
            Transparency = transparency;

            if (color.HasValue)
            {
                var colorValue = color.Value;
                colorValue.A = (byte)(255d * transparency);
                Color = colorValue;
            }
        }

        public int Lines
        {
            get;
        }

        public Color? Color
        {
            get;
        }

        public double Transparency
        {
            get;
        }
    }
}
