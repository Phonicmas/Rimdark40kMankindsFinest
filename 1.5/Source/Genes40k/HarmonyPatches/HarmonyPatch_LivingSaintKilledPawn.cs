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
    [HarmonyPatch(typeof(Pawn), "Kill")]
    public class LivingSaintKilledPawn
    {
        public static void Prefix(Pawn __instance, DamageInfo? dinfo)
        {
            if (!__instance.RaceProps.Humanlike)
            {
                return;
            }
            
            if (!(dinfo?.Instigator is Pawn pawn))
            {
                return;
            }

            var divineRadianceGene = pawn.genes?.GetFirstGeneOfType<Gene_DivineRadiance>();

            divineRadianceGene?.KilledPawn(__instance);

        }    
    }
}