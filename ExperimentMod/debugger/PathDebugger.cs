namespace ExperimentMod {
    using ColossalFramework.Math;
    using KianCommons;
    using System;
    using UnityEngine;
    using static NetInfo;

    public static class Extensions {
        public const float BYTE2FLOAT_OFFSET = 1f / 255;

        internal static Vector3 RotateXZ90CW(this Vector3 v) =>
            new Vector3(v.z, v.y, -v.x);
        internal static Vector3 RotateXZ90CCW(this Vector3 v) =>
            new Vector3(-v.z, v.y, v.x);

        public static ref CitizenInstance ToCitizenInstance(this ushort id) =>
            ref CitizenManager.instance.m_instances.m_buffer[id];
        public static ref Citizen ToCitizen(this uint id) =>
            ref CitizenManager.instance.m_citizens.m_buffer[id];
        public static ref Vehicle ToVehicle(this ushort id) =>
             ref VehicleManager.instance.m_vehicles.m_buffer[id];
        public static ref PathUnit ToPathUnit(this uint id) => ref PathManager.instance.m_pathUnits.m_buffer[id];
        public static ref NetLane GetLane(this ref PathUnit.Position pathPos) => ref PathManager.GetLaneID(pathPos).ToLane();
        public static Vector3 GetPosition(this ref PathUnit.Position pathPos) =>
            pathPos.GetLane().CalculatePositionByte(pathPos.m_offset);

        public static Vector3 CalculatePositionByte(this ref NetLane lane, byte offset) =>
            lane.CalculatePosition(offset * BYTE2FLOAT_OFFSET);


        public static ushort GetNodeID(this ref PathUnit.Position pathPos) => pathPos.m_offset switch {
            255 => pathPos.m_segment.ToSegment().m_startNode,
            0 => pathPos.m_segment.ToSegment().m_startNode,
            _ => 0,
        };
    }

    public abstract class PathDebugger : MonoBehaviour {
        protected const float alpha = .75f;
        protected const bool renderLimits = true;
        protected const bool alphaBlend = true;
        protected static Vector4[] TargetLookPosFrames = new Vector4[4];
        protected virtual bool showFrame => ModSettings.ShowFrames;
        protected virtual bool showTargets => ModSettings.ShowTargetPos;
        protected virtual bool showLookTargets => ModSettings.ShowLookTarget;
        protected virtual bool showArrow => ModSettings.ShowLookArrow;
        protected virtual bool showSeg => ModSettings.ShowSeg;
        protected virtual bool showPathPos => ModSettings.ShowPathPos;

        public static void RenderOverlayALL(RenderManager.CameraInfo cameraInfo) {
            try {
                if (!ModSettings.RenderOverlay) return;
                HumanDebugger.Instance.RenderOverlay(cameraInfo);
                VehicleDebugger.Instance.RenderOverlay(cameraInfo);
            } catch (Exception ex) { ex.Log(false); }

        }
        public static void SimulationFrameALL() {
            try {
                HumanDebugger.Instance.SimulationFrame();
                VehicleDebugger.Instance.SimulationFrame();
            } catch (Exception ex) { ex.Log(false); }
        }

        protected virtual void Awake() {
            Log.Called(GetType());
        }
        protected virtual void Destroy() {
            Log.Called(GetType());
        }
        protected virtual void LateUpdate() { }

        protected virtual void RenderOverlay(RenderManager.CameraInfo cameraInfo) {
            if (GetID(out ushort id)) {
                if (showLookTargets) {
                    uint targetFrame = GetTargetFrame();
                    for (int i = 0; i < 4; i++) {
                        uint targetF = (uint)(targetFrame - (16 * i));
                        Color colorT = new Color32(255, (byte)(50 + 50 * i), (byte)(50 * i), 255);
                        float hw = 1.5f;
                        float r = hw * (.25f + .25f * i);
                        RenderCircle(cameraInfo, GetTargetLookPosFrame(targetF), colorT, r);
                    }
                }
                if(showArrow) {
                    GetSmoothPosition(out var pos0, out var rot0);
                    var lookPos = GetSmoothLookPos();
                    var lookdir = lookPos - pos0;
                    const float minDistance = .3f;
                    if (lookdir.sqrMagnitude > minDistance * minDistance) {
                        RenderArrow(cameraInfo, pos0, lookdir, Color.red);
                    } else {
                        lookdir = rot0 * Vector3.forward;
                        RenderArrow(cameraInfo, pos0, lookdir, new Color(1, 0, 1));
                    }
                }
                if (showPathPos) {
                    RenderPathOverlay(cameraInfo);
                }
                RenderOverlay(cameraInfo, id);
            }
        }

        private void RenderPathOverlay(RenderManager.CameraInfo cameraInfo) {
            try {
                Log.Called();
                GetPathInfo(out uint pathUnitID, out byte lastOffset, out byte finePathPositionIndex, out Vector3 refPos);
                byte pathIndex = (byte)(finePathPositionIndex >> 1);
                if (!RollAndGetPathPos(ref pathUnitID, pathIndex, out var pathPos)) return;
                if (lastOffset != 255) {
                    Vector3 lastPos = pathPos.GetLane().CalculatePositionByte(lastOffset);
                    RenderCircle(cameraInfo, lastPos, Color.red, 1);
                    RenderCircle(cameraInfo, refPos, Color.blue, 1);
                    GetSmoothPosition(out var pos, out _);
                    RenderCircle(cameraInfo, pos, Color.white, 1);
                    //RenderArrow(cameraInfo, lastPos, pathPos.GetPosition() - lastPos, Color.blue);
                }

                for (int i = 0; i < 10; ++i) {
                    if (!RollAndGetPathPos(ref pathUnitID, pathIndex, out pathPos)) return;
                    RenderCircle(cameraInfo, pathPos.GetPosition(), Color.magenta, 2);
                    pathIndex++;
                }
            } catch(Exception ex) { ex.Log(); }

            static bool RollAndGetPathPos(ref uint pathUnitID, byte pathIndex, out PathUnit.Position pathPos) {
                if(pathIndex >= 12) {
                    pathIndex = 0;
                    pathUnitID = pathUnitID.ToPathUnit().m_nextPathUnit;
                }
                if (pathUnitID == 0 || pathIndex >= pathUnitID.ToPathUnit().m_positionCount) {
                    pathPos = default;
                    return false;
                }

                pathPos = pathUnitID.ToPathUnit().GetPosition(pathIndex);
                if(pathPos.m_segment == 0) return false;
                return true;
            }
        }

        protected abstract void RenderOverlay(RenderManager.CameraInfo cameraInfo, ushort id);

        protected void SimulationFrame() {
            if (GetID(out ushort id)) {
                SimulationFrame(id);
            }
        }

        protected abstract void SimulationFrame(ushort id);
        protected abstract uint GetTargetFrame();
        protected virtual Vector3 GetSmoothLookPos() {
            uint targetFrame = GetTargetFrame();
            Vector4 pos1 = GetTargetLookPosFrame(targetFrame - 16U);
            Vector4 pos2 = GetTargetLookPosFrame(targetFrame - 0);
            float t = ((targetFrame & 15U) + SimulationManager.instance.m_referenceTimer) * 0.0625f;
            return Vector3.Lerp(pos1, pos2, t);
        }

        protected abstract void GetPathInfo(out uint pathUnitID, out byte lastOffset, out byte finePathPositionIndex, out Vector3 refPos);

        protected abstract void GetSmoothPosition(out Vector3 pos, out Quaternion rot);
        protected virtual InstanceID SelectedInstance => InstanceManager.instance.GetSelectedInstance();
        protected abstract bool GetID(out ushort id);
        protected ushort GetID() {
            GetID(out ushort id);
            return id;
        }

        protected Vector4 GetTargetLookPosFrame(uint simulationFrame) {
            uint index = simulationFrame >> 4 & 3U;
            return TargetLookPosFrames[index];
        }

        protected void RenderArrow(RenderManager.CameraInfo cameraInfo, Vector3 pos, Vector3 dir, Color color, float size = 0.1f) {
            color.a = alpha;
            Segment3 line = new Segment3 { a = pos, b = pos + dir };
            float minY = line.Min().y - 0.1f;
            float maxY = line.Max().y + 0.1f;
            RenderManager.instance.OverlayEffect.DrawSegment(
            cameraInfo,
            color,
            line,
            size,
            0,
            minY: minY,
            maxY: maxY,
            renderLimits: true,
            alphaBlend: alphaBlend);

            float len = Mathf.Min(dir.magnitude * 0.3f, .8f);
            dir = dir.normalized * len;
            Vector3 dir90 = dir.RotateXZ90CW();
            Segment3 line1 = new(line.b, line.b - dir + dir90);
            Segment3 line2 = new(line.b, line.b - dir - dir90);
            RenderManager.instance.OverlayEffect.DrawSegment(
                cameraInfo,
                color,
                segment1: line1,
                segment2: line2,
                size: size,
                dashLen: 0,
                minY: minY,
                maxY: maxY,
                renderLimits: renderLimits,
                alphaBlend: alphaBlend);
        }

        protected void RenderCircle(RenderManager.CameraInfo cameraInfo, Vector3 pos, Color color, float radius) {
            color.a = alpha;
            RenderManager.instance.OverlayEffect.DrawCircle(
                cameraInfo,
                color,
                pos, radius,
                pos.y - 0.1f, pos.y + 0.1f,
                renderLimits: renderLimits, alphaBlend: alphaBlend);
        }
    }
}
