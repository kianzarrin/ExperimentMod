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
                    var dict = RenderMangerPatch.Timers;
                    if(dict == null) {
                        Log.Info("Timers == null");
                        return;
                    }

                    var entries = dict.OrderBy(pair => pair.Value.ElapsedMilliseconds);
                    var total = dict.Values.Cast<Stopwatch>().Sum(t => t.ElapsedMilliseconds);
                    float percentLane = (100f * Netlane_RenderPatch.Timer.ElapsedMilliseconds) / total;
                    Log.Info($"\n%{total*(100f/1000f)} of time spent in EndRendering");
                    Log.Info($"NetLane.RenderInstance/total.EndRendering = {Mathf.RoundToInt(percentLane)}");
                    Log.Info($"percentages of maneger.Endering/total.EndRendering:");
                    foreach(var pair in entries) {
                        float percent = (100f * pair.Value.ElapsedMilliseconds) / total;
                        Log.Info($"{pair.Key} : %{Mathf.RoundToInt(percent)}");
                    }

                    // reset timers
                    timer.Reset();
                    timer.Start();
                    Netlane_RenderPatch.Timer.Reset();
                    foreach(Stopwatch timer in dict.Values)
                        timer.Reset();
                }
            } catch(Exception ex) {
                ex.Log(false);
            }
        }

    }
}
