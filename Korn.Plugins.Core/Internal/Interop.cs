using System;
using System.Runtime.InteropServices;

static class Interop
{
    const string kernel = "kernel32";

    [DllImport(kernel)] public static extern
        IntPtr GetModuleHandle([MarshalAs(UnmanagedType.LPWStr)] string moduleName);
}