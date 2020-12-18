namespace NodeController.Patches {
    using ColossalFramework;
    using HarmonyLib;
    using JetBrains.Annotations;
    using KianCommons;
    using KianCommons.Patches;
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;
    using UnityEngine;
    using static KianCommons.Patches.TranspilerUtils;
    using System.Collections.Generic;

    // private void NetNode.RefreshJunctionData(
    //      ushort nodeID, int segmentIndex, ushort nodeSegment, Vector3 centerPos, ref uint instanceIndex, ref RenderManager.Instance data
    [UsedImplicitly]
    //[HarmonyPatch]
    static class PavementWidthPatch {
        delegate void dRefreshJunctionData(ushort nodeID, int segmentIndex, ushort nodeSegment, Vector3 centerPos, ref uint instanceIndex, ref RenderManager.Instance data);
        delegate void dPopulateGroupData(ushort nodeID, int groupX, int groupZ, int layer, ref int vertexIndex, ref int triangleIndex, Vector3 groupPosition, RenderGroup.MeshData data, ref Vector3 min, ref Vector3 max, ref float maxRenderDistance, ref float maxInstanceDistance, ref bool requireSurfaceMaps);

        [UsedImplicitly]
        static IEnumerable<MethodBase> TargetMethods()
        {
            yield return DeclaredMethod<dRefreshJunctionData>(typeof(NetNode), "RefreshJunctionData");
            //yield return DeclaredMethod<dPopulateGroupData>(typeof(NetNode), "PopulateGroupData");
            //yield return GetMethod(typeof(NetNode), "RefreshEndData");

            //yield return GetMethod(typeof(NetTool), "RenderNode");
            //yield return GetMethod(typeof(RoadBaseAI), "NodeModifyMask");
        }

        //[UsedImplicitly]
        //static void Postfix(ref NetNode __instance, ref RenderManager.Instance data, ushort nodeID, [HarmonyArgument("nodeSegment")] ushort segmentID)
        //{
        //    if (__instance.m_flags.IsFlagSet(NetNode.Flags.Junction)) {

        //    }
        //}
     
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, MethodBase original)
        {
            //if(original.Name == "NodeModifyMask")
            //    HelpersExtensions.VERBOSE = true;

            var codes = instructions.ToCodeList();
            for (int occurance = 1; occurance < 100; occurance++) {
                int index = codes.Search(_c => _c.LoadsField(f_pavementWidth), count:occurance, throwOnError:false); //main left
                if (index == -1) break;
                codes.InsertInstructions(index + 1, //after
                    new[] {
                    codes[index-1].Clone(), //ldInfo
                    new CodeInstruction(OpCodes.Ldc_I4_S, occurance),
                    new CodeInstruction(OpCodes.Call, mModifyPavementWidth),
                    });
            }

            return codes;
        }

        static MethodInfo mModifyPavementWidth = GetMethod(typeof(PavementWidthPatch), nameof(ModifyPavementWidth));
        static FieldInfo f_pavementWidth = typeof(NetInfo).GetField("m_pavementWidth");

        public static float ModifyPavementWidth(float width, NetInfo info, int occurance)
        {
            if (info.name.Contains("Test_Offset_Road_pavement_2")) {
                Log.DebugWait("found- Test_Offset_Road_pavement_2");
                width += 3;
            }
            return width;
        }

    }
}