using HarmonyLib;
using Verse;

namespace Genes40k;

[HarmonyPatch(typeof(Pawn), "Destroy")]
public class LivingSaintDestroyRemoveCount
{
    public static void Postfix(Pawn __instance)
    {
        if (__instance?.genes == null || !__instance.IsLivingSaint())
        {
            return;
        }
            
        var gComp = Current.Game.GetComponent<GameComponent_LivingSaint>();
        gComp.RemoveSaintFromSpawnable(__instance);
    }    
}