namespace ExperimentMod
{
    using System;
    using ICities;
    using KianCommons;
    using System.Linq;

    public class UserMod : IUserMod
    {
        public static Version ModVersion => typeof(UserMod).Assembly.GetName().Version;
        public static string VersionString => ModVersion.ToString(2);
        public string Name => "Experiment Mod " + VersionString;
        public string Description => "test GC";
    }

    public class ThreadingExtension : ThreadingExtensionBase
    {
        public override void OnCreated(IThreading threading)
        {
            Log.Buffered = true;
            base.OnCreated(threading);
            Log.Info("ThreadingExtension.OnCreated");
        }

        public override void OnUpdate(float realTimeDelta, float simulationTimeDelta)
        {
            try
            {
                CreateGarbage();
            } catch(Exception ex) {
                Log.Exception(ex);
            }
        }

        static void CreateGarbage()
        {
            long before = GC.GetTotalMemory(false);
            int[] garbage;
            for (int i = 0; i < 1000; ++i)
                garbage = new int[100];
            long after = GC.GetTotalMemory(false);
            Log.Debug($"allocated: {(after - before)/1024l}KB  ({after / 1024l} - {before / 1024l}) ");
        }

    }
}
