using System.Drawing;
using System.IO;
using ForceFeedback.Core.adapters.configuration;
using Xunit;

namespace ForceFeedback.Core.Tests_dnc
{
    public class ConfigrationProvider_tests
    {
        [Fact]
        public void Load_default_config_if_no_paths_are_given() {
            var sut = new ConfigurationProvider("", "", "");
            var result = sut.Configuration;
            
            Assert.True(result.TryFindRule(29, out var rule));
            Assert.Equal(Color.BurlyWood, rule.BackgroundColor);
        }

        
        [Fact]
        public void Load_config_from_solution_path()
        {
            if (Directory.Exists("slnfolder"))
                Directory.Delete("slnfolder", true);
            Directory.CreateDirectory("slnfolder");
            File.Copy("../../../sampledata/ConfigurationDefaults.json", "slnfolder/.forcefeedbackprogramming");
            
            var sut = new ConfigurationProvider("slnfolder", "", "");
            var result = sut.Configuration;
            
            Assert.True(result.TryFindRule(1, out var rule));
            Assert.Equal(Color.Beige, rule.BackgroundColor);
        }
    }
}