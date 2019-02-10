using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace StealthRobotics.Dashboard.IO
{
    public static class SaveFileManager
    {
        private static JsonSerializer serializer = new JsonSerializer
        {
            ContractResolver = LayoutContractResolver.Instance,
            Formatting = Formatting.Indented,
            TypeNameHandling = TypeNameHandling.Auto,
            SerializationBinder = LayoutSerializationBinder.Instance
        };
        
        private static JsonSerializer vanillaConverter = new JsonSerializer
        {
            Formatting = Formatting.Indented,
            TypeNameHandling = TypeNameHandling.All
        };

        /// <summary>
        /// Saves a dashboard layout as a json-formatted file
        /// </summary>
        /// <param name="filename">A filename. If the file doesn't exist, it will be created.</param>
        /// <param name="source">The layout to save</param>
        public static void SaveLayout(string filename, UIElementCollection source)
        {
            using (StreamWriter sw = new StreamWriter(filename))
            {
                using (JsonWriter writer = new JsonTextWriter(sw))
                {
                    serializer.Serialize(writer, source);
                }
            }
        }

        /// <summary>
        /// Loads a dashboard layout from a json-formatted file
        /// </summary>
        /// <param name="fileName">A filename</param>
        /// <param name="target">The layout to put content in</param>
        public static void LoadLayout(string fileName, UIElementCollection target)
        {
            List<UIElement> content = null;
            using (StreamReader sr = new StreamReader(fileName))
            {
                using (JsonReader reader = new JsonTextReader(sr))
                {
                    try
                    {
                        content = serializer.Deserialize<List<UIElement>>(reader);
                    }
                    catch
                    {
                        MessageBox.Show("Couldn't load the layout! Either the file contains a control from an unloaded plugin, " + 
                            "or was an incorrect file format.", "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        return;
                    }
                }
            }
            target.Clear();
            foreach (UIElement e in content)
            {
                target.Add(e);
            }
        }

        private static string EnsureSettingsDir()
        {
            string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string pluginsDir = localAppData + @"\StealthDashboard";

            Directory.CreateDirectory(pluginsDir);

            return pluginsDir;
        }

        private static string GetSettingsFilePath()
        {
            string settingsDir = EnsureSettingsDir();
            string settingsFile = $"{settingsDir}\\.settings";
            return settingsFile;
        }

        private static string EnsureSettingsFile()
        {
            string settingsFile = GetSettingsFilePath();
            if(!File.Exists(settingsFile))
            {
                SaveSettings(0, false);
            }
            return settingsFile;
        }

        /// <summary>
        /// Save dashboard-wide settings
        /// </summary>
        /// <param name="team">The team number being used</param>
        /// <param name="useDS">Whether the dashboard is using the driver station connection</param>
        public static void SaveSettings(int team, bool useDS)
        {
            string settingsFile = GetSettingsFilePath();
            using (StreamWriter sw = new StreamWriter(settingsFile))
            {
                using (JsonWriter writer = new JsonTextWriter(sw))
                {
                    vanillaConverter.Serialize(writer, (team, useDS));
                }
            }
        }

        /// <summary>
        /// Loads dashboard-wide settings
        /// </summary>
        /// <returns>team: the team number to be used
        /// useDS: Whether the dashboard should use the driver station connection</returns>
        public static (int team, bool useDS) LoadSettings()
        {
            string settingsFile = EnsureSettingsFile();
            using (StreamReader sr = new StreamReader(settingsFile))
            {
                using (JsonReader reader = new JsonTextReader(sr))
                {
                    object val = vanillaConverter.Deserialize(reader);
                    return ((int, bool))val;
                }
            }
        }
    }
}