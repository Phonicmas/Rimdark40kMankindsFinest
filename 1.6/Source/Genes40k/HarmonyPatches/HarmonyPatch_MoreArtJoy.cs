using HarmonyLib;
using RimWorld;
using System.Linq;
using UnityEngine;
using Verse;

namespace Genes40k;

[HarmonyPatch(typeof(JoyUtility), "JoyTickCheckEnd")]
public class MoreArtJoy
{
    public static void Prefix(Pawn pawn, ref float extraJoyGainFactor)
    {
        var pawnJoyFromArtFactor = pawn?.GetStatValue(Genes40kDefOf.BEWH_JoyFromArtFactor) ?? 1f;
        if (Mathf.Approximately(pawnJoyFromArtFactor, 1f))
        {
            return;
        }
        
        extraJoyGainFactor *= pawnJoyFromArtFactor;
    }
}