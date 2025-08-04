using LudeonTK;
using Verse;

namespace Genes40k;

public static class DebugActions
{
    [DebugAction("RimDark", "Get dead perpetual info", false, false, true, false, false,0, false, actionType = DebugActionType.Action, allowedGameStates = AllowedGameStates.PlayingOnMap, displayPriority = -1000)]
    private static void DeadPerpetualInfo()
    {
        var gameComponentPerpetual = Current.Game.GetComponent<GameComponent_Perpetual>();
        Log.Message("Current dead perpetual amount: " + gameComponentPerpetual.Perpetuals?.Count);
        var currentTime = Current.Game.tickManager.TicksGame;
        foreach (var perpetualDict in gameComponentPerpetual.Perpetuals)
        {
            Log.Message("Perpetual: " + perpetualDict.Key);
            Log.Message("Time left: " + (perpetualDict.Value - currentTime));
        }
    }
    
    [DebugAction("RimDark", "Get dead living saint info", false, false, true, false, false,0, false, actionType = DebugActionType.Action, allowedGameStates = AllowedGameStates.PlayingOnMap, displayPriority = -1000)]
    private static void DeadLivingSaintInfo()
    {
        var gameComponentLivingSaint = Current.Game.GetComponent<GameComponent_LivingSaint>();
        Log.Message("Current dead perpetual amount: " + gameComponentLivingSaint.LivingSaintsCount);
        foreach (var livingSaint in gameComponentLivingSaint.LivingSaints)
        {
            Log.Message("Living Saint: " + livingSaint);
        }
    }
}