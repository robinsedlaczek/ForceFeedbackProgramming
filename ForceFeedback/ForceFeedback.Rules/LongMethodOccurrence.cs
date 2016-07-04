using ForceFeedback.Rules.Configuration;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ForceFeedback.Rules
{
    internal class LongMethodOccurrence
    {
        public LongMethodOccurrence(BlockSyntax methodDeclaration, LongMethodLimitConfiguration limitConfiguration)
        {
            MethodDeclaration = methodDeclaration;
            LimitConfiguration = limitConfiguration;
        }

        public BlockSyntax MethodDeclaration { get; set; }

        public LongMethodLimitConfiguration LimitConfiguration { get; set; }
    }
}