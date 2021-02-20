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

        public static Assembly LoadDLL(string path)
        {
            try
            {
                Log.Debug("Loading assembly from path " + path);
                var asm = Assembly.Load(File.ReadAllBytes(path));
                if (asm is not null)
                {
                    Log.Debug("Assembly loaded: " + asm);
                    return asm;
                }
            }
            catch (Exception ex){
                Log.Exception(ex, showInPanel:false);
            }
            Log.Error("failed to load assembly " + path);
            return null;
        }


        [UsedImplicitly]
        public void OnEnabled()
        {
            Log.Buffered = false;

            string pdbtools = @"C:\Users\dell\AppData\Local\Colossal Order\Cities_Skylines\Addons\Mods\_pdbtools";
            var a = Path.Combine(pdbtools, "Mono.Cecil.dll");
            var b = Path.Combine(pdbtools, "Mono.CompilerServices.SymbolWriter.dll");
            var c = Path.Combine(pdbtools, "pdb2mdb.exe");
            var d = @"C:\Users\dell\AppData\Local\Colossal Order\Cities_Skylines\Addons\Mods\ExperimentMod\ExperimentingMod.dll";

            Logpdbtools();

            LoadDLL(a);
            LoadDLL(b);
            var pdb2mdb = LoadDLL(c);

            Logpdbtools();

            var converter = pdb2mdb.GetType("Pdb2Mdb.Converter", throwOnError:true);
            var mConvert = converter.GetMethod("Convert");

            File.Delete(d + ".mdb");

            Log.Debug("converting pdb2mdb");
            mConvert.Invoke(null, new object[] { d });

            return;
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
