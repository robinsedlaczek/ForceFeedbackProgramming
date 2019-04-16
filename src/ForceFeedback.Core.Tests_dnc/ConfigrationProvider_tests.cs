using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using ForceFeedback.Core.adapters.configuration;
using Xunit;

namespace ForceFeedback.Core.Tests_dnc
{
    /*
     * Different sln folder names are needed due to parallel execution of tests.
     * Otherwise one test might interfere with the folder structure of another.
     */
    public class ConfigrationProvider_tests
    {
        private readonly string TEST_SLN_PATH = "slnfolder-" + Guid.NewGuid().ToString();
        
        
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
            if (Directory.Exists(TEST_SLN_PATH))
                Directory.Delete(TEST_SLN_PATH, true);
            Directory.CreateDirectory(TEST_SLN_PATH);
            File.Copy("../../../sampledata/ConfigurationDefaults.json", Path.Combine(TEST_SLN_PATH,".forcefeedbackprogramming"));
            
            var sut = new ConfigurationProvider(Path.Combine(TEST_SLN_PATH, "my.sln"), "", "");
            var result = sut.Configuration;
            
            Assert.True(result.TryFindRule(1, out var rule));
            Assert.Equal(Color.Beige, rule.BackgroundColor);
        }
        
        
        [Fact]
        public void Create_config_in_sln_folder_from_defaults_if_none_found()
        {
            if (Directory.Exists(TEST_SLN_PATH))
                Directory.Delete(TEST_SLN_PATH, true);
            Directory.CreateDirectory(TEST_SLN_PATH);
            
            var sut = new ConfigurationProvider(Path.Combine(TEST_SLN_PATH, "my.sln"), "", "");
            var result = sut.Configuration;
            
            Assert.True(result.TryFindRule(30, out var rule));
            Assert.True(File.Exists(Path.Combine(TEST_SLN_PATH, ".forcefeedbackprogramming")));
            Assert.Equal(26, rule.MinimumNumberOfLinesInMethod);
            Assert.Equal(Color.BurlyWood, rule.BackgroundColor);
        }


        [Fact]
        public void Pick_first_config_found_from_file_path()
        {
            if (Directory.Exists(TEST_SLN_PATH))
                Directory.Delete(TEST_SLN_PATH, true);
            Directory.CreateDirectory(Path.Combine(TEST_SLN_PATH, "prjfolder", "filefolder"));
            
            File.Copy("../../../sampledata/ConfigurationDefaults.json", Path.Combine(TEST_SLN_PATH, "prjfolder/filefolder/.forcefeedbackprogramming"));
            File.Copy("../../../sampledata/ConfigurationEmpty.json", Path.Combine(TEST_SLN_PATH, "prjfolder/.forcefeedbackprogramming"));
            
            var sut = new ConfigurationProvider(Path.Combine(TEST_SLN_PATH, "my.sln"), 
                                                Path.Combine(TEST_SLN_PATH, "prjfolder/my.csproj"), 
                                                Path.Combine(TEST_SLN_PATH, "prjfolder/filefolder/my.cs"));
            var result = sut.Configuration;
            
            Assert.True(result.TryFindRule(1, out var rule));
            Assert.Equal(Color.Beige, rule.BackgroundColor);
        }
        
        
        [Fact]
        public void Pick_first_config_found_from_project_path()
        {
            if (Directory.Exists(TEST_SLN_PATH))
                Directory.Delete(TEST_SLN_PATH, true);
            Directory.CreateDirectory(Path.Combine(TEST_SLN_PATH, "prjfolder", "filefolder"));
            
            File.Copy("../../../sampledata/ConfigurationDefaults.json", Path.Combine(TEST_SLN_PATH, "prjfolder/.forcefeedbackprogramming"));
            File.Copy("../../../sampledata/ConfigurationEmpty.json", Path.Combine(TEST_SLN_PATH, ".forcefeedbackprogramming"));
            
            var sut = new ConfigurationProvider(Path.Combine(TEST_SLN_PATH, "my.sln"), 
                                                Path.Combine(TEST_SLN_PATH, "prjfolder/my.csproj"), 
                                                Path.Combine(TEST_SLN_PATH, "prjfolder/filefolder/my.cs"));
            var result = sut.Configuration;
            
            Assert.True(result.TryFindRule(1, out var rule));
            Assert.Equal(Color.Beige, rule.BackgroundColor);
        }
    }
}
