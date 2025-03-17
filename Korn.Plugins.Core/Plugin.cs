using Korn.Interface.ServiceModule;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Korn.Plugins.Core
{
    public abstract class Plugin
    {
        public Plugin()
        {
            Dispatcher = new PluginDispatcher(this);
        }

        public readonly PluginDispatcher Dispatcher;
        public bool IsAssemblyAlreadyLoaded { get; private set; }
        public PluginDirectoryInfo PluginDirectory { get; private set; }
        public KornLogger Logger { get; private set; }

        protected virtual void OnLoad() { }
        protected virtual void OnUnload() { }
        protected virtual void OnAssemblyLoaded(Assembly assembly) { }

        Dictionary<string, List<Action<Assembly>>> registeredAssemblies = new Dictionary<string, List<Action<Assembly>>>();
        protected void RegisterAssemblyLoad(string name, Action<Assembly> handler)
        {
            var list = 
                registeredAssemblies.ContainsKey(name) 
                ? registeredAssemblies[name] 
                : registeredAssemblies[name] = new List<Action<Assembly>>();

            list.Add(handler);
        }

        public class PluginDispatcher
        {
            public PluginDispatcher(Plugin plugin)
            {
                this.plugin = plugin;
            }

            Plugin plugin;

            public bool IsAssemblyAlreadyLoaded { get => plugin.IsAssemblyAlreadyLoaded; set => plugin.IsAssemblyAlreadyLoaded = value; }
            public PluginDirectoryInfo PluginDirectory { get => plugin.PluginDirectory; set => plugin.PluginDirectory = value; }
            public KornLogger Logger { get => plugin.Logger; set => plugin.Logger = value; }

            public void OnLoad() => plugin.OnLoad();
            public void OnUnload() => plugin.OnUnload();
            public void OnAssemblyLoaded(Assembly assembly)
            {
                plugin.OnAssemblyLoaded(assembly);

                var name = assembly.GetName().Name;
                var hasHandlers = plugin.registeredAssemblies.TryGetValue(name, out List<Action<Assembly>> handlers);
                if (hasHandlers)
                {
                    Logger.WriteMessage($"Plugin.OnAssemblyLoaded: handled loading of assembly \"{name}\"");
                    foreach (var handler in handlers)
                        handler(assembly);
                }    
            }
        }
    }
}