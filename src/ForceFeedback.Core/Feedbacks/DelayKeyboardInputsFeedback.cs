namespace ForceFeedback.Core.Feedbacks
{
    public class DelayKeyboardInputsFeedback : IFeedback
    {
        public DelayKeyboardInputsFeedback(int milliseconds)
        {
            Milliseconds = milliseconds;
        }

        public int Milliseconds { get; private set; }
    }
}
