namespace ExperimentMod {
    using ColossalFramework.Math;
    using UnityEngine;

    public class VehicleDebugger : PathDebugger {
        public static VehicleDebugger Instance;
        protected override void SimulationStep(ushort id) {
            ref var vehicle = ref id.ToVehicle();
            TargetPosFrames[vehicle.m_lastFrame] = vehicle.m_targetPos3;
        }

        protected override void RenderOverlay(RenderManager.CameraInfo cameraInfo, ushort id) {
            ref var vehicle = ref id.ToVehicle();
            if (showFrame) {
                uint targetFrame = GetTargetFrame();
                for (int i = 0; i < 4; ++i) {
                    uint targetF = (uint)(targetFrame - (16 * i));
                    RenderFrame(cameraInfo, vehicle.GetFrameData(targetF), new Color(0, 0.25f * (4 - i), 0.1f));
                }
            }
        }

        void RenderFrame(RenderManager.CameraInfo cameraInfo, Vehicle.Frame frame, Color color, float xscale = 1) {
            color.a = alpha;
            Vector3 halfSize = GetID().ToVehicle().Info.m_generatedInfo.m_size * 0.5f;
            Matrix4x4 matrix4X4_P = new();
            matrix4X4_P.SetTRS(frame.m_position, frame.m_rotation, Vector3.one);
            Quad3 quadP = new Quad3 {
                a = matrix4X4_P.MultiplyPoint(new Vector3(-halfSize.x * xscale, 0f, -halfSize.z)),
                b = matrix4X4_P.MultiplyPoint(new Vector3(halfSize.x * xscale, 0f, -halfSize.z)),
                c = matrix4X4_P.MultiplyPoint(new Vector3(halfSize.x * xscale, 0f, +halfSize.z)),
                d = matrix4X4_P.MultiplyPoint(new Vector3(-halfSize.x * xscale, 0f, halfSize.z)),
            };
            RenderManager.instance.OverlayEffect.DrawQuad(
                cameraInfo,
                color,
                quadP,
                frame.m_position.y - 0.1f,
                frame.m_position.y + 0.1f,
                renderLimits: true,
                alphaBlend: alphaBlend);
        }

        protected override void Awake() {
            base.Awake();
            Instance = this;
        }

        protected override uint GetTargetFrame() {
            uint i = (uint)(((int)GetID() << 4) / 65536);
            return SimulationManager.instance.m_referenceFrameIndex - i;
        }

        protected override Vector3 GetSmoothLookPos() {
            uint targetFrame = GetTargetFrame();
            Vector4 pos1 = GetTargetPosFrame(targetFrame - 32U);
            Vector4 pos2 = GetTargetPosFrame(targetFrame - 16U);
            float t = ((targetFrame & 15U) + SimulationManager.instance.m_referenceTimer) * 0.0625f;
            return Vector3.Lerp(pos1, pos2, t);
        }

        protected override void GetSmoothPosition(out Vector3 pos, out Quaternion rot) {
            ushort id = GetID();
            id.ToVehicle().GetSmoothPosition(id, out pos, out rot);
        }

        protected override bool GetID(out ushort id) {
            id = 0;
            InstanceID selectedInstance = InstanceManager.instance.GetSelectedInstance();
            if (selectedInstance.Type == InstanceType.Vehicle) {
                id = selectedInstance.Vehicle;
            }
            return id != 0;
        }
    }
}
