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
            //KornShared.Logger.Message(hook.ToString());
            logger.WriteMessage($"[{name}] Created hook for {forMethod.Method.Name}");
            hook.AddEntry(hookEntry);
            logger.WriteMessage($"[{name}] Added hook entry {hookEntry.Method.Name} for {forMethod.Method.Name}");

            self.Hooks.Add(hook);
            return self;
        }

        public static IHookImplemention EnableHooks(this IHookImplemention self)
        {
            var logger = CoreEnv.Logger;
            var name = self.GetType().Name;

            logger.WriteMessage($"[{name}] Hooks enabling…");
            foreach (var hook in self.Hooks)
                hook.Enable();
            logger.WriteMessage($"[{name}] Hooks enabled");

            return self;
        }

        public static IHookImplemention DisableHooks(this IHookImplemention self)
        {
            var logger = CoreEnv.Logger;
            var name = self.GetType().Name;

            logger.WriteMessage($"[{name}] Hooks disabling…");
            foreach (var hook in self.Hooks)
                hook.Disable();
            logger.WriteMessage($"[{name}] Hooks disabled");

            return self;
        }
    }
}