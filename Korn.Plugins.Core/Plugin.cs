using Korn.Interface.ServiceModule;

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

        public virtual void OnLoad() { }
        public virtual void OnUnload() { }

        public class PluginDispatcher
        {
            public PluginDispatcher(Plugin plugin)
            {
                this.plugin = plugin;
            }

            Plugin plugin;

            public bool IsAssemblyAlreadyLoaded { get => plugin.IsAssemblyAlreadyLoaded; set => plugin.IsAssemblyAlreadyLoaded = value; }
            public PluginDirectoryInfo PluginDirectory { get => plugin.PluginDirectory; set => plugin.PluginDirectory = value; }
        }
    }
}