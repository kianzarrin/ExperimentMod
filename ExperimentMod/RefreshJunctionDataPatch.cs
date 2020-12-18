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

    // private void NetNode.RefreshJunctionData(
    //      ushort nodeID, int segmentIndex, ushort nodeSegment, Vector3 centerPos, ref uint instanceIndex, ref RenderManager.Instance data
    [UsedImplicitly]
    [HarmonyPatch]
    static class RefreshJunctionDataPatch {
        [UsedImplicitly]
        static MethodBase TargetMethod()
        {
            return AccessTools.Method(
            typeof(NetNode),
            "RefreshJunctionData",
            new Type[] {
                typeof(ushort),
                typeof(int),
                typeof(ushort),
                typeof(Vector3),
                typeof(uint).MakeByRefType(),
                typeof(RenderManager.Instance).MakeByRefType()
            });
        }

        [UsedImplicitly]
        static void Postfix(ref NetNode __instance, ref RenderManager.Instance data, ushort nodeID, [HarmonyArgument("nodeSegment")] ushort segmentID)
        {
            if (__instance.m_flags.IsFlagSet(NetNode.Flags.Junction)) {

            }
        }
     
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, MethodBase original)
        {
            var codes = instructions.ToCodeList();
            var ldSegmentID = GetLDArg(original, "nodeSegment");
            var ldSegmentIDA = new CodeInstruction(OpCodes.Ldloc_S, 20);
            var ldSegmentIDB = new CodeInstruction(OpCodes.Ldloc_S, 21);
            int index;

            index = codes.Search(_c => _c.LoadsField(f_pavementWidth), count:1); //main left
            codes.InsertInstructions(index + 1, //after
                new[] {
                    ldSegmentID.Clone(), //ldInfo
                    new CodeInstruction(OpCodes.Ldc_I4_1), // occurance
                    new CodeInstruction(OpCodes.Call, mModifyPavement),
                });

            index = codes.Search(_c => _c.LoadsField(f_pavementWidth), count: 2); //A left
            codes.InsertInstructions(index + 1, //after
                new[] {
                    ldSegmentIDA.Clone(),
                    new CodeInstruction(OpCodes.Ldc_I4_2), // occurance
                    new CodeInstruction(OpCodes.Call, mModifyPavement),
                });

            index = codes.Search(_c => _c.LoadsField(f_pavementWidth), count: 3); //main right
            codes.InsertInstructions(index + 1, //after
                new[] {
                    ldSegmentID.Clone(),
                    new CodeInstruction(OpCodes.Ldc_I4_3), // occurance
                    new CodeInstruction(OpCodes.Call, mModifyPavement),
                });

            index = codes.Search(_c => _c.LoadsField(f_pavementWidth), count: 4); //B right
            codes.InsertInstructions(index + 1, //after
                new[] {
                    ldSegmentIDB.Clone(),
                    new CodeInstruction(OpCodes.Ldc_I4_4), // occurance
                    new CodeInstruction(OpCodes.Call, mModifyPavement),
                });

            index = codes.Search(_c => _c.LoadsField(f_pavementWidth), count: 5); //.z
            codes.InsertInstructions(index + 1, //after
                new[] {
                    ldSegmentID.Clone(),
                    new CodeInstruction(OpCodes.Ldc_I4_5), // occurance
                    new CodeInstruction(OpCodes.Call, mModifyPavement),
                });

            index = codes.Search(_c => _c.LoadsField(f_pavementWidth), count: 6); //.w
            codes.InsertInstructions(index + 1, //after
                new[] {
                    ldSegmentID.Clone(),
                    new CodeInstruction(OpCodes.Ldc_I4_6), // occurance
                    new CodeInstruction(OpCodes.Call, mModifyPavement),
                });
            return codes;
        }

        static MethodInfo mModifyPavement = GetMethod(typeof(RefreshJunctionDataPatch), nameof(ModifyPavement));
        static FieldInfo f_pavementWidth = typeof(NetInfo).GetField("m_pavementWidth");

        public static float ModifyPavement(float width, ushort segmentID, int occurance)
        {
            NetInfo info = segmentID.ToSegment().Info;
            if (info.name.Contains("Test_Offset_Road_pavement_2")) {
                Log.DebugWait("found- Test_Offset_Road_pavement_2");
                float d = occurance switch
                {
                    1 => 5, //main Left
                    2 => 2, //A left
                    3 => 5*2-2/4, //main right
                    4 => 5, //B right
                    5 => 5, //.z
                    6 => 5, //.w
                    _ => throw new Exception("unexoected occurance:"+ occurance),
                };
                width = d;
            }
            return width;
        }

    }
}