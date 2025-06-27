using HarmonyLib;
using RimWorld;
using System;
using System.Linq;
using Verse;

namespace Genes40k;

[HarmonyPatch(typeof(Pawn), "Kill")]
public class LivingSaintAscension
{
    public static void Postfix(Pawn __instance)
    {
        if (!Genes40kUtils.ModSettings.livingSaintSystem)
        {
            return;
        }

        var gComp = Current.Game.GetComponent<GameComponent_LivingSaint>();
        if (gComp.LivingSaintsCount >= Genes40kUtils.ModSettings.livingSaintLimit)
        {
            return;
        }
            
        if (__instance == null)
        {
            return;
        }

        if (!Genes40kUtils.ModSettings.livingSaintMale && __instance.gender == Gender.Male)
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

        var baseChance = Genes40kUtils.ModSettings.livingSaintBaseChance;
        if (__instance.gender == Gender.Female)
        {
            baseChance++;
        }
            
        var skillAddChance = (meleeSkill - 10) * meleeLevelMult + (shootingSkill - 10) * shootingLevelMult;
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
        var resurrectionChance = baseChance + skillAddChance + traitAddChance;

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
            
        var lSSword = (ThingWithComps)ThingMaker.MakeThing(Genes40kDefOf.BEWH_LSaintSword);
        lSSword.TryGetComp<CompBiocodable>().CodeFor(__instance);
        lSSword.TryGetComp<CompQuality>().SetQuality(QualityCategory.Legendary, ArtGenerationContext.Outsider);
            
        var lSPistol = (ThingWithComps)ThingMaker.MakeThing(Genes40kDefOf.BEWH_LSaintBoltPistol);
        lSPistol.TryGetComp<CompBiocodable>().CodeFor(__instance);
        lSPistol.TryGetComp<CompQuality>().SetQuality(QualityCategory.Legendary, ArtGenerationContext.Outsider);

        if (__instance.equipment.PrimaryEq != null)
        {
            __instance.equipment.TryTransferEquipmentToContainer(__instance.equipment.PrimaryEq.parent, __instance.equipment.GetDirectlyHeldThings());
        }

        if (shootingSkill > meleeSkill)
        {
            __instance.equipment.AddEquipment(lSPistol);
            __instance.inventory.TryAddAndUnforbid(lSSword);
        }
        else
        {
            __instance.equipment.AddEquipment(lSSword);
            __instance.inventory.TryAddAndUnforbid(lSPistol);
        }
        
        if (__instance.gender != Gender.Male)
        {
            var lSArmor = (Apparel)ThingMaker.MakeThing(Genes40kDefOf.BEWH_LivingSaintArmor);
            lSArmor.TryGetComp<CompBiocodable>().CodeFor(__instance);
            lSArmor.TryGetComp<CompQuality>().SetQuality(QualityCategory.Legendary, ArtGenerationContext.Outsider);

            if (!__instance.apparel.CanWearWithoutDroppingAnything(Genes40kDefOf.BEWH_LivingSaintArmor))
            {
                bool Predicate(Apparel a) => a.def.apparel.layers.Contains(ApparelLayerDefOf.Shell);
                __instance.apparel.MoveAllToInventory(selector: Predicate);
            }
            
            __instance.apparel.Wear(lSArmor);
        }
            
        gComp.AddSaintToSpawnable(__instance);
    }    
}