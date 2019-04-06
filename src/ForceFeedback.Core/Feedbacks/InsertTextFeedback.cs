using System.Drawing;

namespace ForceFeedback.Core.Feedbacks
{
    public class InsertTextFeedback : IFeedback
    {
        public InsertTextFeedback(int position, string text)
        {
            Position = position;
            Text = text;

        }

        public int Position { get; }

        public string Text { get; }
    }
}
