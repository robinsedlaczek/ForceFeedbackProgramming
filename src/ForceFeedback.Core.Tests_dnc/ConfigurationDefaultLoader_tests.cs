using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using ForceFeedback.Core.adapters.configuration;
using Newtonsoft.Json;
using Xunit;

namespace ForceFeedback.Core.Tests_dnc
{
    public class ConfigurationDefaultLoader_tests
    {
        [Fact]
        public void Load_default_config_json() {
            var result = ConfigurationDefaultLoader.Load_default_configuration_text();
            Assert.True(result.IndexOf("Version") > 0);
            Assert.True(result.IndexOf("Burlywood") > 0);
        }
    }
}