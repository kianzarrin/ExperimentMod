namespace ExperimentMod.Patches {
    using HarmonyLib;
    using KianCommons.Patches;
    using KianCommons;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection.Emit;
    using System.Reflection;
    using System;

    [HarmonyPatch(typeof(RenderManager), "FpsBoosterLateUpdate")]
    public static class RenderMangerPatch {
        static MethodInfo mi = typeof(RenderMangerPatch).GetMethod("EndRenderingWrapper", true);

        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, MethodBase origin) {
            foreach(var code in instructions) {
                if(code.Calls("EndRendering")) {
                    yield return new CodeInstruction(OpCodes.Call, mi);
                } else {
                    yield return code;
                }
            }
        }

        public static Dictionary<IRenderableManager, Stopwatch> timer_dict_;
        static Stopwatch GetOrCreateTimer(IRenderableManager man) {
            try {
                timer_dict_ ??= new Dictionary<IRenderableManager, Stopwatch>();
                if(timer_dict_.TryGetValue(man, out var ret))
                    return ret;
                ret = new Stopwatch();
                timer_dict_.Add(man, ret);
                return ret;
            }catch(Exception ex) { ex.Log(false); }
            return new Stopwatch();
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
