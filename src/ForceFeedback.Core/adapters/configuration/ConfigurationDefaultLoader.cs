using System.IO;
using System.Reflection;

namespace ForceFeedback.Core.adapters.configuration
{
    internal static class ConfigurationDefaultLoader
    {
        private const string DEFAUL_CONFIGURATION_RESOURCE_NAME = "ConfigurationDefaultsV1.0.json";

        public static string Load_default_configuration_text()
        {
            var fullResourceName = typeof(ConfigurationProvider).Namespace + "." + DEFAUL_CONFIGURATION_RESOURCE_NAME;
            return Load_embedded_resource_text(fullResourceName);
        }

        private static string Load_embedded_resource_text(string resourceName, Assembly assembly = null)
        {
            if (assembly == null)
                assembly = Assembly.GetExecutingAssembly();

            using (var stream = assembly.GetManifestResourceStream(resourceName))
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
