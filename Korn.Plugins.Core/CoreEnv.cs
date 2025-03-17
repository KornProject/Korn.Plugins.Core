using System.Diagnostics;

namespace Korn.Plugins.Core
{
    public static class CoreEnv
    {
        public static KornLogger Logger;
        public static Process CurrentProcess;
        public static PluginLoader PluginLoader;
    }
}