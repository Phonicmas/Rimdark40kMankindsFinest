using System.Linq;
using HarmonyLib;
using RimWorld;

namespace Genes40k;

[HarmonyPatch(typeof(Apparel), "WornGraphicPath", MethodType.Getter)]
public class HideJumpPack
{
    public static void Postfix(ref string __result, Apparel __instance)
    {
        if ( __instance.def != Genes40kDefOf.Apparel_PackJump)
        {
            return;
        }

        if (__instance.Wearer?.apparel?.WornApparel == null)
        {
            return;
        }

        if (!__instance.Wearer.apparel.WornApparel.Any(apparel => apparel.def.HasModExtension<DefModExtension_HideJumpPack>()))
        {
            return;
        }
        
        const string texPath = "Things/Armor/Imperium/PowerArmor/CommonIcons/BEWH_None";
        __result = texPath;
    }
}