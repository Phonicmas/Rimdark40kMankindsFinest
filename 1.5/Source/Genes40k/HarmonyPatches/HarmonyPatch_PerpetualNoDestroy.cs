using Core40k;
using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;

namespace Genes40k
{
    [HarmonyPatch(typeof(Pawn), "Destroy")]
    public class PerpetualNoDestroy
    {
        public static bool Prefix(Pawn __instance)
        {
            if (__instance.genes == null || !__instance.genes.GenesListForReading.Any(gene => gene.def.HasModExtension<DefModExtension_PerpetualGene>()))
            {
                return true;
            }

            if (!__instance.Dead)
            {
                __instance.Kill(null);
            }

            if (__instance.Spawned)
            {
                __instance.DeSpawn(); 
            }
            
            return false;
        }    
    }
}