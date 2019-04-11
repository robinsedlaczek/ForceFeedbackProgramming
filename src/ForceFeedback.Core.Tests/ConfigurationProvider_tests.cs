using Xunit;

namespace ForceFeedback.Core.Tests
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