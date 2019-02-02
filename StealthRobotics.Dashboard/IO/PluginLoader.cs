using StealthRobotics.Dashboard.API.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using System.Windows;

namespace StealthRobotics.Dashboard.IO
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
                    //might have ANY dll in plugins, make sure we only have valid .NET plugins
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
            try
            {
                File.Copy(path, $"{pluginDir}\\{filename}");
            }
            catch
            {
                MessageBox.Show("You're trying to load a plugin that is already loaded! " +
                    "If you need to load a newer version, unload the old one first.", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        public static IEnumerable<string> GetLoadedPlugins()
        {
            string pluginDir = EnsurePluginsDir();
            return Directory.GetFiles(pluginDir, "*.dll").Select((s) => Path.GetFileNameWithoutExtension(s));
        }

        public static void UnloadFromQueue()
        {
            string pluginDir = EnsurePluginsDir();
            IEnumerable<string> pluginsToRemove = new List<string>();
            using (FileStream qFile = File.Open($"{pluginDir}\\pending", FileMode.OpenOrCreate))
            {
                using (StreamReader r = new StreamReader(qFile))
                {
                    pluginsToRemove = r.ReadToEnd().Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                }
            }
            foreach(string plugin in pluginsToRemove)
            {
                UnloadPlugin(plugin.Trim());
            }
            File.Delete($"{pluginDir}\\pending");
        }

        public static void UnloadPlugin(string pluginName)
        {
            string pluginDir = EnsurePluginsDir();
            try
            {
                File.Delete($"{pluginDir}\\{pluginName}.dll");
            }
            catch
            {
                MessageBox.Show("This plugin can't be unloaded because it was used this session. " + 
                    "It will be unloaded next time you start the dashboard. ",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                using (FileStream qFile = File.Open($"{pluginDir}\\pending", FileMode.Append))
                {
                    using (StreamWriter w = new StreamWriter(qFile))
                    {
                        w.WriteLine(pluginName);
                    }
                }
            }
        }
    }
}
