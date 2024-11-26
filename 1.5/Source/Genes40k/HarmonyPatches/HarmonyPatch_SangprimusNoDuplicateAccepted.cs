using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace Genes40k
{
    [HarmonyPatch(typeof(Building_Storage), "Accepts")]
    public class SangprimusNoDuplicateAccepted
    {
        public static void Postfix(ref bool __result, Thing t, Building_Storage __instance)
        {
            if (!__result || !(__instance is Building_SangprimusPortum sangprimusPortum))
            {
                return;
            }

            if (sangprimusPortum.SearchableContentsPrimarch.Any(thing => thing.def == t.def) || sangprimusPortum.SearchableContentsChapter.Any(thing => thing.def == t.def))
            {
                __result = false;
            }
        }
    }
}