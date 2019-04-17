using System.Diagnostics;
using Xunit;

namespace ForceFeedback.Core.Tests_dnc
{
    // Original source and idea: https://lostechies.com/jimmybogard/2013/06/20/run-tests-explicitly-in-xunit-net/
    internal class DebugFact : FactAttribute
    {
        public DebugFact()
        {
            if (!Debugger.IsAttached)
            {
                Skip = "Only running in interactive mode with debugger attached.";
            }
        }
    }
}
