using System.Collections.Generic;
using System;
using System.Reflection;

namespace Korn.Plugins.Core
{
    public class AssemblyWatcher : IDisposable
    {
        public AssemblyWatcher()
        {
            AppDomain.CurrentDomain.AssemblyLoad += CurrentDomain_AssemblyLoad;
        }

        public delegate void AssemblyLoadDelegate(Assembly assembly);
        public event AssemblyLoadDelegate AssemblyLoad;

        bool pastAssembliesLoaded;
        public void EnsureAllAssembliesLoaded()
        {
            if (pastAssembliesLoaded)
                return;
            pastAssembliesLoaded = true;

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
                InvokeAssemblyLoaded(assembly);
        }

        void CurrentDomain_AssemblyLoad(object sender, AssemblyLoadEventArgs args)
            => InvokeAssemblyLoaded(args.LoadedAssembly);

        object invokeLocker = new object();
        List<int> loadedAssembliesHashes = new List<int>();
        void InvokeAssemblyLoaded(Assembly assembly)
        {
            if (assembly == null)
                return;

            var hash = assembly.GetName().Name.GetHashCode();
            lock (invokeLocker)
            {
                if (loadedAssembliesHashes.Contains(hash))
                    return;

                loadedAssembliesHashes.Add(hash);
            }

            AssemblyLoad?.Invoke(assembly);
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
}