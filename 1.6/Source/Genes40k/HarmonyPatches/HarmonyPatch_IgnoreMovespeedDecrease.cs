using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using Verse;

namespace Genes40k;

//Thanks VE Team for letting theirs as a base!
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
        if (stat == StatDefOf.MoveSpeed && val < 0f && TryGetNegatingGene(gear, stat, out var negatingGene))
        {
            var defMod = gear.def.GetModExtension<DefModExtension_IgnoreMovespeedDecrease>();
            return defMod?.newMoveSpeedOffset ?? 0f;
        }
        return val;
    }

    // Shared by both the numeric override above and the stat-card explanation patch below, so the
    // two never disagree about whether a given piece of gear's movespeed penalty is being negated.
    // Mirrors the check StatWorker.StatOffsetFromGear itself does (StatUtility.GetStatOffsetFromList
    // against gear.def.equippedStatOffsets) rather than re-deriving the fully patched value, since by
    // the time InfoTextLineFromGear's postfix runs, the return value has already been zeroed out.
    public static bool TryGetNegatingGene(Thing gear, StatDef stat, out Gene negatingGene)
    {
        negatingGene = null;
        if (stat != StatDefOf.MoveSpeed || gear?.def?.equippedStatOffsets == null)
        {
            return false;
        }

        if (gear.ParentHolder is not Pawn_ApparelTracker pawn_ApparelTracker || pawn_ApparelTracker.pawn.genes == null)
        {
            return false;
        }

        if (StatUtility.GetStatOffsetFromList(gear.def.equippedStatOffsets, stat) >= 0f)
        {
            return false;
        }

        negatingGene = pawn_ApparelTracker.pawn.genes.GenesListForReading
            .FirstOrDefault(gene => gene.def.HasModExtension<DefModExtension_IgnoreMovespeedDecrease>());
        return negatingGene != null;
    }
}

// The transpiler above only fixes the number - the "Relevant gear" breakdown on the stat card still
// calls StatWorker.InfoTextLineFromGear to build each item's line, which shows the correctly-zeroed
// offset but with no indication why it isn't negative. This appends that explanation.
[HarmonyPatch(typeof(StatWorker), "InfoTextLineFromGear")]
public static class StatWorker_InfoTextLineFromGear_Patch
{
    public static void Postfix(Thing gear, StatDef stat, ref string __result)
    {
        if (StatWorker_StatOffsetFromGear_Patch.TryGetNegatingGene(gear, stat, out var negatingGene))
        {
            __result += " (" + "BEWH.MankindsFinest.MovespeedPenaltyIgnored".Translate(negatingGene.LabelCap) + ")";
        }
    }
}