using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace Genes40k;

[HarmonyPatch(typeof(TransferableUIUtility), "DoExtraIcons")]
public static class ChapterMaterialCheckboxedPatch
{
    private static string cachedTooltip;
    private static LoadedLanguage cachedTooltipLanguage;

    private static string Tooltip
    {
        get
        {
            if (cachedTooltip == null || cachedTooltipLanguage != LanguageDatabase.activeLanguage)
            {
                cachedTooltipLanguage = LanguageDatabase.activeLanguage;
                cachedTooltip = "BEWH.MankindsFinest.Other.AlreadyUnlocked".Translate();
            }

            return cachedTooltip;
        }
    }

    public static void Postfix(Transferable trad, Rect rect, ref float curX)
    {
        if (trad.AnyThing is not GeneMaterialExtra materialExtra)
        {
            return;
        }

        var gameComp = GameComponent_UnlockedMaterials.Instance;

        if (gameComp == null || !gameComp.HasMaterial(materialExtra.def))
        {
            return;
        }
        
        var iconRect = new Rect(curX - 24f, (rect.height - 24f) / 2f, 24f, 24f);
        GUI.DrawTexture(iconRect, Widgets.GetCheckboxTexture(true));
        TooltipHandler.TipRegion(iconRect, Tooltip);
        curX -= 24f;
    }
}