using System.Drawing;

namespace ForceFeedback.Core.Feedbacks
{
    public class DrawColoredBackgroundFeedback : IFeedback
    {
        public DrawColoredBackgroundFeedback(Color backgroundColor, double backgroundTransparency)
        {
            BackgroundColor = backgroundColor;
            OutlineColor = Color.White;
            BackgroundTransparency = backgroundTransparency;
        }

        public Color BackgroundColor { get; set; }

        public Color OutlineColor { get; }
        
        public double BackgroundTransparency { get; }
    }
}
