namespace ExperimentMod {
    using ICities;

    public class ThreadingExtension : ThreadingExtensionBase {
        public override void OnAfterSimulationFrame() {
            base.OnAfterSimulationFrame();
            PathDebugger.SimulationFrameALL();
        }
    }
}
