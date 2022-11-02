namespace ExperimentMod {
    using ColossalFramework;
    using ColossalFramework.Math;
    using KianCommons;
    using KianCommons.Math;
    using KianCommons.UI;
    using System;
    using UnityEngine;
    using static NetInfo;
    using static RenderManager;

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

        public static bool CalculateTransitionBezier(this PathUnit pathUnit, byte finePathPosIndex, out Bezier3 bezier) {
            bezier = default;
            if ((finePathPosIndex & 1) == 0) return false; // transition is odd
            if ((finePathPosIndex >> 1) >= pathUnit.m_positionCount) return false; // bad index
            var pathPos1 = pathUnit.GetPosition(finePathPosIndex >> 1);
            if (pathPos1.m_segment == 0) return false;
            if (!pathUnit.GetNextPosition(finePathPosIndex >> 1, out var pathPos2)) return false;
            bezier = pathPos1.CalculateTransitionBezier(pathPos2);
            return true;
        }

        public static Vector3 GetPosition(this PathUnit.Position pathPos) =>
            pathPos.GetLane().CalculatePosition(pathPos.m_offset * BYTE2FLOAT_OFFSET);

        public static void CalculatePositionAndDirection(this PathUnit.Position pathPos, byte offset, out Vector3 pos, out Vector3 dir) {
            pathPos.GetLane().CalculatePositionAndDirection(
    offset * BYTE2FLOAT_OFFSET, out pos, out dir);
            dir.Normalize();
            if (offset == 0) dir = -dir;
        }

        public static Bezier3 CalculateTransitionBezier(this PathUnit.Position pathPos1, PathUnit.Position pathPos2) {
            pathPos1.CalculatePositionAndDirection(
        pathPos1.m_offset, out var pos1, out var dir1);

            if (pathPos1.GetNodeID() != 0) {

                byte offset2 = pathPos2.GetEndOffsetToward(pathPos1);
                pathPos2.CalculatePositionAndDirection(
                offset2, out var pos2, out var dir2);
                return BezierUtil.Bezier3ByDir(
                    pos1, dir1, pos2, dir2, true, true);
            } else {
                // use pathPos1 offset because we are transitioning from road to pavement.
                pathPos2.CalculatePositionAndDirection(
                pathPos1.m_offset, out var pos2, out var dir2);
                // straight line:
                dir1 = (pos2 - pos1).normalized;
                dir2 = -dir1;
                return BezierUtil.Bezier3ByDir(
                    pos1, dir1, pos2, dir2);
            }


        }

        public static byte GetEndOffsetToward(this PathUnit.Position pathPos, PathUnit.Position pathPos0) {
            ushort nodeId = pathPos0.GetNodeID();
            bool startNode = pathPos.m_segment.ToSegment().IsStartNode(nodeId);
            if (startNode) return 0;
            else return 255;
        }

        public static ushort GetNodeID(this ref PathUnit.Position pathPos) => pathPos.m_offset switch {
            255 => pathPos.m_segment.ToSegment().m_endNode,
            0 => pathPos.m_segment.ToSegment().m_startNode,
            _ => 0,
        };

    }

    public abstract class PathDebugger : MonoBehaviour {
        public const float BYTE2FLOAT_OFFSET = 1f / 255;
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
                GetPathInfo(
                    out uint pathUnitID,
                    out byte lastOffset,
                    out byte finePathPositionIndex,
                    out Vector3 pos0,
                    out Vector3 velocity0);
                RenderCircle(cameraInfo, pos0, Color.blue, 1);
                if (pathUnitID == 0) return;

                PathUnit.Position pathPos;
                float speed = velocity0.magnitude * 5; // meters per second
                speed = Math.Max(5, speed);
                float seconds = 5;
                float distance = speed * seconds;// velocity0.magnitude * 20; // 10 seconds
                Log.DebugWait("speed=" + velocity0.magnitude);
                float accDistance = 0;
                if (finePathPositionIndex == 255) {
                    finePathPositionIndex = 0;// initial position
                    pathPos = pathUnitID.ToPathUnit().GetPosition(finePathPositionIndex >> 1);
                    if (!pathUnitID.ToPathUnit().CalculatePathPositionOffset(
                        0, pos0, out lastOffset)) {
                        return;
                    }
                } else {
                    pathPos = pathUnitID.ToPathUnit().GetPosition(finePathPositionIndex >> 1);
                    if (pathPos.m_segment == 0) return;
                }

                Bezier3 bezier;
                if ((finePathPositionIndex & 1) == 0) { 
                    bezier = pathPos.GetLane().m_bezier;
                    bezier = bezier.Cut(lastOffset * BYTE2FLOAT_OFFSET, pathPos.m_offset * BYTE2FLOAT_OFFSET);
                } else {
                    pathUnitID.ToPathUnit().CalculateTransitionBezier(finePathPositionIndex, out bezier);
                    bezier = bezier.Cut(lastOffset * BYTE2FLOAT_OFFSET, 1);
                }
                RenderCircle(cameraInfo, bezier.d, Color.cyan, radius: 1);
                RenderCircle(cameraInfo, bezier.a, Color.red, radius: 1);
                RenderCircle(cameraInfo, pathPos.GetPosition(), Color.magenta, radius: 1);


                while (true) {
                    if (!HandleBezier(cameraInfo, bezier, ref accDistance, distance)) {
                        return;
                    }
                    var pathPos0 = pathPos;
                    finePathPositionIndex++;
                    if (!RollAndGetPathPos(ref pathUnitID, ref finePathPositionIndex, out pathPos))
                        return;
                    if ((finePathPositionIndex & 1) != 0) {
                        pathUnitID.ToPathUnit().CalculateTransitionBezier(finePathPositionIndex, out bezier);
                        RenderCircle(cameraInfo, bezier.d, Color.cyan, radius: 1);
                    } else {
                        RenderCircle(cameraInfo, pathPos.GetPosition(), Color.magenta, radius: 2);
                        bezier = pathPos.GetLane().m_bezier;
                        byte offset1;
                        if (pathPos.m_segment != pathPos0.m_segment) {
                            offset1 = pathPos.GetEndOffsetToward(pathPos0);
                        } else {
                            offset1 = pathPos0.m_offset;
                        }
                        float t1 = offset1 * BYTE2FLOAT_OFFSET;
                        float t2 = pathPos.m_offset * BYTE2FLOAT_OFFSET;
                        bezier = bezier.Cut(t1, t2);
                    }
                }
            } catch(Exception ex) { ex.Log(); }

            static bool HandleBezier(RenderManager.CameraInfo cameraInfo, Bezier3 bezier, ref float accDistance, float distance) {
                float l = bezier.ArcLength();
                if (l == 0 || l > 1000) return true; // bad bezier
                if (accDistance + l >= distance) {
                    float t = bezier.ArcTravel(distance - accDistance);
                    bezier = bezier.Cut(0, t);
                }
                accDistance += l;
                bezier.Render(cameraInfo, Color.yellow, hw: 2, alphaBlend: true, cutEnds: false);
                return accDistance < distance;
            }

            static bool RollAndGetPathPos(ref uint pathUnitID, ref byte finePathIndex, out PathUnit.Position pathPos) {
                if (finePathIndex >= 24) {
                    finePathIndex = 0;
                    pathUnitID = pathUnitID.ToPathUnit().m_nextPathUnit;
                }
                if (pathUnitID == 0 || (finePathIndex >>1) >= pathUnitID.ToPathUnit().m_positionCount) {
                    pathPos = default;
                    return false;
                }

                pathPos = pathUnitID.ToPathUnit().GetPosition(finePathIndex>>1);
                if (pathPos.m_segment == 0) return false;
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

        protected abstract void GetPathInfo(out uint pathUnitID, out byte lastOffset, out byte finePathPositionIndex, out Vector3 refPos, out Vector3 velocity);

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

        protected void RenderArrow(RenderManager.CameraInfo cameraInfo, Vector3 pos, Vector3 dir, Color color, float size = 0.05f) {
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
