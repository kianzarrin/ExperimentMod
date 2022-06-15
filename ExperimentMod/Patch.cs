namespace ExperimentMod {
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;
    using HarmonyLib;
    using KianCommons;
    using KianCommons.Patches;
    using System.Text.RegularExpressions;

    [HarmonyPatch]
    public static class ReadTypePatch {
        static MethodBase TargetMethod() {
            var t = Type.GetType("System.Runtime.Serialization.Formatters.Binary.ObjectReader");
            return AccessTools.DeclaredMethod(t, "ReadType");
        }

        delegate Type GetType(string typeName);
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
            MethodInfo m = TranspilerUtils.DeclaredMethod<GetType>(typeof(Type));
            MethodInfo mReplaceGeneric = AccessTools.DeclaredMethod(typeof(ReadTypePatch), nameof(ReplaceGeneric));

            foreach (var code in instructions) {
                if (code.Calls(m)) {
                    yield return new CodeInstruction(OpCodes.Call, mReplaceGeneric);
                }
                yield return code;
            }
        }

        static string ReplaceGeneric(string s) {
            if (s.Contains("Harmony")) {
                return s;
            }
            s = ReplaceGenericImpl("ExperimentMod", s);
            return s;
        }

        static string ReplaceGenericImpl(string asm, string s) {
            string assembly = typeof(ReadTypePatch).Assembly.GetName().Name;
            var v = typeof(ReadTypePatch).VersionOf();
            string nd = "\\d+\\."; // number.
            string patter = $"{assembly}, Version={nd}{nd}{nd}\\d*, Culture=neutral, PublicKeyToken=null";
            //string replacement = $"{assembly}";
            string replacement = $"{assembly}, Version={v}, Culture=neutral, PublicKeyToken=null";
            var s2 = Regex.Replace(s, patter, replacement);
            return s2.LogRet(ReflectionHelpers.CurrentMethod(1, asm, s));
        }

        static void Postfix(object code, ref Type __result) {
            if (!__result.FullName.Contains("Harmony")) {
                Log.Called(code, __result);
                foreach(var parameterType in __result.GetGenericArguments()) {
                    Log.Info(parameterType.AssemblyQualifiedName);
                }
                if(__result.GetElementType() is Type elementType) {
                    Log.Info(elementType.AssemblyQualifiedName);
                }
            }
        }
    }
}
