namespace ForceFeedback.Core.Feedback.Tactile
{
    public class Noise : IFeedback
    {
        public Noise(string text) {
            Text = text;
        }

        public string Text { get; }
    }
}
