using System.Collections.Generic;
using Korn.Hooking;

namespace Korn.Plugins.Core.Interfaces
{
    public interface IHookImplemention
    {
        List<MethodHook> Hooks { get; }
    }

    public static class IHookImplementionExtensions
    {
        public static IHookImplemention AddHook(this IHookImplemention self, MethodInfoSummary forMethod, MethodInfoSummary hookEntry)
        {
            CoreEnv.Logger.WriteMessage($"Creating hook for {self.GetType().Name}.{forMethod.Method.Name}…");
            var hook = MethodHook.Create(forMethod);
            hook.AddEntry(hookEntry);

            self.Hooks.Add(hook);
            return self;
        }

        public static IHookImplemention EnableHooks(this IHookImplemention self)
        {
            CoreEnv.Logger.WriteMessage($"Enabling hooks for {self.GetType().Name}");
            foreach (var hook in self.Hooks)
                hook.Enable();

            return self;
        }

        public static IHookImplemention DisableHooks(this IHookImplemention self)
        {
            CoreEnv.Logger.WriteMessage($"Disabling hooks for {self.GetType().Name}");
            foreach (var hook in self.Hooks)
                hook.Disable();

            return self;
        }
    }
}