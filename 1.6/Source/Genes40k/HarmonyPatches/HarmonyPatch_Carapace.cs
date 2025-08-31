using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using Verse;

namespace Genes40k;

//Thanks VE Team for letting me use this!
[HarmonyPatch(typeof(StatWorker), "StatOffsetFromGear")]
public static class StatWorker_StatOffsetFromGear_Patch
{
    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> codeInstructions)
    {
        var patched = false;
        var codes = codeInstructions.ToList();
        foreach (var code in codes)
        {
            yield return code;
            if (patched || code.opcode != OpCodes.Stloc_0)
            {
                continue;
            }
            
            yield return new CodeInstruction(OpCodes.Ldloc_0);
            yield return new CodeInstruction(OpCodes.Ldarg_0);
            yield return new CodeInstruction(OpCodes.Ldarg_1);
            yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(StatWorker_StatOffsetFromGear_Patch), "ChangeValueIfNeeded"));
            yield return new CodeInstruction(OpCodes.Stloc_0);
            patched = true;
        }
    }

    public static float ChangeValueIfNeeded(float val, Thing gear, StatDef stat)
    {
        if (stat == StatDefOf.MoveSpeed && val < 0f && gear.ParentHolder is Pawn_ApparelTracker pawn_ApparelTracker)
        {
            if (!gear.def.HasModExtension<DefModExtension_IgnoreMovespeedDecrease>() || pawn_ApparelTracker.pawn.genes == null)
            {
                return val;
            }

            var defMod = gear.def.GetModExtension<DefModExtension_IgnoreMovespeedDecrease>();

            if (Enumerable.Any(pawn_ApparelTracker.pawn.genes.GenesListForReading, gene => gene.def.HasModExtension<DefModExtension_IgnoreMovespeedDecrease>()))
            {
                return defMod.newMoveSpeedOffset;
            }
        }
        return val;
    }
}