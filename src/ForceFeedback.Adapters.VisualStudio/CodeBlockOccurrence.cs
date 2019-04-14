using Microsoft.CodeAnalysis.CSharp.Syntax;
using ForceFeedback.Core;
using System.Collections.Generic;
using ForceFeedback.Core.Feedback;

namespace ForceFeedback.Adapters.VisualStudio
{
    internal class CodeBlockOccurrence
    {
        public CodeBlockOccurrence(BlockSyntax block, IEnumerable<IFeedback> feedbacks)
        {
            Block = block;
            Feedbacks = feedbacks;
        }

        public BlockSyntax Block { get; set; }

        public IEnumerable<IFeedback> Feedbacks { get; set; }
    }
}
