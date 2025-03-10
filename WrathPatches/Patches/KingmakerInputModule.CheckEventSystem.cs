﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using HarmonyLib;

using Kingmaker.UI.Selection;

using Owlcat.Runtime.Core.Logging;

using WrathPatches.TranspilerUtil;

namespace WrathPatches
{
    [WrathPatch("Silence 'Await event system' messages")]
    [HarmonyPatch]
    internal static class KingmakerInputModule_CheckEventSystem
    {
        static MethodBase? TargetMethod() =>
            typeof(KingmakerInputModule)
                .GetNestedTypes(AccessTools.all)
                .Where(t => t.GetCustomAttributes<CompilerGeneratedAttribute>().Any())
                .Select(t => t.GetMethod("MoveNext", AccessTools.all))
                .FirstOrDefault();

        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            Main.Logger.Log($"{nameof(KingmakerInputModule_CheckEventSystem)}.{nameof(Transpiler)}");

            var match = new Func<CodeInstruction, bool>[]
            {
                ci => ci.LoadsField(typeof(LogChannel).GetField(nameof(LogChannel.System), AccessTools.all)),
                ci => ci.opcode == OpCodes.Ldstr && "Await event system".Equals(ci.operand),
                ci => ci.opcode == OpCodes.Call,
                ci => ci.opcode == OpCodes.Callvirt
            };

            var iMatch = instructions.FindInstructionsIndexed(match);

            if (!iMatch.Any())
            {
                Main.Logger.Log("No match found");
                return instructions;
            }

            var iList = instructions.ToList();
            
            foreach ((var index, var _) in iMatch)
            {
                var i = new CodeInstruction(OpCodes.Nop)
                {
                    labels = iList[index].labels,
                    blocks = iList[index].blocks
                };
                iList[index] = i;
            }

            return iList;
        }
    }
}
