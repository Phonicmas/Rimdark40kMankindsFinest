using HarmonyLib;
using RimWorld;
using System;
using System.Linq;
using Verse;

namespace Genes40k
{
    [HarmonyPatch(typeof(Pawn), "Kill")]
    public class LivingSaintAscension
    {
        public static void Postfix(Pawn __instance)
        {
            if (__instance == null)
            {
                return;
            }
            if (__instance.Faction != Faction.OfPlayer || __instance.genes == null)
            {
                return;
            }

            if (__instance.IsGhoul || __instance.IsSlave || __instance.IsPrisoner)
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
            else if (__instance.story.traits.HasTrait(Genes40kDefOf.PsychicSensitivity, 1))
            {
                traitAddChance = 5f;
            }
            else if (__instance.story.traits.HasTrait(Genes40kDefOf.PsychicSensitivity, -1))
            {
                traitAddChance = -10f;
            }
            else if (__instance.story.traits.HasTrait(Genes40kDefOf.PsychicSensitivity, -2))
            {
                traitAddChance = -200f;
            }

            var rand = new Random();
            var resurrectionChance = baseChance + skillAddsChance + traitAddChance;

            /*if (Prefs.DevMode && DebugSettings.godMode)
            {
                resurrectionChance = 400;
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
            
            var letter = LetterMaker.MakeLetter("BEWH.MankindsFinest.CommonKeywords.LivingSaint".Translate(), "BEWH.MankindsFinest.LivingSaint.LivingSaintMessage".Translate(__instance), Genes40kDefOf.BEWH_GoldenPositive, __instance);
            Find.LetterStack.ReceiveLetter(letter);
            
            switch (__instance.Name)
            {
                case NameTriple triple:
                    var nameTriple = new NameTriple("St. " + triple.First, "St. " + triple.Nick, triple.Last);
                    __instance.Name = nameTriple;
                    break;
                case NameSingle single:
                    var nameSingle = new NameSingle("St. " + single.Name);
                    __instance.Name = nameSingle;
                    break;
            }
        }    
    }
}