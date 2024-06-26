using HarmonyLib;
using RimWorld;
using Verse;

namespace Genes40k
{
    [HarmonyPatch(typeof(IncidentWorker), "TryExecute")]
    public class LivingSaintResurrection
    {
        public static void Prefix(IncidentWorker __instance)
        {
            GameComponent_LivingSaint gameComponent = Current.Game.GetComponent<GameComponent_LivingSaint>();
            if (gameComponent != null)
            {
                gameComponent.TrySpawnSaint(__instance.def.category);
            }
        }    
    }
}