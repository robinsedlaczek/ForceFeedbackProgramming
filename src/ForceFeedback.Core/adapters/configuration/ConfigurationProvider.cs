using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ForceFeedback.Core.adapters.configuration
{
    class ConfigurationProvider
    {
        private const string DEFAUL_CONFIG_FILENAME = ".forcefeedbackprogramming";

        public ConfigurationProvider(string solutionFilePath, string projectFilePath, string sourceFilePath)
        {
            var solutionFolderPath = Path.GetDirectoryName(solutionFilePath);
            var projectFolderPath = Path.GetDirectoryName(projectFilePath);
            var sourceFolderPath = Path.GetDirectoryName(sourceFilePath);

            /*
             * Find config file in one of the paths provided - or create it from defaults in the solution path.
             */
            if (Try_to_find_config_file(solutionFolderPath, projectFolderPath, sourceFolderPath, out var configFilePath))
            {
                var configText = File.ReadAllText(configFilePath);
                Configuration = Deserialize_configuration(configText);
            }
            else
            {
                var configText = ConfigurationDefaultLoader.Load_default_configuration_text();
                Create_config_file(solutionFolderPath, configText);
                Configuration = Deserialize_configuration(configText);
            }
        }


        public Configuration Configuration { get; }


        private static bool Try_to_find_config_file(string solutionFolderPath, string projectFolderPath, string sourceFolderPath,
                                                    out string configFilePath)
        {
            if (Try_find_here(sourceFolderPath, out configFilePath))
                return true;
            if (Try_find_here(projectFolderPath, out configFilePath))
                return true;
            return Try_find_here(solutionFolderPath, out configFilePath);


            bool Try_find_here(string folderPath, out string filePath)
            {
                filePath = "";
                if (string.IsNullOrWhiteSpace(folderPath))
                    return false;
                filePath = Path.Combine(folderPath, DEFAUL_CONFIG_FILENAME);
                return File.Exists(filePath);
            }
        }


        private static void Create_config_file(string path, string configText)
        {
            if (string.IsNullOrWhiteSpace(path))
                return;

            var configFilePath = Path.Combine(path, DEFAUL_CONFIG_FILENAME);
            File.WriteAllText(configFilePath, configText);
        }


        private static Configuration Deserialize_configuration(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return new Configuration(new Configuration.FeedbackRule[0]);

            // Deserialize to anonymously typed object tree
            // (This way no dynamic objects are needed with additional assembly refs. Also no additional
            // type needs to be defined polluting the namespace.)
            var schema = new
            {
                Version = "",
                FeedbackRules = new[] {
                    new {
                        MinimumNumberOfLinesInMethod = 0,
                        BackgroundColor = "",
                        BackgroundColorTransparency = 0.0,
                        NoiseDistance = 0,
                        NoiseLevel = 0,
                        Delay = 0
                    }
                }
            };
            var tmpConfig = JsonConvert.DeserializeAnonymousType(text, schema);
            if (tmpConfig.Version != "1.0")
                throw new InvalidOperationException("Wrong or missing version number for configuration!");

            // Map anonymously typed object tree to strongly typed Configuration
            return new Configuration(tmpConfig.FeedbackRules.Select(tr => new Configuration.FeedbackRule(
                tr.MinimumNumberOfLinesInMethod,
                Color.FromName(tr.BackgroundColor),
                tr.BackgroundColorTransparency,
                tr.NoiseDistance,
                tr.NoiseLevel,
                tr.Delay
            )));
        }
    }
}
