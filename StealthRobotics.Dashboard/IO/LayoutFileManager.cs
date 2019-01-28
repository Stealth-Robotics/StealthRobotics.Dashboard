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
            List<UIElement> content;
            serializer.Error += Serializer_Error;
            using (StreamReader sr = new StreamReader(fileName))
            {
                using (JsonReader reader = new JsonTextReader(sr))
                {
                    content = serializer.Deserialize<List<UIElement>>(reader);
                }
            }
            serializer.Error -= Serializer_Error;
            target.Clear();
            foreach (UIElement e in content)
            {
                target.Add(e);
            }
        }

        private static void Serializer_Error(object sender, Newtonsoft.Json.Serialization.ErrorEventArgs e)
        {
            //handle any errors. possibly do something if the file isn't a layout at all
            e.ErrorContext.Handled = true;
        }
    }
}