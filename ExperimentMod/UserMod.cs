namespace ExperimentMod {
    using System;
    using JetBrains.Annotations;
    using ICities;
    using CitiesHarmony.API;
    using System.Diagnostics;
    using KianCommons;

    public class UserMod : IUserMod {
        static UserMod() {
            Log.Called();
            Log.Debug(Environment.StackTrace);
        }

        public UserMod()
        {
            Log.Called();
            Log.Debug(Environment.StackTrace);
        }

        public static Version ModVersion => typeof(UserMod).Assembly.GetName().Version;
        public static string VersionString => ModVersion.ToString(2);
        public string Name => "Experiment Mod " + VersionString;
        public string Description => "control Road/junction transitions";

        [UsedImplicitly]
        public void OnEnabled()
        {
            Log.Called();
            Log.Debug(Environment.StackTrace);
        }

        [UsedImplicitly]
        public void OnDisabled()
        {
            Log.Called();
            Log.Debug(Environment.StackTrace);
        }

        //[UsedImplicitly]
        //public void OnSettingsUI(UIHelperBase helper)
        //{
        //    GUI.Settings.OnSettingsUI(helper);
        //}

    }
}
