﻿using System;
using HarmonyLib;
using RimWorld;
using Verse;

namespace Genes40k
{
    [HarmonyPatch(typeof(TraitSet), "GetTrait", new Type[]
    {
        typeof(TraitDef),
    }, new ArgumentType[]
    {
        ArgumentType.Normal,
    })]
    public class SerfGetTrait2
    {
        public static void Postfix(Pawn ___pawn, TraitDef tDef)
        {
            if (tDef != Genes40kDefOf.BEWH_Serf)
            {
                return;
            }

            if (___pawn.health.hediffSet.GetFirstHediffOfDef(Genes40kDefOf.BEWH_SerfBuff) == null)
            {
                ___pawn.health.AddHediff(Genes40kDefOf.BEWH_SerfBuff);
            }
        }
    }
}