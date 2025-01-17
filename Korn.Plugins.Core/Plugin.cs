namespace Korn.Plugins.Core
{
    public abstract class Plugin
    {
        public string PluginDirectory;

        public virtual void OnLoad() { }
        public virtual void OnUnload() { }
    }
}