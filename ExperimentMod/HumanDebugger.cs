namespace ExperimentMod {
    using UnityEngine;

    public class HumanDebugger : PathDebugger {
        public static HumanDebugger Instance;

        Vector4[] TargetPosHistory = new Vector4[4];
        protected override void SimulationFrame(ushort id) {
            ref var human = ref id.ToCitizenInstance();
            var pos = human.m_targetPos;
            TargetPosHistory[human.m_lastFrame] = pos;

            uint frame0 = human.m_lastFrame == 0 ? 3u : human.m_lastFrame - 1u;
            var pos0 = TargetPosHistory[frame0];
            var distance2 = (pos - pos0).sqrMagnitude;
            const float minDistance = .1f;
            if (distance2 < minDistance * minDistance) {
                TargetLookPosFrames[human.m_lastFrame] = pos;
            } else {
                TargetLookPosFrames[human.m_lastFrame] = Vector3.LerpUnclamped(pos0,pos,0.5f);
            }

            TargetLookPosFrames[human.m_lastFrame] = human.m_targetPos;
        }



        protected override void RenderOverlay(RenderManager.CameraInfo cameraInfo, ushort id) {
            ref var human = ref id.ToCitizenInstance();
            if (showFrame) {
                uint targetFrame = GetTargetFrame();
                for (int i = 0; i < 4; ++i) {
                    uint targetF = (uint)(targetFrame - (16 * i));
                    RenderFrame(cameraInfo, human.GetFrameData(targetF), new Color(0, 0.25f * (4 - i), 0.1f));
                }
            }
        }

        void RenderFrame(RenderManager.CameraInfo cameraInfo, CitizenInstance.Frame frame, Color color) {
            color.a = alpha;
            RenderCircle(cameraInfo, frame.m_position, color, 1);
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
            id.ToCitizenInstance().GetSmoothPosition(id, out pos, out rot);
        }

        protected override bool GetID(out ushort id) {
            id = 0;
            InstanceID selectedInstance = InstanceManager.instance.GetSelectedInstance();
            if (selectedInstance.Type == InstanceType.Citizen) {
                id = selectedInstance.Citizen.ToCitizen().m_instance;
            }
            return id != 0;
        }
    }
}
