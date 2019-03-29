using Microsoft.CodeAnalysis.CSharp.Syntax;
using ForceFeedback.Adapters.VisualStudio.Configuration;

namespace ForceFeedback.Adapters.VisualStudio
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