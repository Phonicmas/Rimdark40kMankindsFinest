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
    public class LivingSaintDeath
    {
        public static void Postfix(Pawn __instance)
        {
            if (__instance.Faction != Faction.OfPlayer || __instance.genes == null)
            {
                return;
            }

            var forbiddenGenes = Genes40kDefOf.BEWH_LivingSaintBeingOfFaith.GetModExtension<DefModExtension_LivingSaint>().cantHaveGenes;

            if (Enumerable.Any(__instance.genes.GenesListForReading, gene => forbiddenGenes.Contains(gene.def)))
            {
                return;
            }

            var shootingSkill = __instance.skills.GetSkill(SkillDefOf.Shooting).levelInt;
            var meleeSkill = __instance.skills.GetSkill(SkillDefOf.Melee).levelInt;

            if (shootingSkill + meleeSkill < 12)
            {
                return;
            }

            var shootingLevelMult = shootingSkill > 10 ? 0.5f : 0f;
            var meleeLevelMult = meleeSkill > 10 ? 0.5f : 0f;

            if (shootingSkill >= 15)
            {
                shootingLevelMult = shootingSkill >= 20 ? 1.5f : 1;
            }
            if (meleeSkill >= 15)
            {
                meleeLevelMult = meleeSkill >= 20 ? 1.5f : 1;
            }

            var baseChance = __instance.gender == Gender.Female ? 2f : 1f;
            var skillAddsChance = (meleeSkill - 10) * meleeLevelMult + (shootingSkill - 10) * shootingLevelMult;
            var traitAddChance = 0f;

            if (__instance.story.traits.HasTrait(Genes40kDefOf.PsychicSensitivity, 2))
            {
                traitAddChance = 10f;
            }
            else if (__instance.story.traits.HasTrait(Genes40kDefOf.PsychicSensitivity, 2))
            {
                traitAddChance = 5f;
            }

            var rand = new Random();
            var resurrectionChance = baseChance + skillAddsChance + traitAddChance;

            /*if (Prefs.DevMode && DebugSettings.godMode)
            {
                resurrectionChance = 200;
            }*/

            const int chanceMax = 100;

            if (rand.Next(0, chanceMax) > resurrectionChance)
            {
                return;
            }
            
            __instance.genes.SetXenotypeDirect(Genes40kDefOf.BEWH_LivingSaint);
            
            foreach (var gene in Genes40kDefOf.BEWH_LivingSaint.genes)
            {
                __instance.genes.AddGene(gene, true);
            }

            ResurrectionUtility.TryResurrect(__instance);

            var letter = LetterMaker.MakeLetter("BEWH.LivingSaint".Translate(), "BEWH.LivingSaintMessage".Translate(__instance), Genes40kDefOf.BEWH_GoldenPositive, __instance);
            Find.LetterStack.ReceiveLetter(letter);
        }    
    }
}