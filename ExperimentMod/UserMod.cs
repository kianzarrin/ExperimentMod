namespace ThrowExceptions {
    using System;
    using JetBrains.Annotations;
    using ICities;
    using CitiesHarmony.API;
    using KianCommons;
    using System.Diagnostics;
    using UnityEngine;

    public class UserMod : IUserMod {
        public string Name => "Throw exceptions";
        public string Description => "control Road/junction transitions";
        const string HARMONY_ID = "Kian.ExperimentMod";

        [UsedImplicitly]
        public void OnEnabled()
        {
            Log.Buffered = false;
            Log.VERBOSE = false;
            Log.Stack();
            if (!LoadingManager.instance.m_currentlyLoading) {
                ThrowExceptionsGO.Ensure();
            } else {
                LoadingManager.instance.m_introLoaded += ThrowExceptionsGO.Ensure;
            }

            //HarmonyHelper.DoOnHarmonyReady(() => HarmonyUtil.InstallHarmony(HARMONY_ID));

            //if (HelpersExtensions.InGame) {

            //}
        }

        [UsedImplicitly]
        public void OnDisabled()
        {
            LoadingManager.instance.m_introLoaded -= ThrowExceptionsGO.Ensure;
            GameObject.Destroy(ThrowExceptionsGO.instance);
            //HarmonyUtil.UninstallHarmony(HARMONY_ID);
        }

        [UsedImplicitly]
        public void OnSettingsUI(UIHelperBase helper)
        {
            
        }

    }
}
