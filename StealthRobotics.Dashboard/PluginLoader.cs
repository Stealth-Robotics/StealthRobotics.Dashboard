using StealthRobotics.Dashboard.API.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;

namespace StealthRobotics.Dashboard
{
    public static class PluginLoader
    {
        private static string EnsurePluginsDir()
        {
            string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string pluginsDir = localAppData + @"\StealthDashboard\Plugins";

            Directory.CreateDirectory(pluginsDir);

            return pluginsDir;
        }

        public static IEnumerable<Type> GetControls()
        {
            string pluginsDir = EnsurePluginsDir();
            Assembly thisAssembly = typeof(PluginLoader).Assembly;
            IEnumerable<Type> locals = thisAssembly.GetTypes()
                .Where((t) => typeof(SourcedControl).IsAssignableFrom(t));
            IEnumerable<Type> plugins = Directory.GetFiles(pluginsDir, "*.dll")
                .SelectMany((file) =>
                {
                    try
                    {
                        Assembly pluginAsm = Assembly.LoadFrom(file);
                        return pluginAsm.GetTypes()
                            .Where((t) => typeof(SourcedControl).IsAssignableFrom(t));
                    }
                    catch
                    {
                        return new List<Type>();
                    }
                }).Distinct();
            return locals.Union(plugins);
        }

        public static void LoadPlugin(string path)
        {
            string filename = Path.GetFileName(path);
            string pluginDir = EnsurePluginsDir();
            File.Copy(path, $"{pluginDir}\\filename", true);
        }

        public static IEnumerable<string> GetLoadedPlugins()
        {
            string pluginDir = EnsurePluginsDir();
            return Directory.GetFiles(pluginDir).Select((s) => Path.GetFileNameWithoutExtension(s));
        }

        public static void UnloadPlugin(string pluginName)
        {
            string pluginDir = EnsurePluginsDir();
            File.Delete($"{pluginDir}\\{pluginName}.dll");
        }
    }
}
