namespace Korn.Plugins.Core;
public class AssemblyWatcher : IDisposable
{
    public AssemblyWatcher()
    {
        AppDomain.CurrentDomain.AssemblyLoad += CurrentDomain_AssemblyLoad;
    }

    public delegate void AssemblyLoadedDelegate(string name);
    public event AssemblyLoadedDelegate? AssemblyLoaded;

    bool pastAssembliesLoaded;
    public void EnsureAllAssembliesLoaded()
    {
        if (pastAssembliesLoaded)
            return;
        pastAssembliesLoaded = true;

        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        foreach (var assembly in assemblies) 
        {
            var assemblyName = assembly.GetName().Name;
            InvokeAssemblyLoaded(assemblyName);
        }
    }

    void CurrentDomain_AssemblyLoad(object? sender, AssemblyLoadEventArgs args)
    {
        var assemblyName = args.LoadedAssembly.GetName().Name;
        InvokeAssemblyLoaded(assemblyName);
    }

    object invokeLocker = new();
    List<int> loadedAssembliesHashes = [];
    void InvokeAssemblyLoaded(string? assemblyName)
    {
        if (assemblyName is null)
            return;

        var hash = assemblyName.GetHashCode();
        lock (invokeLocker)
        {
            if (loadedAssembliesHashes.Contains(hash))
                return;

            loadedAssembliesHashes.Add(hash);
            AssemblyLoaded?.Invoke(assemblyName);
        }
    }

    bool disposed;
    public void Dispose()
    {
        if (disposed)
            return;
        disposed = true;

        AppDomain.CurrentDomain.AssemblyLoad -= CurrentDomain_AssemblyLoad;
    }

    ~AssemblyWatcher() => Dispose();
}