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
        public static SavedBool ShowLookTarget = new SavedBool("ShowLookTarget", FileName, true);
        public static SavedBool ShowTargetPos = new SavedBool("ShowTargetPos", FileName, false);
        public static SavedBool ShowFrames = new SavedBool("ShowFrames", FileName, false);
        public static SavedInt TargetPos = new SavedInt("TargetPos", FileName, 3);
        static UISlider Slider;
        public static void OnSettingsUI(UIHelper helper) {
            helper.AddSavedToggle("render look targets overlay", ShowLookTarget);
            helper.AddSavedToggle("render target position[s] overlay", ShowTargetPos);
            helper.AddSavedToggle("render frames overlay", ShowFrames);
            Slider = helper.AddSlider(
                "choose targetPos to use:",
                min:0, max:3, step:1,
                defaultValue: TargetPos.value,
                delegate(float val) {
                    TargetPos.value = (int)val;
                    Slider.tooltip = $"m_targetPos{val}";
                    Slider.RefreshTooltip();
                }) as UISlider;
        }
    }
}
