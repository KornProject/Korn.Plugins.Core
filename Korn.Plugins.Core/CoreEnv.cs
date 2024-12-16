using System.Diagnostics.CodeAnalysis;

namespace Korn.Plugins.Core;
public static class CoreEnv
{
    [AllowNull] public static string KornPath;

    public static readonly List<Plugin> PluginInstances = [];
}   