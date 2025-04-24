using System;
using HarmonyLib;
using RimWorld;
using Verse;

namespace Genes40k;

[HarmonyPatch(typeof(StorageSettings), "AllowedToAccept", new Type[]
{
    typeof(Thing),
}, new ArgumentType[]
{
    ArgumentType.Normal,
})]
public class SangprimusNoDuplicateAcceptedThing
{
    public static void Postfix(ref bool __result, Thing t, IStoreSettingsParent ___owner)
    {
        if (!__result || ___owner is not Building_SangprimusPortum sangprimusPortum)
        {
            return;
        }
        if (sangprimusPortum.SearchableContentsPrimarch.Any(thing => thing.def == t.def) || sangprimusPortum.SearchableContentsChapter.Any(thing => thing.def == t.def))
        {
            __result = false;
        }
    }
}