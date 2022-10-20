namespace ExperimentMod {
    using System;
    using System.Collections.Generic;
    using ICities;
    using KianCommons;
    using KianCommons.IImplict;
    using TrafficManager.API;
    using TrafficManager.API.Traffic;

    public class TMPEBasedClass : TrafficManager.API.Traffic.ISegmentEnd
    {
        public void Update()
        {
            throw new NotImplementedException();
        }

        public IDictionary<ushort, uint>[] MeasureOutgoingVehicles(bool includeStopped = true, bool debug = false)
        {
            throw new NotImplementedException();
        }

        public ushort NodeId { get; }

        public bool Relocate(ushort segmentId, bool startNode)
        {
            throw new NotImplementedException();
        }

        public ushort SegmentId { get; }
        public bool StartNode { get; }

        public bool Equals(ISegmentEndId other)
        {
            throw new NotImplementedException();
        }
    }

    public class UserMod : IUserMod, IMod {
        static UserMod() {
            Log.Called();
            Log.Debug(Environment.StackTrace);
        }

        public UserMod()
        {
            Log.Called();
            Log.Debug(Environment.StackTrace);
        }

        public static Version ModVersion => typeof(UserMod).Assembly.GetName().Version;
        public static string VersionString => ModVersion.ToString(2);
        public string Name => "Experiment Mod " + VersionString;
        public string Description => "control Road/junction transitions";

        public void OnEnabled()
        {
            Log.Called();
            Log.Debug(Environment.StackTrace);
        }

        public void OnDisabled()
        {
            Log.Called();
            Log.Debug(Environment.StackTrace);
        }

        public void OnSettingsUI(UIHelperBase helper)
        {
            Log.Called();
            Log.Debug(Environment.StackTrace);
        }
    }
}
