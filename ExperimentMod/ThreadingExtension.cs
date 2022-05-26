namespace ExperimentMod {
    using ICities;
    using KianCommons;

    public class ThreadingExtension : ThreadingExtensionBase {
        public override void OnCreated(IThreading threading) {
            Log.Called();
        }
        public override void OnAfterSimulationFrame() {
            PathDebugger.SimulationFrameALL();
        }
    }
}
