using Microsoft.CodeAnalysis.CSharp.Syntax;
using ForceFeedback.Core;
using System.Collections.Generic;

namespace ForceFeedback.Adapters.VisualStudio
{
    internal class CodeBlockOccurrence
    {
        public CodeBlockOccurrence(BlockSyntax block, List<IFeedback> feedbacks)
        {
            Block = block;
            Feedbacks = feedbacks;
        }

        public BlockSyntax Block { get; set; }

        public List<IFeedback> Feedbacks { get; set; }
    }
}