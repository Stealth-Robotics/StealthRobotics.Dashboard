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

        public static void UnloadPlugin(string pluginName)
        {
            try
            {
                string pluginDir = EnsurePluginsDir();
                File.Delete($"{pluginDir}\\{pluginName}.dll");
            }
            catch(Exception e)
            {
                MessageBox.Show("Can't unload that plugin because it is in use!",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }
    }
}
