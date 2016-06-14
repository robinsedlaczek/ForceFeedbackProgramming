using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForceFeedback.Rules.Configuration
{
    internal class Configuration
    {
        public List<LongMethodLimitConfiguration> MethodTooLongLimits { get; set; }

        public static Configuration Default
        {
            get
            {
                return new Configuration()
                {
                    MethodTooLongLimits = new List<LongMethodLimitConfiguration>();
                }
            }
        }
    }
}
