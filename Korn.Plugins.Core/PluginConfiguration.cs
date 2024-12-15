namespace Korn.Plugins.Core;
public record PluginConfiguration(
    string Name,
    string Version,
    PluginAuthor[] Authors,
    PluginTarget[] Targets
);

public record PluginAuthor(
    string Name, 
    string? Github
);

public record PluginTarget(
    PluginFrameworkTarget TargetFramework,
    string[]? TargetProcesses,
    string ExecutableFilePath
);

public enum PluginFrameworkTarget
{
    NetFramework472,
    Net8
}