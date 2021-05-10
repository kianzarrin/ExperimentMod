namespace ExperimentMod {
    using System;
    using JetBrains.Annotations;
    using ICities;
    using KianCommons;

    public class UserMod : IUserMod {
        static UserMod() {
            Log.Debug("ExperimentMod.UserMod static constructor called!");
        }

        public static Version ModVersion => typeof(UserMod).Assembly.GetName().Version;
        public static string VersionString => ModVersion.ToString(2);
        public string Name => "Experiment Mod " + VersionString;
        public string Description => "Experiment";

        [UsedImplicitly]
        public void OnEnabled()
        {
            Log.Buffered = false;
            Log.Debug("Experimenting mod enabled" + Environment.StackTrace);
        }

        [UsedImplicitly]
        public void OnDisabled()
        {
            Log.Debug("Experimenting mod disabled" + Environment.StackTrace);
        }

        //[UsedImplicitly]
        //public void OnSettingsUI(UIHelperBase helper)
        //{
        //    GUI.Settings.OnSettingsUI(helper);
        //}
    }
}
