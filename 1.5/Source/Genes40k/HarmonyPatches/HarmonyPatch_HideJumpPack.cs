using HarmonyLib;
using RimWorld;

namespace Genes40k
{
    [HarmonyPatch(typeof(Apparel), "WornGraphicPath", MethodType.Getter)]
    public class HideJumpPack
    {
        public static void Postfix(ref string __result, Apparel __instance)
        {
            if ( __instance.def != Genes40kDefOf.Apparel_PackJump)
            {
                return;
            }

            foreach (var apparel in __instance.Wearer.apparel.WornApparel)
            {
                if (apparel.def.HasModExtension<DefModExtension_HideJumpPack>())
                {
                    const string texPath = "Things/Armor/Imperium/PowerArmor/CommonIcons/BEWH_None";
                    __result = texPath;
                }
            }
        }
    }
}