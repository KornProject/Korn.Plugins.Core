using Korn.Hooking;
using Korn.Shared;
using System;
using System.Collections.Generic;

namespace Korn.Plugins.Core.Interfaces
{
    public interface IHookImplemention : IDisposable
    {
        List<MethodHook> Hooks { get; }
    }

    public static class IHookImplementionExtensions
    {
        public static IHookImplemention DisposeHooks(this IHookImplemention self)
        {
            foreach (var hook in self.Hooks)
                hook.RemoveAllEntries();

            return self;
        } 

        public static IHookImplemention AddHook(this IHookImplemention self, MethodInfoSummary forMethod, string methodName)
        {
            var method = self.GetType().GetMethodEx(methodName);
            if (method == null)
                throw new KornError(
                    $"Korn.Plugins.Core.Interfaces.IHookImplementionExtensions->AddHook(this IHookImplemention, MethodInfoSummary, string): ",
                    $"unable find method with name {methodName}."
                );

            return AddHook(self, forMethod, method);
        }

        public static IHookImplemention AddHook(this IHookImplemention self, MethodInfoSummary forMethod, MethodInfoSummary hookEntry)
        {
            var logger = CoreEnv.Logger;
            var name = self.GetType().Name;

            logger.WriteMessage($"[{name}] Creating a hook for {forMethod.Method.Name}…");
            var hook = MethodHook.Create(forMethod);
            hook.AddEntry(hookEntry);

            self.Hooks.Add(hook);
            return self;
        }

        public static IHookImplemention EnableHooks(this IHookImplemention self)
        {
            var logger = CoreEnv.Logger;
            var name = self.GetType().Name;

            logger.WriteMessage($"[{name}] Enabling hooks");
            foreach (var hook in self.Hooks)
                hook.Enable();

            return self;
        }

        public static IHookImplemention DisableHooks(this IHookImplemention self)
        {
            var logger = CoreEnv.Logger;
            var name = self.GetType().Name;

            logger.WriteMessage($"[{name}] Disabling hooks");
            foreach (var hook in self.Hooks)
                hook.Disable();

            return self;
        }
    }
}