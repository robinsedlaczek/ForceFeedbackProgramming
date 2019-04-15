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
            if (Directory.Exists("slnfolder_2"))
                Directory.Delete("slnfolder_2", true);
            Directory.CreateDirectory("slnfolder_2");
            File.Copy("../../../sampledata/ConfigurationDefaults.json", "slnfolder_2/.forcefeedbackprogramming");
            
            var sut = new ConfigurationProvider("slnfolder_2/my.sln", "", "");
            var result = sut.Configuration;
            
            Assert.True(result.TryFindRule(1, out var rule));
            Assert.Equal(Color.Beige, rule.BackgroundColor);
        }
        
        
        [Fact]
        public void Create_config_in_sln_folder_from_defaults_if_none_found()
        {
            if (Directory.Exists("slnfolder_3"))
                Directory.Delete("slnfolder_3", true);
            Directory.CreateDirectory("slnfolder_3");
            
            var sut = new ConfigurationProvider("slnfolder_3/my.sln", "", "");
            var result = sut.Configuration;
            
            Assert.True(result.TryFindRule(30, out var rule));
            Assert.True(File.Exists("slnfolder_3/.forcefeedbackprogramming"));
            Assert.Equal(26, rule.MinimumNumberOfLinesInMethod);
            Assert.Equal(Color.BurlyWood, rule.BackgroundColor);
        }


        [Fact]
        public void Pick_first_config_found_from_file_path()
        {
            if (Directory.Exists("slnfolder_4"))
                Directory.Delete("slnfolder_4", true);
            Directory.CreateDirectory("slnfolder_4/prjfolder/filefolder");
            
            File.Copy("../../../sampledata/ConfigurationDefaults.json", "slnfolder_4/prjfolder/filefolder/.forcefeedbackprogramming");
            File.Copy("../../../sampledata/ConfigurationEmpty.json", "slnfolder_4/prjfolder/.forcefeedbackprogramming");
            
            var sut = new ConfigurationProvider("slnfolder_4/my.sln", "slnfolder_4/prjfolder/my.csproj", "slnfolder_4/prjfolder/filefolder/my.cs");
            var result = sut.Configuration;
            
            Assert.True(result.TryFindRule(1, out var rule));
            Assert.Equal(Color.Beige, rule.BackgroundColor);
        }
        
        
        [Fact]
        public void Pick_first_config_found_from_project_path()
        {
            if (Directory.Exists("slnfolder_5"))
                Directory.Delete("slnfolder_5", true);
            Directory.CreateDirectory("slnfolder_5/prjfolder/filefolder");
            
            File.Copy("../../../sampledata/ConfigurationDefaults.json", "slnfolder_5/prjfolder/.forcefeedbackprogramming");
            File.Copy("../../../sampledata/ConfigurationEmpty.json", "slnfolder_5/.forcefeedbackprogramming");
            
            var sut = new ConfigurationProvider("slnfolder_5/my.sln", "slnfolder_5/prjfolder/my.csproj", "slnfolder_5/prjfolder/filefolder/my.cs");
            var result = sut.Configuration;
            
            Assert.True(result.TryFindRule(1, out var rule));
            Assert.Equal(Color.Beige, rule.BackgroundColor);
        }
    }
}
