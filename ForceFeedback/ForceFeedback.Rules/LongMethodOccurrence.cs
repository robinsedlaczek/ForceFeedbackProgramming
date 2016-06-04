using ForceFeedback.Rules.Configuration;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ForceFeedback.Rules
{
    internal class LongMethodOccurrence
    {
        public LongMethodOccurrence(MethodDeclarationSyntax methodDeclaration, LongMethodLimitConfiguration limitConfiguration)
        {
            MethodDeclaration = methodDeclaration;
            LimitConfiguration = limitConfiguration;
        }

        public MethodDeclarationSyntax MethodDeclaration { get; set; }

        public LongMethodLimitConfiguration LimitConfiguration { get; set; }
    }
}