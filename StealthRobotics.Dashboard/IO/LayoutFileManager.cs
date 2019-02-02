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
    public static class LayoutFileManager
    {
        private static JsonSerializer serializer = new JsonSerializer
        {
            ContractResolver = LayoutContractResolver.Instance,
            Formatting = Formatting.Indented,
            TypeNameHandling = TypeNameHandling.Auto,
            SerializationBinder = LayoutSerializationBinder.Instance
        };

        /// <summary>
        /// Saves a dashboard layout as a json-formatted file
        /// </summary>
        /// <param name="filename">A filename. If the file doesn't exist, it will be created.</param>
        /// <param name="source">The layout to save</param>
        public static void Save(string filename, UIElementCollection source)
        {
            using (StreamWriter sw = new StreamWriter(filename))
            {
                using (JsonWriter writer = new JsonTextWriter(sw))
                {
                    serializer.Serialize(writer, source);
                }
            }
        }

        public static void Load(string fileName, UIElementCollection target)
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
    }
}