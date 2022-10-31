namespace ExperimentMod {
    using UnityEngine;

    public class HumanDebugger : PathDebugger {
        public static HumanDebugger Instance;

        Vector4[] TargetPosHistory = new Vector4[4];
        protected override void SimulationFrame(ushort id) {
            ref var human = ref id.ToCitizenInstance();
            var pos = human.m_targetPos;
            TargetPosHistory[human.m_lastFrame] = pos;

            static uint Dec(uint b) => b == 0 ? 3u : b - 1u;
            uint frame1 = Dec(human.m_lastFrame);
            uint frame2 = Dec(frame1);
            uint frame3 = Dec(frame2);
            var pos1 = TargetPosHistory[frame1];
            var pos2 = TargetPosHistory[frame2];
            var pos3 = TargetPosHistory[frame3];

            const float minDistance = .1f;
            static bool IsGoodDistance(Vector3 v1, Vector3 v2) => (v1 - v2).sqrMagnitude > minDistance * minDistance;

            // smooth out.
            if (IsGoodDistance(pos, pos1)) {
                TargetLookPosFrames[human.m_lastFrame] = pos1;
            } else if (IsGoodDistance(pos, pos2)) {
                TargetLookPosFrames[human.m_lastFrame] = Vector3.LerpUnclamped(pos2, pos, .5f);
            } else if(IsGoodDistance(pos, pos3)) {
                TargetLookPosFrames[human.m_lastFrame] = Vector3.LerpUnclamped(pos3, pos, .75f);
            } else {
                TargetLookPosFrames[human.m_lastFrame] = pos;
            }

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
            if (showTargets) {
                RenderCircle(cameraInfo, human.m_targetPos, new Color(0, 1, 0.1f), 1);
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

        protected override void GetPathInfo(out uint pathUnitID, out byte lastOffset, out byte finePathPositionIndex, out Vector3 refPos) {
            ref CitizenInstance citizenInstance = ref GetID().ToCitizenInstance();
            pathUnitID = citizenInstance.m_path;
            lastOffset = citizenInstance.m_lastPathOffset;
            finePathPositionIndex = citizenInstance.m_pathPositionIndex;
            refPos = citizenInstance.GetLastFramePosition();
        }

        protected override uint GetTargetFrame() {
            uint i = (uint)(((int)GetID() << 4) / 65536);
            return SimulationManager.instance.m_referenceFrameIndex - i;
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
