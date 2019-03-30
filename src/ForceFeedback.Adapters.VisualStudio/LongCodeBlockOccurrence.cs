using Microsoft.CodeAnalysis.CSharp.Syntax;
using ForceFeedback.Adapters.VisualStudio.Configuration;
using ForceFeedback.Core;
using System.Collections.Generic;

namespace ForceFeedback.Adapters.VisualStudio
{
    internal class LongCodeBlockOccurrence
    {
        public LongCodeBlockOccurrence(BlockSyntax block, List<IFeedback> feedbacks)
        {
            Block = block;
            Feedbacks = feedbacks;
        }

        public BlockSyntax Block { get; set; }

        public List<IFeedback> Feedbacks { get; set; }
    }
}