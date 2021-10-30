namespace ExperimentMod.Patches {
    using HarmonyLib;
    using KianCommons.Patches;
    using KianCommons;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection.Emit;
    using System.Reflection;
    using System;
    using System.Collections;

    [HarmonyPatch(typeof(RenderManager), "FpsBoosterLateUpdate")]
    public static class RenderMangerPatch {
        static MethodInfo mEndRenderingWrapper = typeof(RenderMangerPatch).GetMethod("EndRenderingWrapper", true);
        static MethodInfo mEndRendering = typeof(IRenderableManager).GetMethod("EndRendering", true);

        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, MethodBase origin) {
            foreach(var code in instructions) {
                if(code.Calls(mEndRendering)) {
                    Log.Debug("replacing" + code);
                    yield return new CodeInstruction(OpCodes.Call, mEndRenderingWrapper);
                } else {
                    yield return code;
                }
            }
        }

        public static Hashtable Timers;
        static Stopwatch GetOrCreateTimer(IRenderableManager man) {
            try {
                Timers ??= new Hashtable(100);
                return (Timers[man] ??= new Stopwatch()) as Stopwatch;
            } catch(Exception ex) {
                ex.Log(false);
                return new Stopwatch();
            }
        } 

        public static void EndRenderingWrapper(IRenderableManager man, RenderManager.CameraInfo cameraInfo) {
            var timer = GetOrCreateTimer(man);
            timer.Start();
            try {
                man.EndRendering(cameraInfo);
            } finally {
                timer.Stop();
            }
        }
    }
}
