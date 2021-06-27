namespace ExperimentMod {
    using CitiesHarmony.API;
    using ICities;
    using JetBrains.Annotations;
    using KianCommons;
    using System;
    using System.Diagnostics;

    public class UserMod : IUserMod {
        public static Version ModVersion => typeof(UserMod).Assembly.GetName().Version;
        public static string VersionString => ModVersion.ToString(2);
        public string Name => "Experiment Mod " + VersionString;
        public string Description => "control Road/junction transitions";
        const string HARMONY_ID = "Kian.ExperimentMod";

        [UsedImplicitly]
        public void OnEnabled() {
            Log.Buffered = false;
            Log.VERBOSE = false;

            Log.Debug("Testing StackTrace:\n" + new StackTrace(true).ToString(), copyToGameLog: false);

            HarmonyHelper.DoOnHarmonyReady(() => HarmonyUtil.InstallHarmony(HARMONY_ID));

            if (!Helpers.InStartupMenu) {
                Log.Debug("P1");
                MainPanel.Create();
            }

        }

        [UsedImplicitly]
        public void OnDisabled() {
            HarmonyUtil.UninstallHarmony(HARMONY_ID);
            if (!Helpers.InStartupMenu) {
                MainPanel.Release();
            }
        }

        //[UsedImplicitly]
        //public void OnSettingsUI(UIHelperBase helper)
        //{
        //    GUI.Settings.OnSettingsUI(helper);
        //}

    }

    public class LoadingExtension : LoadingExtensionBase {
        public override void OnLevelLoaded(LoadMode mode) {
            MainPanel.Create();
        }
        public override void OnLevelUnloading() {
            MainPanel.Release();
        }

    }
}
