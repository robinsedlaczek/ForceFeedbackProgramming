using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media;

namespace ForceFeedback.Rules
{
    /// <summary>
    /// This class provides some drawing resource like pens, brushes etc.
    /// </summary>
    internal class Config
    {
        internal static readonly Color LongMethodBorderColor;
        internal static readonly Color LongMethodBackgroundColor;

        internal static readonly Pen LongMethodBorderPen;
        internal static readonly Brush LongMethodBackgroundBrush;

        static Config()
        {
            LoadConfiguration();

            // Light Gray
            LongMethodBackgroundColor = Color.FromArgb(0x20, 0x96, 0x96, 0x96);
            LongMethodBorderColor = Colors.Red;
            //LongMethodBorderColor = LongMethodBackgroundColor;

            var penBrush = new SolidColorBrush(LongMethodBorderColor);
            penBrush.Freeze();

            LongMethodBorderPen = new Pen(penBrush, 0.5);
            LongMethodBorderPen.Freeze();

            LongMethodBackgroundBrush = new SolidColorBrush(LongMethodBackgroundColor);
            LongMethodBackgroundBrush.Freeze();
        }

        internal static readonly IList<MethodTooLongLimit> MethodsTooLongLimits = new List<MethodTooLongLimit>();

        private static void LoadConfiguration()
        {
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), @"ForceFeedbackProgramming\Config.json");

            MethodsTooLongLimits.Add(new MethodTooLongLimit(10, null));
            MethodsTooLongLimits.Add(new MethodTooLongLimit(15, Color.FromArgb(150, 150, 150, 150)));
            MethodsTooLongLimits.Add(new MethodTooLongLimit(20, Color.FromArgb(150, 200, 150, 150)));
            MethodsTooLongLimits.Add(new MethodTooLongLimit(25, Color.FromArgb(150, 250, 150, 150)));
        }

    }
}
