namespace ExperimentMod {
    using ICities;
    using System;
    using System.Diagnostics;
    using KianCommons;
    using CitiesHarmony.API;
    public class UserMod : IUserMod {
        public static Version ModVersion => typeof(UserMod).Assembly.GetName().Version;
        public static string VersionString => ModVersion.ToString(2);
        public string Name => "Experiment Mod " + VersionString;
        public string Description => "test serialization ";

        const string HARMONY_ID = "kian.zarrin.exp";
        public void OnEnabled() {
            Log.Called();
            HarmonyHelper.DoOnHarmonyReady(delegate () {
                HarmonyUtil.InstallHarmony(HARMONY_ID);
            });
            try { 
                Test.Run();
            } catch(Exception ex) { ex.Log(); }
            //Process.GetCurrentProcess().Kill();

        }

        public void OnDisabled() {
            HarmonyUtil.UninstallHarmony(HARMONY_ID);
        }

    }

}
