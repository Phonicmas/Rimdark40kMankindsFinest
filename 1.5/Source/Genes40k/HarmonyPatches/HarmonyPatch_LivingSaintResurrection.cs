using Core40k;
using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace Genes40k
{
    [HarmonyPatch(typeof(Pawn), "Kill")]
    public class LivingSaintResurrection
    {
        public static bool Prefix(Pawn __instance)
        {
            if (__instance.genes == null || !__instance.genes.HasActiveGene(Genes40kDefOf.BEWH_LivingSaintBeingOfFaith))
            {
                Random rand = new Random();
                int resurrectionChance = 1;

                if (__instance.gender == Gender.Female)
                {
                    resurrectionChance = 2;
                }

                if (rand.Next(0, 100) > resurrectionChance)
                {
                    return true;
                }

                __instance.genes.SetXenotypeDirect(Genes40kDefOf.BEWH_LivingSaint);
                foreach (GeneDef gene in Genes40kDefOf.BEWH_LivingSaint.genes)
                {
                    __instance.genes.AddGene(gene, true);
                }

                __instance.health.Notify_Resurrected();

                return false;
            }
            return true;
        }    
    }
}