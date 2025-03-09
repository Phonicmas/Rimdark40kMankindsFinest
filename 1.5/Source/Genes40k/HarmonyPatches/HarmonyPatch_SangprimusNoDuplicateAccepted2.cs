using System;
using HarmonyLib;
using RimWorld;
using Verse;

namespace Genes40k
{
    [HarmonyPatch(typeof(StorageSettings), "AllowedToAccept", new Type[]
    {
        typeof(ThingDef),
    }, new ArgumentType[]
    {
        ArgumentType.Normal,
    })]
    public class SangprimusNoDuplicateAccepted2
    {
        public static void Postfix(ref bool __result, ThingDef t, IStoreSettingsParent ___owner)
        {
            if (!__result || !(___owner is Building_SangprimusPortum sangprimusPortum))
            {
                return;
            }
            if (sangprimusPortum.SearchableContentsPrimarch.Any(thing => thing.def == t) || sangprimusPortum.SearchableContentsChapter.Any(thing => thing.def == t))
            {
                __result = false;
            }
        }
    }
}