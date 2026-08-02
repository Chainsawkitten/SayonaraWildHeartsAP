using HarmonyLib;
using UnityEngine;
using static SGGameLogic;
using static SGScoreHandler;

namespace SayonaraWildHeartsRandomizer;

public class Hooks
{
    [HarmonyPatch(typeof(SGMenuHandler), "UpdateLevelPageIndicators")]
    [HarmonyPrefix]
    public static bool Prefix_SGMenuHandler_UpdateLevelPageIndicators(SGMenuHandler.MENUPAGE hMenuPage, SGMenuHandler __instance)
    {
        Plugin.Logger.LogInfo("UpdateLevelPageIndicators");
        return true;
    }

    [HarmonyPatch(typeof(SGMenuHandler), "UnlockNextLevel")]
    [HarmonyPrefix]
    public static bool Prefix_SGMenuHandler_UnlockNextLevel(SGMenuHandler __instance)
    {
        Plugin.Logger.LogInfo("UnlockNextLevel");
        return true;
    }

    [HarmonyPatch(typeof(SGGameLogic.MainLogic), "GotoGameViewState")]
    [HarmonyPrefix]
    public static bool Prefix_SGGameLogic_MainLogic_GotoGameViewState(GAMEVIEWSTATE nState, SGGameLogic.MainLogic __instance)
    {
        switch (nState)
        {
            case GAMEVIEWSTATE.LEVELEND:
                Plugin.Logger.LogInfo("Level end");
                break;
            case GAMEVIEWSTATE.IMPACT:
            case GAMEVIEWSTATE.FALL:
                Plugin.Logger.LogInfo("Death");
                break;
            default:
                break;
        }

        return true;
    }

    [HarmonyPatch(typeof(SGScoreHandler), "ReportEvent", [typeof(SCOREEVENT), typeof(int), typeof(bool), typeof(Vector3)])]
    [HarmonyPrefix]
    public static bool Prefix_SGScoreHandler_ReportEvent(SCOREEVENT nEventID, int nUserData, bool bPopup, Vector3 vScreenPos, SGScoreHandler __instance)
    {
        int currentLevelIndex = SGFW.GameLogic().GetCurrentLevelIndex();

        switch (nEventID)
        {
            case SCOREEVENT.SECRETBANANA:
                Plugin.locations.CollectCoin(currentLevelIndex, nUserData);
                break;
            case SCOREEVENT.RESPAWN:
                Plugin.Logger.LogInfo("Respawn");
                break;
            case SCOREEVENT.LEVELCLEAR:
                Plugin.locations.ClearLevel(currentLevelIndex);
                break;
        }

        return true;
    }
}
