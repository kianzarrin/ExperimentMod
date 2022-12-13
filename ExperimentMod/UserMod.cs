namespace ExperimentMod {
    using ColossalFramework;
    using ColossalFramework.IO;
    using ICities;
    using KianCommons;
    using KianCommons.IImplict;
    using System;
    using UnityEngine;

    public class KianUserMod : IUserMod, ILoadingExtension, IMod, IModWithSettings {
        static KianUserMod() => Log.Stack();
        public KianUserMod() => Log.Stack();
        public string Name => "LifeCycle Log ";
        public string Description => "log order/thread/stack-trace of lifecycle evens while loading game";
        const string HARMONY_ID = "Kian.ExperimentMod";

        public void OnEnabled() {
            Log.Buffered = false;
            Log.VERBOSE = false;
            Log.Stack();

            LoadingManager.instance.m_introLoaded += Instance_m_introLoaded;
            LoadingManager.instance.m_levelPreLoaded += Instance_m_levelPreLoaded;
            LoadingManager.instance.m_levelPreUnloaded += Instance_m_levelPreUnloaded;
            LoadingManager.instance.m_levelUnloaded += Instance_m_levelUnloaded;
            LoadingManager.instance.m_metaDataReady += Instance_m_metaDataReady;
            LoadingManager.instance.m_simulationDataReady += Instance_m_simulationDataReady;
            KianManager.Ensure();

            //HarmonyHelper.DoOnHarmonyReady(() => HarmonyUtil.InstallHarmony(HARMONY_ID));
        }
        public void OnDisabled() {
            Log.Stack();
            //HarmonyUtil.UninstallHarmony(HARMONY_ID);
        }
        public void OnSettingsUI(UIHelper helper) => Log.Stack();

        #region events
        private void Instance_m_simulationDataReady() => Log.Stack();
        private void Instance_m_metaDataReady() => Log.Stack();
        private void Instance_m_levelUnloaded() => Log.Stack();
        private void Instance_m_levelPreUnloaded() => Log.Stack();
        private void Instance_m_levelPreLoaded() => Log.Stack();
        private void Instance_m_introLoaded() => Log.Stack();
        #endregion

        #region LoadingEtension
        public void OnCreated(ILoading loading) => Log.Stack();
        public void OnReleased() => Log.Stack();
        public void OnLevelLoaded(LoadMode mode) => Log.Stack();
        public void OnLevelUnloading() => Log.Stack();
        #endregion
    }

    public class KianSerializableDataExtension : ISerializableDataExtension {
        public void OnCreated(ISerializableData serializedData) => Log.Stack();
        public void OnReleased() => Log.Stack();
        public void OnLoadData() => Log.Stack();
        public void OnSaveData() => Log.Stack();
    }

    public class KianManager : Singleton<KianManager>, ISimulationManager, IRenderableManager {
        static void RegisterManager(object manager) {
            try {
                ReflectionHelpers.InvokeMethod<SimulationManager>("RegisterManager", manager);
            } catch(Exception ex) {
                ex.Log();
            }
        }

        void Awake() {
            Log.Stack();
            RegisterManager(this);
        }

        public string GetName() => "KianManager";
        public void SimulationStep(int subStep) => Log.Stack();
        public void GetData(FastList<IDataContainer> data) => Log.Stack();
        public void EarlyUpdateData() => Log.Stack();
        public void UpdateData(SimulationManager.UpdateMode mode) => Log.Stack();
        public void LateUpdateData(SimulationManager.UpdateMode mode) => Log.Stack();
        public void InitRenderData() => Log.Stack();
        public void CheckReferences() => Log.Stack();
        public DrawCallData GetDrawCallData() => default;
        public void BeginRendering(RenderManager.CameraInfo cameraInfo) { }
        public void EndRendering(RenderManager.CameraInfo cameraInfo) { }
        public void BeginOverlay(RenderManager.CameraInfo cameraInfo) { }
        public void EndOverlay(RenderManager.CameraInfo cameraInfo) { }
        public void UndergroundOverlay(RenderManager.CameraInfo cameraInfo) { }
        public bool CalculateGroupData(int groupX, int groupZ, int layer, ref int vertexCount, ref int triangleCount, ref int objectCount, ref RenderGroup.VertexArrays vertexArrays) => true;
        public void PopulateGroupData(int groupX, int groupZ, int layer, ref int vertexIndex, ref int triangleIndex, Vector3 groupPosition, RenderGroup.MeshData data, ref Vector3 min, ref Vector3 max, ref float maxRenderDistance, ref float maxInstanceDistance, ref bool requireSurfaceMaps) { }
        public ThreadProfiler GetSimulationProfiler() => default;
    }
}
