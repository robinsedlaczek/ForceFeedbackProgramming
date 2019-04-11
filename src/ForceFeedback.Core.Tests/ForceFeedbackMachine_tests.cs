using System;
using Xunit;

namespace ForceFeedback.Core.Tests
{
    public class ForceFeedbackMachine_tests
    {
        [Fact]
        public void Empty_method_without_context() {
            var sut = new ForceFeedbackMachine("", "", "");
            var result = sut.RequestFeedbackForMethodCodeBlock("", 0);
            Assert.Empty(result);
        }
    }
}