namespace VehicleDebugger {
    using System;
    using JetBrains.Annotations;
    using ICities;
    using CitiesHarmony.API;
    using KianCommons;
    using System.Diagnostics;
    using UnityEngine;
    using KianCommons.IImplict;

    public class UserMod : LoadingExtensionBase, IUserMod , IModWithSettings {
        public static Version ModVersion => typeof(UserMod).Assembly.GetName().Version;
        public static string VersionString => ModVersion.ToString(2);
        public string Name => "Experiment Mod " + VersionString;
        public string Description => "human/vehicle path debugger";
        const string HARMONY_ID = "Kian.ExperimentMod";

        [UsedImplicitly]
        public void OnEnabled()
        {
            Log.Buffered = false;
            Log.VERBOSE = false;

            Log.Debug("Testing StackTrace:\n" + new StackTrace(true).ToString(), copyToGameLog: false);
            
            HarmonyHelper.DoOnHarmonyReady(() => HarmonyUtil.InstallHarmony(HARMONY_ID));

            if (HelpersExtensions.InGame) {
                Install();
            }
        }

        [UsedImplicitly]
        public void OnDisabled()
        {
            if (HelpersExtensions.InGame) {
                UnInstall();
            }
            HarmonyUtil.UninstallHarmony(HARMONY_ID);
        }

        public override void OnLevelLoaded(LoadMode mode) {
            Install();
        }
        public override void OnLevelUnloading() {
            UnInstall();
        }

        public static void Install() {
            Log.Called();
            UnityUtil.CreateComponent<HumanDebugger>(false);
            UnityUtil.CreateComponent<VehicleDebugger>(false);
        }

        public static void UnInstall() {
            Log.Called();
            GameObject.Destroy(HumanDebugger.Instance?.gameObject);
            GameObject.Destroy(VehicleDebugger.Instance?.gameObject);
        }


        public void OnSettingsUI(UIHelper helper) => ModSettings.OnSettingsUI(helper);
    }
}
