namespace ExperimentMod {
    using ICities;
    using KianCommons;
    using Patches;
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Collections.Generic;
    using UnityEngine;
    public class ThreaadingExtension : ThreadingExtensionBase {
        Stopwatch timer_ = Stopwatch.StartNew();
        const float INTERVAL = 1000f;
        public override void OnUpdate(float realTimeDelta, float simulationTimeDelta) {
            try {
                if(timer_.ElapsedMilliseconds > INTERVAL) {
                    var dict = TimeEndRenderingPatch.Timers;
                    var entries = dict.OrderByDescending(pair => pair.Value.ElapsedMilliseconds);
                    var totalEndRendering = dict.Values.Sum(t => t.ElapsedMilliseconds) + LightSystemEndRenderingPatch.Timer.ElapsedMilliseconds;
                    var totalBeginRendering = TimeBeginRenderingPatch.Timers.Values.Sum(t => t.ElapsedMilliseconds);
                    var stages = new Dictionary<string, float> {
                        { "LateUpdate", Patches.RenderManager_.LateUdatePatch.Timer.ElapsedMilliseconds},
                        { "RenderOverlay", Patches.RenderManager_.RenderOverlayPatch.Timer.ElapsedMilliseconds},
                        { "UpdateCameraInfo", Patches.RenderManager_.UpdateCameraInfoPatch.Timer.ElapsedMilliseconds},
                        { "UpdateColorMap", Patches.RenderManager_.UpdateColorMapPatch.Timer.ElapsedMilliseconds},
                        { "RenderGroupRender(camera)", RenderGroup_RenderPatch1.Timer.ElapsedMilliseconds},
                        { "RenderGroup_Render(mask)", RenderGroup_RenderPatch2.Timer.ElapsedMilliseconds},
                        { "MegaRenderGroup.Render", MegaRenderGroup_RenderPatch.Timer.ElapsedMilliseconds},
                        { "BeginRendering", totalBeginRendering},
                        { "EndRendering", totalEndRendering },
                    };

                    Log.Info("==================================================");
                    Log.Info("percentages of total time spend in each stage:");
                    foreach(var stage in stages) {
                        Log.Info($"{stage.Key} : %{CalculatePercent(stage.Value)}");
                    }

                    Log.Info($"percentages of maneger.EndRendering/total_EndRendering:");
                    Log.Info($"NetLane.RenderInstance : %{CalculatePercent(Netlane_RenderPatch.Timer, totalEndRendering)}");
                    Log.Info($"LightSystem : %{CalculatePercent(LightSystemEndRenderingPatch.Timer.ElapsedMilliseconds, totalEndRendering)}");
                    foreach(var pair in entries) {
                        Log.Info($"{pair.Key} : %{CalculatePercent(pair.Value.ElapsedMilliseconds, totalEndRendering)}");
                    }
                    Log.Info("==================================================");

                    // reset timers
                    timer_.Reset();
                    timer_.Start();

                    foreach(Stopwatch timer in TimeBeginRenderingPatch.Timers.Values)
                        timer.Reset();

                    foreach(Stopwatch timer in dict.Values)
                        timer.Reset();
                    Netlane_RenderPatch.Timer.Reset();
                    LightSystemEndRenderingPatch.Timer.Reset();

                    Patches.RenderManager_.LateUdatePatch.Timer.Reset();
                    Patches.RenderManager_.RenderOverlayPatch.Timer.Reset();
                    Patches.RenderManager_.UpdateCameraInfoPatch.Timer.Reset();

                    RenderGroup_RenderPatch1.Timer.Reset();
                    RenderGroup_RenderPatch2.Timer.Reset();
                    MegaRenderGroup_RenderPatch.Timer.Reset();
                }
            } catch(Exception ex) {
                ex.Log(false);
            }
        }

        public static float CalculatePercent(Stopwatch timer, float total = INTERVAL) {
            return CalculatePercent(Netlane_RenderPatch.Timer.ElapsedMilliseconds, total);
        }

        public static float CalculatePercent(float time, float total = INTERVAL) {
            var ret = (100f * time) / total;
            return Mathf.Round(ret);
        }
    }
}
