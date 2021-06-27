namespace ExperimentMod {
    using ICities;
    using System;
    using System.Diagnostics;
    using KianCommons;

    public class UserMod : IUserMod {
        public static Version ModVersion => typeof(UserMod).Assembly.GetName().Version;
        public static string VersionString => ModVersion.ToString(2);
        public string Name => "Experiment Mod " + VersionString;
        public string Description => "test serialization ";

        public void OnEnabled() {
            try {
                Test.Run();
            }catch(Exception ex) { ex.Log(); }
            Process.GetCurrentProcess().Kill();
        }

    }

}
