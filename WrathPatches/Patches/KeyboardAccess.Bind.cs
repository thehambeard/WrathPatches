﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

using HarmonyLib;

using Kingmaker.UI;

namespace WrathPatches
{
    [WrathPatch("Silence 'no binding' warnings")]
    [HarmonyPatch]
    internal static class KeyboardAccess_Bind_Patch
    {
        [HarmonyPatch(typeof(KeyboardAccess), nameof(KeyboardAccess.Bind))]
        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> Bind_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var iList = instructions.ToList();
            var (index, i) = iList.Take(45).Indexed().First(i => i.item.opcode == OpCodes.Brfalse_S);

            //Main.Logger.Log($"{index}: {iList[index]}");

            iList[index] = new CodeInstruction(OpCodes.Br, i.operand);
            iList.Insert(index, new CodeInstruction(OpCodes.Pop));

            //Main.Logger.Log($"{index}: {iList[index]}");
            //Main.Logger.Log($"{index}: {iList[index + 1]}");

            return iList;
        }

        [HarmonyPatch(typeof(KeyboardAccess), nameof(KeyboardAccess.DoUnbind))]
        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> Unbind_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var iList = instructions.ToList();
            var (index, i) = iList.Take(45).Indexed().First(i => i.item.opcode == OpCodes.Brfalse_S);

            //Main.Logger.Log($"{index}: {iList[index]}");

            iList[index] = new CodeInstruction(OpCodes.Br, i.operand);
            iList.Insert(index, new CodeInstruction(OpCodes.Pop));

            //Main.Logger.Log($"{index}: {iList[index]}");
            //Main.Logger.Log($"{index}: {iList[index + 1]}");

            return iList;
        }
    }
}
