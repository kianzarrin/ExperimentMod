namespace ExperimentMod {
    using ColossalFramework;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using KianCommons;
    using KianCommons.UI;
    using ColossalFramework.UI;

    public static class ModSettings {

        public const string FileName = "look ahead experiment";

        static ModSettings() {
            // Creating setting file - from SamsamTS
            if (GameSettings.FindSettingsFileByName(FileName) == null) {
                GameSettings.AddSettingsFile(new SettingsFile[] { new SettingsFile() { fileName = FileName } });
            }
        }

        public static SavedBool RenderOverlay = new SavedBool("RenderOverlay", FileName, true);
        public static SavedBool ShowTargetPos = new SavedBool("ShowTargetPos", FileName, true);
        public static SavedBool ShowFrames = new SavedBool("ShowFrames", FileName, true);
        public static SavedBool ShowSeg = new SavedBool("ShowSeg", FileName, true);

        public static SavedBool ShowLookTarget = new SavedBool("ShowLookTarget", FileName, false);
        public static SavedBool ShowLookArrow = new SavedBool("ShowLookArrow", FileName, false);
        public static SavedInt LookTargetPos = new SavedInt("LookTargetPos", FileName, 3);
        static UISlider Slider;
        public static void OnSettingsUI(UIHelper helper) {
            helper.AddSavedToggle("Render Overlay", RenderOverlay).tooltip = "switch on/off all overlay";
            helper.AddSavedToggle("render target position[s] overlay", ShowTargetPos);
            helper.AddSavedToggle("render frames overlay", ShowFrames);
            helper.AddSavedToggle("render vehicle seg", ShowSeg);

            var g = helper.AddGroup("Look ahead");
            g.AddSavedToggle("render look arrow overlay", ShowLookArrow);
            g.AddSavedToggle("render look targets overlay", ShowLookTarget);
            Slider = g.AddSlider(
                "choose targetPos to use:",
                min:0, max:3, step:1,
                defaultValue: LookTargetPos.value,
                delegate(float val) {
                    LookTargetPos.value = (int)val;
                    Slider.tooltip = $"m_targetPos{val}";
                    Slider.parent.RefreshTooltip();
                }) as UISlider;
        }
    }
}
