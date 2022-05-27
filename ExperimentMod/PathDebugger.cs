namespace ExperimentMod {
    using ColossalFramework.Math;
    using KianCommons;
    using System;
    using UnityEngine;

    public static class Extensions {
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
    }

    public abstract class PathDebugger : MonoBehaviour {
        protected const float alpha = .75f;
        protected const bool renderLimits = true;
        protected const bool alphaBlend = true;
        protected static Vector4[] TargetLookPosFrames = new Vector4[4];
        protected virtual bool showFrame => ModSettings.ShowFrames;
        protected virtual bool showTargets => ModSettings.ShowTargetPos;
        protected virtual bool showLookTargets => ModSettings.ShowLookTarget;

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
                        RenderCircle(cameraInfo, GetTargetPosFrame(targetF), colorT, r);
                    }
                }
                {
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
                RenderOverlay(cameraInfo, id);
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
            Vector4 pos1 = GetTargetPosFrame(targetFrame - 16U);
            Vector4 pos2 = GetTargetPosFrame(targetFrame - 0);
            float t = ((targetFrame & 15U) + SimulationManager.instance.m_referenceTimer) * 0.0625f;
            return Vector3.Lerp(pos1, pos2, t);
        }

        protected abstract void GetSmoothPosition(out Vector3 pos, out Quaternion rot);
        protected virtual InstanceID SelectedInstance => InstanceManager.instance.GetSelectedInstance();
        protected abstract bool GetID(out ushort id);
        protected ushort GetID() {
            GetID(out ushort id);
            return id;
        }

        protected Vector4 GetTargetPosFrame(uint simulationFrame) {
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
