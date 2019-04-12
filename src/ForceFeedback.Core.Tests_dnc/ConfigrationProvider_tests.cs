using System.Drawing;
using ForceFeedback.Core.adapters.configuration;
using Xunit;

namespace ForceFeedback.Core.Tests_dnc
{
    public class ConfigrationProvider_tests
    {
        [Fact]
        public void Load_default_config() {
            var sut = new ConfigurationProvider("", "", "");
            var result = sut.Configuration;
            
            Assert.True(result.TryFindRule(29, out var rule));
            Assert.Equal(Color.BurlyWood, rule.BackgroundColor);
        }
    }
}