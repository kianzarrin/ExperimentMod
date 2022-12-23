namespace ExperimentMod {
    using System;
    using JetBrains.Annotations;
    using ICities;
    using CitiesHarmony.API;
    using KianCommons;
    using System.Diagnostics;

    public class UserMod : IUserMod {
        public string Name => "Throw exceptions";
        public string Description => "control Road/junction transitions";
        const string HARMONY_ID = "Kian.ExperimentMod";

        [UsedImplicitly]
        public void OnEnabled()
        {
            Log.Buffered = false;
            Log.VERBOSE = false;

            Log.Debug("Testing StackTrace:\n" + new StackTrace(true).ToString(), copyToGameLog: false);
            
            HarmonyHelper.DoOnHarmonyReady(() => HarmonyUtil.InstallHarmony(HARMONY_ID));

            if (HelpersExtensions.InGame) {

            }
        }

        [UsedImplicitly]
        public void OnDisabled()
        {
            HarmonyUtil.UninstallHarmony(HARMONY_ID);
        }

        [UsedImplicitly]
        public void OnSettingsUI(UIHelperBase helper)
        {
            
        }

    }
}
