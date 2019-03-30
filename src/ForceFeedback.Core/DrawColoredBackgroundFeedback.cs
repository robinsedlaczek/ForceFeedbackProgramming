using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace ForceFeedback.Core
{
    public class DrawColoredBackgroundFeedback : IFeedback
    {
        public DrawColoredBackgroundFeedback(Color backgroundColor, Color outlineColor)
        {
            BackgroundColor = backgroundColor;
            OutlineColor = outlineColor;
        }

        public Color BackgroundColor { get; set; }

        public Color OutlineColor { get; }
    }
}
