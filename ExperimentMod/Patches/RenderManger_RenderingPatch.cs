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
    public static class TimeEndRenderingPatch {
        static MethodInfo mWrapper = typeof(TimeEndRenderingPatch).GetMethod("Wrapper", true);
        static MethodInfo mEndRendering = typeof(IRenderableManager).GetMethod("EndRendering", true);

        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, MethodBase origin) {
            foreach(var code in instructions) {
                if(code.Calls(mEndRendering)) {
                    Log.Debug($"replacing {code.operand} with {mWrapper}");
                    yield return new CodeInstruction(OpCodes.Call, mWrapper);
                } else {
                    yield return code;
                }
            }
        }

        public static Dictionary<IRenderableManager, Stopwatch> Timers = new Dictionary<IRenderableManager, Stopwatch>();
        static Stopwatch GetOrCreateTimer(IRenderableManager man) {
            try {
                if(!Timers.TryGetValue(man, out var ret)) {
                    ret = Timers[man] = new Stopwatch();
                }
                return ret;
            } catch(Exception ex) {
                ex.Log(false);
                return new Stopwatch();
            }
        } 

        public static void Wrapper(IRenderableManager man, RenderManager.CameraInfo cameraInfo) {
            var timer = GetOrCreateTimer(man);
            timer.Start();
            try {
                man.EndRendering(cameraInfo);
            } finally {
                timer.Stop();
            }
        }
    }

    [HarmonyPatch(typeof(RenderManager), "FpsBoosterLateUpdate")]
    public static class TimeBeginRenderingPatch {
        static MethodInfo mWrapper = typeof(TimeBeginRenderingPatch).GetMethod("Wrapper", true);
        static MethodInfo mBeginRendering = typeof(IRenderableManager).GetMethod("BeginRendering", true);

        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, MethodBase origin) {
            foreach(var code in instructions) {
                if(code.Calls(mBeginRendering)) {
                    Log.Debug($"replacing {code.operand} with {mWrapper}");
                    yield return new CodeInstruction(OpCodes.Call, mWrapper);
                } else {
                    yield return code;
                }
            }
        }

        public static Dictionary<IRenderableManager, Stopwatch> Timers = new Dictionary<IRenderableManager, Stopwatch>();
        static Stopwatch GetOrCreateTimer(IRenderableManager man) {
            try {
                if(!Timers.TryGetValue(man, out var ret)) {
                    ret = Timers[man] = new Stopwatch();
                }
                return ret;
            } catch(Exception ex) {
                ex.Log(false);
                return new Stopwatch();
            }
        }

        public static void Wrapper(IRenderableManager man, RenderManager.CameraInfo cameraInfo) {
            var timer = GetOrCreateTimer(man);
            timer.Start();
            try {
                man.BeginRendering(cameraInfo);
            } finally {
                timer.Stop();
            }
        }
    }

    [HarmonyPatch(typeof(LightSystem), "EndRendering")]
    public static class LightSystemEndRenderingPatch {
        public static Stopwatch Timer = new Stopwatch();
        static void Prefix() => Timer.Start();
        static void Finalizer() => Timer.Stop();
    }
}
