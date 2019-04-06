using System.Drawing;

namespace ForceFeedback.Core.Feedbacks
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
