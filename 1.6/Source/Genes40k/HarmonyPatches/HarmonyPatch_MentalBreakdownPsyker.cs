using Core40k;
using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;

namespace Genes40k;

[HarmonyPatch(typeof(MentalStateHandler), "TryStartMentalState")]
public class MentalBreakdownPsyker
{
    public static void Postfix(MentalStateHandler __instance, bool __result)
    {
        var modSettings = Genes40kUtils.ModSettings;
        if (!modSettings.psychicPhenomena)
        {
            return;
        }
        //Mental state didn't start, skip
        if (!__result)
        {
            return;
        }

        var pawn = __instance?.CurState?.pawn;

        if (pawn?.genes == null || pawn.genes.GenesListForReading.NullOrEmpty())
        {
            return;
        }

        if (!pawn.genes.GenesListForReading.Where(gene => gene.def.HasModExtension<DefModExtension_Psyker>()).Any(gene => gene.Active))
        {
            return;
        }
            
        var letter = new StandardLetter
        {
            lookTargets = pawn,
            def = Genes40kDefOf.BEWH_NaturalBornX,
        };
        var sendLetter = true;
        var rand = new Random();
        var roll = rand.Next(1, 100);
        roll += (int)pawn.GetStatValue(StatDefOf.PsychicSensitivity);

        letter.Label = roll >= 90 ? "BEWH.MankindsFinest.Event.PerilsOfTheWarpLetter".Translate() : "BEWH.MankindsFinest.Event.PsychicPhenomenaLetter".Translate();
            
        switch (roll)
        {
            case 100:
                pawn.Kill(null);
                GenExplosion.DoExplosion(pawn.Corpse.Position, pawn.Corpse.Map, pawn.GetStatValue(StatDefOf.PsychicSensitivity) * 5, Genes40kDefOf.BEWH_WarpEnergy, pawn, damAmount: (int)(pawn.GetStatValue(StatDefOf.PsychicSensitivity) * 100), armorPenetration: 10f);
                letter.Text = "BEWH.MankindsFinest.Event.Annihilation".Translate(pawn.Named("PAWN"));
                break;
            case >= 99:
                SummonDaemons(pawn);
                letter.Text = "BEWH.MankindsFinest.Event.DaemonHost".Translate(pawn.Named("PAWN"));
                break;
            case >= 95:
                GenExplosion.DoExplosion(pawn.Position, pawn.Map, pawn.GetStatValue(StatDefOf.PsychicSensitivity) * 5, Genes40kDefOf.BEWH_WarpEnergy, pawn);
                letter.Text = "BEWH.MankindsFinest.Event.UncontrollablePowers".Translate(pawn.Named("PAWN"));
                break;
            case >= 90:
                pawn.health?.AddHediff(Genes40kDefOf.BEWH_PsychicComa);
                letter.Text = "BEWH.MankindsFinest.Event.PsychicComa".Translate(pawn.Named("PAWN"));
                break;
            /*case int n when n >= 80:
                        //??
                        break;*/
            case >= 70:
                pawn.health?.AddHediff(Genes40kDefOf.BEWH_PsychicConnectionSevered);
                letter.Text = "BEWH.MankindsFinest.Event.PsychicConnectionSevered".Translate(pawn.Named("PAWN"));
                break;
            case >= 60:
                pawn.Map?.weatherManager?.TransitionTo(Genes40kDefOf.BEWH_BloodRain);
                letter.Text = "BEWH.MankindsFinest.Event.BloodRain".Translate();
                break;
            case >= 30:
                var t = GenRadial.RadialCellsAround(pawn.Position, 8, true);
                foreach (var c in t)
                {
                    var plant = c.GetPlant(pawn.Map);
                    plant?.Kill();
                }
                letter.Text = "BEWH.MankindsFinest.Event.PlantRot".Translate(pawn.Named("PAWN"));
                break;
            default:
                sendLetter = false;
                break;
        }
        if (sendLetter)
        {
            Find.LetterStack.ReceiveLetter(letter);
        }
    }
    
    private static void SummonDaemons(Pawn pawn)
    {
        var rand = new Random(); 

        var randNum = rand.Next(1, (int)pawn.GetStatValue(StatDefOf.PsychicSensitivity));

        //Make sort of portal first, then spawn.
        for (var i = 0; i < randNum; i++)
        {
            var toSpawn = PawnGenerator.GeneratePawn(PawnKindDefOf.Megaspider, null);
            GenSpawn.Spawn(toSpawn, pawn.Position.RandomAdjacentCell8Way(), pawn.Map);
            toSpawn.mindState.mentalStateHandler.TryStartMentalState(MentalStateDefOf.ManhunterPermanent);
        }
    }
    
}