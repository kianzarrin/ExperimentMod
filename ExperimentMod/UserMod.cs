namespace ExperimentMod {
    using System;
    using JetBrains.Annotations;
    using ICities;
    using CitiesHarmony.API;
    using KianCommons;
    using System.Diagnostics;
    using UnityEngine;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    public class UserMod : IUserMod {
        //public static string testvar = HarmonyLib.AccessTools.Method("ExperimentMod.UserMod.OnEnabled").Name;

        static UserMod() {
            Log.Debug("ExperimentMod.UserMod static constructor called!" + Environment.StackTrace);
        }

        public static Version ModVersion => typeof(UserMod).Assembly.GetName().Version;
        public static string VersionString => ModVersion.ToString(2);
        public string Name => "Experimenting Mod " + VersionString;
        public string Description => "control Road/junction transitions";
        const string HARMONY_ID = "Kian.ExperimentMod";

        public static void Logpdbtools()
        {
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                var name = asm.Name();
                AppDomain.CurrentDomain.GetAssemblies();
                if (name == "pdb2mdb" || name.ToLower().Contains("mono"))
                {

                }
            }
        }


        [UsedImplicitly]
        public void OnEnabled()
        {
            Log.Buffered = false;

            string pdbtools = @"C:\Users\dell\AppData\Local\Colossal Order\Cities_Skylines\Addons\Mods\_pdbtools";
            var a = Path.Combine(pdbtools, "Mono.Cecil.dll");
            var b = Path.Combine(pdbtools, "Mono.CompilerServices.SymbolWriter.dll");
            var pdb2mdb = Path.Combine(pdbtools, "pdb2mdb.exe");
            
            Logpdbtools();
            A






            //try
            //{
            //    Log.Debug("Testing StackTrace:\n" + new StackTrace(true).ToString());
            //    Log.Debug("Testing StackTrace:\n" + Environment.StackTrace);
            //    throw new Exception("some exception");
            //}
            //catch (Exception ex)
            //{
            //    Log.Exception(ex);
            //    throw ex;
            //}



            HelpersExtensions.VERBOSE = false;
            HarmonyHelper.DoOnHarmonyReady(() => HarmonyUtil.InstallHarmony(HARMONY_ID));
            Application.runInBackground = true;



            if (HelpersExtensions.InGame) {
                Application.runInBackground = true;
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
