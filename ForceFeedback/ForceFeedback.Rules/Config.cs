using System.Windows.Media;

namespace ForceFeedback.Rules
{
    /// <summary>
    /// This class provides some drawing resource like pens, brushes etc.
    /// </summary>
    internal class Config
    {
        internal static readonly int LongMethodLineCountThreshold = 10;

        internal static readonly Color LongMethodBorderColor;
        internal static readonly Color LongMethodBackgroundColor;

        internal static readonly Pen LongMethodBorderPen;
        internal static readonly Brush LongMethodBackgroundBrush;

        static Config()
        {
            // Light Gray
            LongMethodBackgroundColor = Color.FromArgb(0x20, 0x96, 0x96, 0x96);
            //LongMethodBorderColor = Colors.Red;
            LongMethodBorderColor = LongMethodBackgroundColor;

            var penBrush = new SolidColorBrush(LongMethodBorderColor);
            penBrush.Freeze();

            LongMethodBorderPen = new Pen(penBrush, 0.5);
            LongMethodBorderPen.Freeze();

            LongMethodBackgroundBrush = new SolidColorBrush(LongMethodBackgroundColor);
            LongMethodBackgroundBrush.Freeze();
        }
    }
}
