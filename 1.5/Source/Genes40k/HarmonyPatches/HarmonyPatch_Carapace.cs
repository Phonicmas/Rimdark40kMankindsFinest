using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using Verse;


namespace Genes40k
{
    //Thanks VE Team for letting me use this!
    [HarmonyPatch(typeof(StatWorker), "StatOffsetFromGear")]
    public static class StatWorker_StatOffsetFromGear_Patch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> codeInstructions)
        {
            bool patched = false;
            List<CodeInstruction> codes = codeInstructions.ToList();
            for (int i = 0; i < codes.Count; i++)
            {
                CodeInstruction code = codes[i];
                yield return code;
                if (!patched && code.opcode == OpCodes.Stloc_0)
                {
                    yield return new CodeInstruction(OpCodes.Ldloc_0, (object)null);
                    yield return new CodeInstruction(OpCodes.Ldarg_0, (object)null);
                    yield return new CodeInstruction(OpCodes.Ldarg_1, (object)null);
                    yield return new CodeInstruction(OpCodes.Call, (object)AccessTools.Method(typeof(StatWorker_StatOffsetFromGear_Patch), "ChangeValueIfNeeded", (Type[])null, (Type[])null));
                    yield return new CodeInstruction(OpCodes.Stloc_0, (object)null);
                    patched = true;
                }
            }
        }

        public static float ChangeValueIfNeeded(float val, Thing gear, StatDef stat)
        {
            if (stat == StatDefOf.MoveSpeed && val < 0f && gear.ParentHolder is Pawn_ApparelTracker pawn_ApparelTracker && pawn_ApparelTracker.pawn.genes != null && (pawn_ApparelTracker.pawn.genes.HasActiveGene(Genes40kDefOf.BEWH_BlackCarapace) || pawn_ApparelTracker.pawn.genes.HasActiveGene(Genes40kDefOf.BEWH_ImmortisGland) || pawn_ApparelTracker.pawn.genes.HasActiveGene(Genes40kDefOf.BEWH_AtlasMorphogen)))
            {
                return 0f;
            }
            return val;
        }
    }
}