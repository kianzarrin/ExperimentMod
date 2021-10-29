namespace ExperimentMod {
    using ICities;
    using KianCommons;
    using Patches;
    using System.Diagnostics;
    using System.Linq;
    using UnityEngine;
    public class ThreaadingExtension : ThreadingExtensionBase {
        Stopwatch timer = Stopwatch.StartNew();
        public override void OnUpdate(float realTimeDelta, float simulationTimeDelta) {
            if(timer.ElapsedMilliseconds > 1000) {
                var dict = RenderMangerPatch.timer_dict_;
                var total = dict.Values.Sum(t => t.ElapsedMilliseconds);
                foreach(var pair in dict) {
                    float percent = (100f * pair.Value.ElapsedMilliseconds) / total;
                    Log.Info($"{pair.Key} : %{Mathf.RoundToInt(percent)}");
                }
                foreach(var timer in dict.Values)
                    timer.Reset();
            }

        }
    }
}
