using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Permissions;
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
        internal static readonly Pen LongMethodBorderPen;
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

            LongMethodBorderPen = new Pen(penBrush, 0.5);
            LongMethodBorderPen.Freeze();

            LongMethodBackgroundBrush = new SolidColorBrush(LongMethodBackgroundColor);
            LongMethodBackgroundBrush.Freeze();
        }

        private static FileSystemWatcher InitConfigurationFileWatcher()
        {
            var configFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "ForceFeedbackProgramming");

            var watcher = new FileSystemWatcher()
            {
                Path = configFolderPath,
                NotifyFilter = NotifyFilters.LastWrite,
                Filter = "Config.json"
            };

            watcher.Changed += OnFileChanged;
            watcher.EnableRaisingEvents = true;

            return watcher;
        }

        private static void OnFileChanged(object sender, FileSystemEventArgs e)
        {
            var configFileChanged = e.ChangeType == WatcherChangeTypes.Changed;

            if (configFileChanged)
                Configuration = LoadConfiguration();
        }

        private static Configuration LoadConfiguration()
        {
            var tempPath = string.Empty;

            try
            {
                var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), @"ForceFeedbackProgramming\Config.json");
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

    }
}
