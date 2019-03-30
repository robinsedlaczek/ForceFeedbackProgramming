namespace ForceFeedback.Core
{
    public class ForceFeedbackContext
    {
        public string FilePath { get; set; }
        public int LineCount { get; set; }
        public string MethodName { get; set; }
        public string Project { get; set; }
        public string Assembly { get; set; }
    }
}
