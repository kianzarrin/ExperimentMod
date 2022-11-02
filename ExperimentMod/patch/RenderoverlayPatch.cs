namespace VehicleDebugger.Patch {
    using HarmonyLib;
    using JetBrains.Annotations;

    [HarmonyPatch(typeof(DefaultTool), "RenderOverlay")]
    [UsedImplicitly]
    public static class RenderOverlayPatch {
        /// <summary>
        /// Renders mass edit overlay even when traffic manager is not current tool.
        /// </summary>
        [HarmonyPostfix]
        [UsedImplicitly]
        public static void Postfix(RenderManager.CameraInfo cameraInfo) {
            if (ToolsModifierControl.toolController.CurrentTool == ToolsModifierControl.GetTool<DefaultTool>()) {
                PathDebugger.RenderOverlayALL(cameraInfo);
            }
        }
    }
}