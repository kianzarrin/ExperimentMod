namespace ExperimentMod {
    using System;
    using JetBrains.Annotations;
    using ICities;
    using CitiesHarmony.API;
    using KianCommons;
    using System.Diagnostics;

    public class UserMod : IUserMod {
        static UserMod() {
            Log.Debug("ExperimentMod.UserMod static constructor called!" + Environment.StackTrace);
        }

        public static Version ModVersion => typeof(UserMod).Assembly.GetName().Version;
        public static string VersionString => ModVersion.ToString(2);
        public string Name => "Experiment Mod " + VersionString;
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
                for (ushort nodeID =1; nodeID < NetManager.MAX_NODE_COUNT; ++nodeID) {
                    if(nodeID.ToNode().m_flags.CheckFlags(NetNode.Flags.Created | NetNode.Flags.Transition, NetNode.Flags.Deleted))
                        NetManager.instance.UpdateNode(nodeID);
                }
            }
        }

        [UsedImplicitly]
        public void OnDisabled()
        {
            HarmonyUtil.UninstallHarmony(HARMONY_ID);
        }

        //[UsedImplicitly]
        //public void OnSettingsUI(UIHelperBase helper)
        //{
        //    GUI.Settings.OnSettingsUI(helper);
        //}

    }
}
