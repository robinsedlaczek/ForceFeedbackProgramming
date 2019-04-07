using ForceFeedback.Rules.Configuration;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ForceFeedback.Rules
{
    internal class LongCodeBlockOccurrence
    {
        public LongCodeBlockOccurrence(BlockSyntax block, LongMethodLimitConfiguration limitConfiguration)
        {
            Block = block;
            LimitConfiguration = limitConfiguration;
        }

        public BlockSyntax Block { get; set; }

        public LongMethodLimitConfiguration LimitConfiguration { get; set; }
    }
}