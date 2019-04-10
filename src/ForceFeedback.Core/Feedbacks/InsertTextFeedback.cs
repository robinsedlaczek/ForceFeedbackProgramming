namespace ForceFeedback.Core.Feedbacks
{
    public class InsertTextFeedback : IFeedback
    {
        public InsertTextFeedback(string text)
        {
            Text = text;
        }

        public string Text { get; }
    }
}
