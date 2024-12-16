using System.Diagnostics.CodeAnalysis;

namespace Korn.Plugins.Core;
public abstract class Plugin
{
    [AllowNull] public string PluginDirectory;

    public virtual void OnLoad() { }
    public virtual void OnUnload() { }
}