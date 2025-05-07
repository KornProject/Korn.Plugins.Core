using Korn.Interface;
using Korn.Shared;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Korn.Plugins.Core
{
    public class PluginLoader
    {
        public readonly List<Plugin> Plugins = new List<Plugin>();

        public Plugin LoadPlugin(string pluginName)
        {
            var process = Process.GetCurrentProcess();

            var pluginDirectory = Korn.Interface.Plugins.GetDirectoryInfo(pluginName);
            if (!pluginDirectory.IsDirectoryExists)
                throw new KornException($"Korn.Bootstrapper.EntryPoint.Main->InitPlugins: Plugin \"{pluginName}\" directory doesn't exist.");

            if (!pluginDirectory.HasManifestFile)
                throw new KornException($"Korn.Bootstrapper.EntryPoint.Main->InitPlugins: Plugin \"{pluginName}\" manifest file \"{pluginDirectory.ManifestFilePath}\" doesn't exist.");

            var manifest = pluginDirectory.DeserializeManifest();
            if (manifest is null)
                throw new KornException($"Korn.Bootstrapper.EntryPoint.Main->InitPlugins: Plugin \"{pluginName}\" has invalid manifest file. Unable to deserialize.");

            if (!manifest.IsValid())
                throw new KornException($"Korn.Bootstrapper.EntryPoint.Main->InitPlugins: Plugin \"{pluginName}\" has invalid manifest file. Missing components.");

            PluginTarget foundPluginTarget = null;
            foreach (var target in manifest.Targets)
            {
                if (target.TargetFramework.ToString() != KornShared.CurrentTargetVersion)
                    continue;

                foreach (var targetProcess in target.TargetProcesses)
                    if (targetProcess == process.ProcessName)
                    {
                        foundPluginTarget = target;
                        break;
                    }
            }

            if (foundPluginTarget is null)
            {
                CoreEnv.Logger.WriteMessage($"The process is not suitable for plugin \"{pluginName}\".");
                return null;
            }

            var (executablePath, executableClass) = (foundPluginTarget.ExecutableFilePath, foundPluginTarget.PluginClass);
            if (!File.Exists(executablePath))
                throw new KornException($"Korn.Bootstrapper.EntryPoint.Main->InitPlugins: Plugin \"{pluginName}\" has invalid manifest file target. Executable file is doesn't exists.");

            var assembly = Assembly.LoadFrom(executablePath);
            var type = assembly.GetType(executableClass);
            if (type == null)
                throw new KornException($"Korn.Bootstrapper.EntryPoint.Main->InitPlugins: Plugin \"{pluginName}\" has invalid manifest file target. Executable class is doesn't exists.");

            if (type.BaseType != typeof(Plugin))            
                throw new KornException($"Korn.Bootstrapper.EntryPoint.Main->InitPlugins: Plugin \"{pluginName}\" has invalid manifest file target. Base class of executable class is not Plugin.");

            return InitializePlugin();

            Plugin InitializePlugin()
            {
                var pluginInstace = Activator.CreateInstance(type) as Plugin;
                if (pluginInstace is null)
                    throw new KornException($"Korn.Bootstrapper.EntryPoint.Main->InitPlugins->InitializePlugin: Plugin \"{pluginName}\": Unable create instance for plugin type \"{type.FullName}\".");

                CoreEnv.Logger.WriteMessage($"Successfully loaded plugin \"{pluginName}\"");
                var pluginDispatcher = pluginInstace.Dispatcher;
                pluginDispatcher.PluginDirectory = pluginDirectory;
                pluginDispatcher.Logger = new KornLogger(pluginDirectory.LogFilePath);
                AddPlugin(pluginInstace);
                pluginDispatcher.OnLoad();
                pluginDispatcher.IsAssemblyAlreadyLoaded = true;

                return pluginInstace;
            }
        }

        public void UnloadPlugin(Plugin plugin)
        {
            plugin.Dispatcher.OnLoad();
            RemovePlugin(plugin);
        }

        void AddPlugin(Plugin plugin) => Plugins.Add(plugin);
        void RemovePlugin(Plugin plugin) => Plugins.Remove(plugin);
    }
}