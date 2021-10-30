namespace ExperimentMod.Patches.RenderManager_ {
    using HarmonyLib;
    using System.Diagnostics;

    [HarmonyPatch(typeof(RenderManager), "FpsBoosterLateUpdate")]
    public static class LateUdatePatch {
        public static Stopwatch Timer = new Stopwatch();
        static void Prefix() => Timer.Start();
        static void Finalizer() => Timer.Stop();
    }

    [HarmonyPatch(typeof(RenderManager), "Managers_RenderOverlay")]
    public static class RenderOverlayPatch {
        public static Stopwatch Timer = new Stopwatch();
        static void Prefix() => Timer.Start();
        static void Finalizer() => Timer.Stop();
    }

    [HarmonyPatch(typeof(RenderManager), "UpdateCameraInfo")]
    public static class UpdateCameraInfoPatch {
        public static Stopwatch Timer = new Stopwatch();
        static void Prefix() => Timer.Start();
        static void Finalizer() => Timer.Stop();
    }

    [HarmonyPatch(typeof(RenderManager), "UpdateColorMap")]
    public static class UpdateColorMapPatch {
        public static Stopwatch Timer = new Stopwatch();
        static void Prefix() => Timer.Start();
        static void Finalizer() => Timer.Stop();
    }

}
