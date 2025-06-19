using Korn.Modules.WinApi;
using System;

namespace Korn.Plugins.Core
{
    public static class EnvUtils
    {
        public static bool HasModule(string name) => Kernel32.GetModuleHandle(name) != IntPtr.Zero;
    }
}