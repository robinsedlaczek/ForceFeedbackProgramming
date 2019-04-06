namespace ForceFeedback.Core
{
    public class ForceFeedbackContext
    {
        public string FilePath { get; set; }
        public int LineCount { get; set; }
        public string MethodName { get; set; }
        public string Project { get; set; }
        public string Assembly { get; set; }
        public int InsertedAt { get; set; }
        public string InsertedText { get; set; }
        public string ReplacedText { get; set; }
        public int CaretPosition { get; set; }
    }
}
