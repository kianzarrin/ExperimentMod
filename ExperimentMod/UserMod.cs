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
            CreateGarbage();
        }

        static int CreateGarbage()
        {
            try
            {
                long before = GC.GetTotalMemory(false);
                var data = new Data[10];
                int counter = data.Sum(item => item.Confuse());
                long after = GC.GetTotalMemory(false);
                Log.Debug($"allocated: {(after - before)/1024l}KB  ({after / 1024l} - {before / 1024l}) ");
                return counter;
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
                return 0;
            }
        }

        public unsafe struct Data
        {
            public fixed int x[1000];
            public int Confuse()
            {
                try
                {
                    return x[10];
                }
                catch(Exception ex)
                {
                    Log.Exception(ex);
                    return 0;
                }
            }
        }

    }
}
