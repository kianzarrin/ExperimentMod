namespace ExperimentMod
{
    using System;
    using ICities;
    using KianCommons;
    using System.Linq;
    using KianCommons.IImplict;

    public class UserMod : IUserMod, IMod
    {
        public static Version ModVersion => typeof(UserMod).Assembly.GetName().Version;
        public static string VersionString => ModVersion.ToString(2);
        public string Name => "Experiment Mod " + VersionString;
        public string Description => "test GC";

        public void OnDisabled()
        {
        }

        public void OnEnabled()
        {
            Log.Debug(Environment.StackTrace);
        }
    }
}
