namespace ExperimentMod.Patches {
    using HarmonyLib;
    using System.Diagnostics;


    [HarmonyPatch(typeof(RenderGroup), "Render", new[] { typeof(RenderManager.CameraInfo) })]
    public static class RenderGroup_RenderPatch1 {
        public static Stopwatch Timer = new Stopwatch();
        static void Prefix() => Timer.Start();
        static void Finalizer() => Timer.Stop();
    }

    [HarmonyPatch(typeof(RenderGroup), "Render", new[] { typeof(int) })]
    public static class RenderGroup_RenderPatch2 {
        public static Stopwatch Timer = new Stopwatch();
        static void Prefix() => Timer.Start();
        static void Finalizer() => Timer.Stop();
    }

    [HarmonyPatch(typeof(MegaRenderGroup), "Render")]
    public static class MegaRenderGroup_RenderPatch {
        public static Stopwatch Timer = new Stopwatch();
        static void Prefix() => Timer.Start();
        static void Finalizer() => Timer.Stop();
    }
}
