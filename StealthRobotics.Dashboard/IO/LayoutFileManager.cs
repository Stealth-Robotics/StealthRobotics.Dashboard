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
        /// <summary>
        /// Saves a dashboard layout as a json-formatted file
        /// </summary>
        /// <param name="filename">A filename. If the file doesn't exist, it will be created.</param>
        /// <param name="c">The layout to save</param>
        public static void Save(string filename, UIElementCollection c)
        {
            IEnumerable<UIElement> list = c.Cast<UIElement>();
            JsonSerializer serializer = new JsonSerializer
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                ContractResolver = new LayoutContractResolver(),
                Formatting = Formatting.Indented,
                TypeNameHandling = TypeNameHandling.Auto
            };
            using (StreamWriter sw = new StreamWriter(filename))
            {
                using (JsonWriter writer = new JsonTextWriter(sw))
                {
                    serializer.Serialize(writer, list);
                }
            }
        }

        public static UIElementCollection Load(string fileName)
        {
            return null;
        }
    }
}