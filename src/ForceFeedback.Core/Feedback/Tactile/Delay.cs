namespace ForceFeedback.Core.Feedback.Tactile
{
    public class Delay : IFeedback
    {
        public Delay(int milliseconds)
        {
            Milliseconds = milliseconds;
        }

        public int Milliseconds { get; }
    }
}
