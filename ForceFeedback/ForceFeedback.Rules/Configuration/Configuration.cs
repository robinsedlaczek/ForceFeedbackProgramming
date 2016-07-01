using System.Collections.Generic;

namespace ForceFeedback.Rules.Configuration
{
    /// <summary>
    /// This class represents the configuration model. The configuration is loaded into an instance of this class.
    /// </summary>
    internal class Configuration
    {
        public List<LongMethodLimitConfiguration> MethodTooLongLimits { get; set; }

        public static Configuration Default
        {
            get
            {
                return new Configuration()
                {
                    MethodTooLongLimits = new List<LongMethodLimitConfiguration>()
                };
            }
        }
    }
}
