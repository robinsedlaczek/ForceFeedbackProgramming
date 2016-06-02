namespace ForceFeedback.Rules.Configuration
{
    public class ReplacePattern
    {
        public ReplacePattern(string replacementCharacters, int numberOfReplacementsAtKeystroke)
        {
            ReplacementCharacters = replacementCharacters;
            NumberOfReplacementsAtKeystroke = numberOfReplacementsAtKeystroke;
        }

        public string ReplacementCharacters
        {
            get;
        }

        public int NumberOfReplacementsAtKeystroke
        {
            get;
        }
    }
}