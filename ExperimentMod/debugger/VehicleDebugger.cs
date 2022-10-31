namespace ExperimentMod {
    using ColossalFramework;
    using ColossalFramework.Math;
    using UnityEngine;

    public class VehicleDebugger : PathDebugger {
        public static VehicleDebugger Instance;
        int targetPosIndex => ModSettings.LookTargetPos;

        protected override void SimulationFrame(ushort id) {
            ref var vehicle = ref id.ToVehicle();
            TargetLookPosFrames[vehicle.m_lastFrame] = vehicle.GetTargetPos(targetPosIndex);
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
            if (showTargets) {
                for (int i = 0; i < 4; ++i) {
                    Color colorT = new Color32(255, (byte)(50 + 50 * i), (byte)(50 * i), 255);
                    float hw = 1.5f;
                    float r = hw * (.25f + .25f * i);
                    RenderCircle(cameraInfo, vehicle.GetTargetPos(i), colorT, r);
                }
            }
            if (showSeg) {
                float hw = GetID().ToVehicle().Info.m_generatedInfo.m_size.x * 0.5f;
                Segment3 vehSegment = vehicle.m_segment;
                Singleton<RenderManager>.instance.OverlayEffect.DrawSegment(
                cameraInfo,
                new Color(0.85f, 0.01f, 1f),
                vehSegment,
                hw,
                hw * 1.9f,
                vehSegment.Min().y - 0.1f,
                vehSegment.Max().y + 0.1f,
                renderLimits: true,
                alphaBlend: alphaBlend);
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

        protected override void GetPathInfo(out uint pathUnitID, out byte lastOffset, out byte finePathPositionIndex, out Vector3 refPos) {
            ref Vehicle vehicle = ref GetID().ToVehicle();
            pathUnitID = vehicle.m_path;
            lastOffset = vehicle.m_lastPathOffset;
            finePathPositionIndex = vehicle.m_pathPositionIndex;
            refPos = vehicle.GetLastFramePosition();
        }

        protected override uint GetTargetFrame() {
            ushort id = GetID();
            ref Vehicle vehicle = ref id.ToVehicle();
            return vehicle.GetTargetFrame(vehicle.Info, id);
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
