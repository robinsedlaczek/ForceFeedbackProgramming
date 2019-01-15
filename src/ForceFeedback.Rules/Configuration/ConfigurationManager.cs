using System;
using System.IO;
using System.Reflection;
using System.Windows.Media;

namespace ForceFeedback.Rules.Configuration
{
    /// <summary>
    /// This class provides some drawing resource like pens, brushes etc..
    /// </summary>
    internal class ConfigurationManager
    {
        internal static readonly Color LongMethodBorderColor;
        internal static readonly Color LongMethodBackgroundColor;
        internal static readonly Pen LongCodeBlockBorderPen;
        internal static readonly Brush LongMethodBackgroundBrush;

        internal static readonly FileSystemWatcher ConfigurationFileWatcher = InitConfigurationFileWatcher();
        internal static Configuration Configuration = LoadConfiguration();

        static ConfigurationManager()
        {
            LoadConfiguration();

            // Light Gray
            LongMethodBackgroundColor = Color.FromArgb(0x20, 0x96, 0x96, 0x96);
            LongMethodBorderColor = Colors.Red;

            var penBrush = new SolidColorBrush(LongMethodBorderColor);
            penBrush.Freeze();

            LongCodeBlockBorderPen = new Pen(penBrush, 0.5);
            LongCodeBlockBorderPen.Freeze();

            LongMethodBackgroundBrush = new SolidColorBrush(LongMethodBackgroundColor);
            LongMethodBackgroundBrush.Freeze();
        }

        private static FileSystemWatcher InitConfigurationFileWatcher()
        {
            var configFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "ForceFeedbackProgramming");

            if (!Directory.Exists(configFolderPath))
                Directory.CreateDirectory(configFolderPath);

            var watcher = new FileSystemWatcher()
            {
                Path = configFolderPath,
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName
            };

            watcher.Changed += OnFileChanged;
            watcher.Renamed += OnFileRenamed;

            watcher.EnableRaisingEvents = true;

			Global.ConfigFilePathChanged += () => { Configuration = LoadConfiguration(); };

            return watcher;
        }

        private static void OnFileRenamed(object sender, RenamedEventArgs e)
        {
            if (e.Name.ToLower() == Global.CONFIG_FILE_NAME)
                Configuration = LoadConfiguration();
        }

        private static void OnFileChanged(object sender, FileSystemEventArgs e)
        {
            if (e.Name.ToLower() == Global.CONFIG_FILE_NAME)
                Configuration = LoadConfiguration();
        }

        private static Configuration LoadConfiguration()
        {
            var tempPath = string.Empty;

            try
            {
				var path = Global.ConfigFilePath;
				if (path == "")
					return Configuration.Default;

                CreateDefaultConfigurationIfNotExists(path);

                tempPath = Path.GetTempFileName();

                File.Delete(tempPath);
                File.Copy(path, tempPath);

                using (var reader = new StreamReader(tempPath))
                {
                    using (var jsonReader = new Newtonsoft.Json.JsonTextReader(reader))
                    {
                        var serializer = new Newtonsoft.Json.JsonSerializer();
                        var result = serializer.Deserialize<Configuration>(jsonReader);

                        return result;
                    }
                }
            }
            catch (Newtonsoft.Json.JsonException exception)
            {
                // TODO: [RS] Handle exception!

                return Configuration.Default;
            }
            catch (Exception exception)
            {
                // TODO: [RS] Handle exception!

                return Configuration.Default;
            }
            finally
            {
                File.Delete(tempPath);
            }
        }

        private static void CreateDefaultConfigurationIfNotExists(string path)
        {
            if (File.Exists(path))
                return;

            try
            {
                var assembly = Assembly.GetExecutingAssembly();
                var resourceName = "ForceFeedback.Rules.Resources.Config.json";

                using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        var defaultConfig = reader.ReadToEnd();
                        File.WriteAllText(path, defaultConfig);
                    }
                }
            }
            catch (Exception)
            {
                // TODO: [RS] Handle exception!
            }
        }
    }
}
