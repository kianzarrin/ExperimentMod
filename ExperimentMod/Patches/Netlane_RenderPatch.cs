namespace ExperimentMod.Patches {
    using HarmonyLib;
    using System.Diagnostics;
    [HarmonyPatch(typeof(NetLane), nameof(NetLane.RenderInstance))]
    public static class Netlane_RenderPatch {
        public static Stopwatch Timer = new Stopwatch();
        static void Prefix() => Timer.Start();
        static void Finalizer() => Timer.Stop();

    }
}
