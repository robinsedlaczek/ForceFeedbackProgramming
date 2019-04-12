using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ForceFeedback.Core.adapters.configuration
{
    class ConfigurationProvider
    {
        private readonly Configuration _config;

        public ConfigurationProvider(string solutionFilePath, string projectFilePath, string sourceFilePath) {
            var configText = ConfigurationDefaultLoader.Load_default_configuration_text();
            _config = Deserialize_configuration(configText);
        }

        internal ConfigurationProvider(Configuration config) { _config = config; }


        public Configuration Configuration => _config;
        
        
        internal static Configuration Deserialize_configuration(string text) {
            if (string.IsNullOrWhiteSpace(text)) return new Configuration(new Configuration.FeedbackRule[0]);

            var schema = new {
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
            
            if (tmpConfig.Version != "1.0") throw new InvalidOperationException("Wrong or missing version number for configuration!");

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