using System.Drawing;

namespace ForceFeedback.Core.Feedback.Visual
{
    public class Colorization : IFeedback
    {
        public Colorization(Color backgroundColor, double backgroundColorTransparency) {
            BackgroundColor = backgroundColor;
            OutlineColor = Color.White;
            BackgroundColorTransparency = backgroundColorTransparency;
        }

        public Color BackgroundColor { get; set; }
        public double BackgroundColorTransparency { get; }

        public Color OutlineColor { get; }
        
    }
}
