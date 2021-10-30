namespace ExperimentMod {
    using ICities;
    using KianCommons;
    using Patches;
    using System;
    using System.Diagnostics;
    using System.Linq;
    using UnityEngine;
    public class ThreaadingExtension : ThreadingExtensionBase {
        Stopwatch timer = Stopwatch.StartNew();
        public override void OnUpdate(float realTimeDelta, float simulationTimeDelta) {
            try {
                if(timer.ElapsedMilliseconds > 1000) {
                    timer.Reset();
                    timer.Start();

                    var hashtable = RenderMangerPatch.Timers;
                    Log.Info("manager times are:");
                    if(hashtable == null) {
                        Log.Info("hashtable is null");
                        return;
                    }
                    var total = hashtable.Values.Cast<Stopwatch>().Sum(t => t.ElapsedMilliseconds);
                    foreach(IRenderableManager man in hashtable.Keys) {
                        var timer = hashtable[man] as Stopwatch;
                        float percent = (100f * timer.ElapsedMilliseconds) / total;
                        Log.Info($"{man} : %{Mathf.RoundToInt(percent)}");
                    }
                    foreach(Stopwatch timer in hashtable.Values)
                        timer.Reset();
                }
            } catch(Exception ex) {
                ex.Log(false);
            }
        }
    }
}
