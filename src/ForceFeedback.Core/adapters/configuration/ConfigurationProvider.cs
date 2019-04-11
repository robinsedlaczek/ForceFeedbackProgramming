using System.Collections;
using System.Collections.Generic;

namespace ForceFeedback.Core
{
    class ConfigurationProvider
    {
        public ConfigurationProvider(string solutionFilePath, string projectFilePath, string sourceFilePath)
        {}


        public Configuration Compile()
        {
            return new Configuration(new List<Configuration.Rule>());
        }
    }
}