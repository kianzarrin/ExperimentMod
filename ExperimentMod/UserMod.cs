namespace ExperimentMod {
    using System;
    using ICities;
    using KianCommons;
    using KianCommons.IImplict;

    public class UserMod : IUserMod, IMod {
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

        public void OnEnabled()
        {
            Log.Called();
            Log.Debug(Environment.StackTrace);
        }

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
