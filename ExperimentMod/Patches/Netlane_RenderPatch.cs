namespace ExperimentMod.Patches {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Diagnostics;
    using HarmonyLib;
    [HarmonyPatch(typeof(NetLane), nameof(NetLane.RenderInstance))]
    public static class Netlane_RenderPatch {
        public static Stopwatch Timer;

        static void Prefix() {
            Timer ??= new Stopwatch();
            Timer.Start();
        }

        static void Finalizer() {
            Timer.Stop();
        }
    }
}
