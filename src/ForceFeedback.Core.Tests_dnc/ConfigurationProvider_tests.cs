using ForceFeedback.Core.adapters.configuration;
using Xunit;

namespace ForceFeedback.Core.Tests_dnc
{
    public class ConfigurationProvider_tests
    {
        [Fact]
        public void Context_empty() {
            var sut = new ConfigurationProvider("", "", "");
            var result = sut.Compile();
            Assert.Empty(result.Rules);
        }
    }
}