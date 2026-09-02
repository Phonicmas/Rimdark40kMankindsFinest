using HarmonyLib;
using RimWorld;
using System.Linq;
using UnityEngine;
using Verse;

namespace Genes40k;

[HarmonyPatch(typeof(JoyUtility), "JoyTickCheckEnd")]
public class MoreArtJoy
{
    public static void Prefix(Pawn pawn, ref float extraJoyGainFactor, Building joySource)
    {
        //The stat is specifically joy from viewing art, so it must not scale every other joy source.
        if (pawn == null || joySource?.TryGetComp<CompArt>() == null)
        {
            return;
        }

        var pawnJoyFromArtFactor = pawn.GetStatValue(Genes40kDefOf.BEWH_JoyFromArtFactor, cacheStaleAfterTicks: 60);

        if (Mathf.Approximately(pawnJoyFromArtFactor, 1f))
        {
            return;
        }
        
        extraJoyGainFactor *= pawnJoyFromArtFactor;
    }
}