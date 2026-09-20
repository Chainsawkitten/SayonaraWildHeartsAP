using HarmonyLib;
using UnityEngine;
using static SGGameLogic;
using static SGMenuHandler;
using static SGScoreHandler;

namespace SayonaraWildHeartsRandomizer;

public class Hooks
{
    [HarmonyPatch(typeof(SGMenuHandler), "UpdateLevelPageIndicators")]
    [HarmonyPrefix]
    public static bool Prefix_SGMenuHandler_UpdateLevelPageIndicators(SGMenuHandler.MENUPAGE hMenuPage, SGMenuHandler __instance)
    {
        if (!Plugin.multiWorld.connected)
        {
            return true;
        }

        // Update level select page indicators to match Archipelago state, rather than core game state.
        if (hMenuPage.hLevelSelectPageIndicators == null)
        {
            return false;
        }

        for (int i = 0; i < hMenuPage.hLevelSelectPageIndicators.Length; i++)
        {
            SGMenuItem sGMenuItem = hMenuPage.hLevelSelectPageIndicators[i];
            if (Plugin.items.IsLevelLocked(i))
            {
                sGMenuItem.mainTextureOffset.x = 0.5f;
            }
            else if (Plugin.locations.IsLevelCleared(i) && Plugin.locations.GetCoinsCollectedCount(i) == __instance.m_hMainMenuPages[i + 1].hLevelDesc.LEVEL_BANANA_COUNT)
            {
                sGMenuItem.mainTextureOffset.x = 0.75f;
            }
            else
            {
                sGMenuItem.mainTextureOffset.x = 0.25f;
            }

            sGMenuItem.mainTextureOffset.y = ((hMenuPage.nPageIndex - 1 != i) ? 0f : 0.5f);
            sGMenuItem.SetTextureUVOffset(0f, 0f);
        }

        return false;
    }

    [HarmonyPatch(typeof(SGMenuHandler), "IsMenuPageLocked")]
    [HarmonyPostfix]
    public static void Postfix_SGMenuHandler_IsMenuPageLocked(int nPageIndex, SGMenuHandler __instance, ref bool __result)
    {
        if (!Plugin.multiWorld.connected)
        {
            return;
        }

        if (nPageIndex < 1 || nPageIndex > __instance.m_hMainMenuPages.Count - 1)
        {
            return;
        }

        // Lock/unlock levels based on received items.
        __result = Plugin.items.IsLevelLocked(nPageIndex - 1);
    }

    [HarmonyPatch(typeof(SGMenuHandler), "ProcessMainMenu")]
    [HarmonyPrefix]
    public static bool Prefix_SGMenuHandler_ProcessMainMenu(float fDeltaTime, SGMenuHandler __instance)
    {
        if (!Plugin.multiWorld.connected)
        {
            return true;
        }

        // Fix crash when locking Claire de Lune, as the game didn't consider this scenario and accesses hLevelSelectPrevArrow, even though it's null.
        if (__instance.m_hMainMenuPages[1].hLevelSelectPrevArrow == null)
        {
            __instance.m_hMainMenuPages[1].hLevelSelectPrevArrow = __instance.m_hMainMenuPages[1].hLevelSelectNextArrow;
        }

        // Change the text describing how levels are unlocked.
        for (int i = 1; i <= 23; i++)
        {
            __instance.m_hMainMenuPages[i].hLevelDesc.LEVEL_LOCKED_TEXT = "Receive Archipelago item to unlock";
        }
        for (int i = 24; i <= 25; i++)
        {
            __instance.m_hMainMenuPages[i].hLevelDesc.LEVEL_LOCKED_TEXT = "Locked when connected to Archipelago";
        }

        return true;
    }

    [HarmonyPatch(typeof(SGMenuHandler), "GotoMenuViewState")]
    [HarmonyPrefix]
    public static bool Prefix_SGMenuHandler_GotoMenuViewState(SGMenuHandler.MENUVIEWSTATE nState, SGMenuHandler __instance)
    {
        if (!Plugin.multiWorld.connected)
        {
            return true;
        }

        // When going from the menu to the level select, if the level the player was last on was locked, the game will
        // fix this by decrementing the level index until it finds an unlocked one. With the randomizer, this may never
        // happen if an earlier level has not been unlocked. Fix this by setting the level index to the last level so
        // the decrementing strategy the game applies will work.
        if (nState == MENUVIEWSTATE.TITLESCREEN_TO_MENU)
        {
            if (!(__instance.m_bFirstTimePrologUnlock || __instance.IsAlbumMode() || __instance.IsBonusMode()))
            {
                int levelIndex = __instance.GetLevelIndexFromUID(SGFW.GameProfile.GetParam(15));
                if (__instance.IsMenuPageLocked(levelIndex))
                {
                    SGFW.GameProfile.SetParam(15, __instance.m_hMainMenuPages[23].hLevelDesc.LEVEL_UID);
                }
            }
        }

        // Make sure we update the level score after getting a new score, even if we didn't get a new high score
        // (compared to base game save file).
        __instance.OnProfileProgressChange();

        return true;
    }

    [HarmonyPatch(typeof(SGGameLogic.MainLogic), "GotoGameViewState")]
    [HarmonyPrefix]
    public static bool Prefix_SGGameLogic_MainLogic_GotoGameViewState(GAMEVIEWSTATE nState, SGGameLogic.MainLogic __instance)
    {
        // Detect deaths.
        switch (nState)
        {
            case GAMEVIEWSTATE.IMPACT:
            case GAMEVIEWSTATE.FALL:
                Plugin.deathLink.OnDeath();
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
        // Detect when the player picks up a coin or clears a level.
        int currentLevelIndex = SGFW.GameLogic().GetCurrentLevelIndex();

        switch (nEventID)
        {
            case SCOREEVENT.SECRETBANANA:
                Plugin.locations.CollectCoin(currentLevelIndex, nUserData);
                break;
            case SCOREEVENT.LEVELCLEAR:
                Plugin.locations.ClearLevel(currentLevelIndex, __instance.m_nScore);
                break;
        }

        return true;
    }

    [HarmonyPatch(typeof(SGScoreHandler), "Reset")]
    [HarmonyPostfix]
    public static void Postfix_SGScoreHandler_Reset(bool bReturnToMenu, bool bClearScore, SGScoreHandler __instance)
    {
        if (!Plugin.multiWorld.connected)
        {
            return;
        }

        int currentLevelIndex = SGFW.GameLogic().GetCurrentLevelIndex();
        if (!bReturnToMenu && bClearScore && currentLevelIndex >= 0 && currentLevelIndex < 23)
        {
            __instance.m_nScore = Plugin.items.GetStartingScore();
            __instance.m_nCheckpointScore = Plugin.items.GetStartingScore();
        }
    }

    [HarmonyPatch(typeof(SGGameProfile), "GetLevelScore")]
    [HarmonyPostfix]
    public static void Postfix_SGGameProfile_GetLevelScore(int nLevelIndex, SGGameProfile __instance, ref int __result)
    {
        if (!Plugin.multiWorld.connected || nLevelIndex < 0 || nLevelIndex >= 23)
        {
            return;
        }

        __result = Plugin.locations.GetScore(nLevelIndex);
    }

    [HarmonyPatch(typeof(SGGameProfile), "GetLevelBananas")]
    [HarmonyPostfix]
    public static void Postfix_SGGameProfile_GetLevelBananas(int nLevelIndex, SGGameProfile __instance, ref int __result)
    {
        if (!Plugin.multiWorld.connected || nLevelIndex < 0 || nLevelIndex >= 23)
        {
            return;
        }

        __result = Plugin.locations.GetCoinsCollected(nLevelIndex);
    }

    [HarmonyPatch(typeof(SGGameProfile), "Save")]
    [HarmonyPrefix]
    public static bool Prefix_SGGameProfile_Save(SGGameProfile __instance)
    {
        Plugin.saveFile.Save();
        return !Plugin.multiWorld.connected;
    }

    // We've repurposed the "Game Center" menu option text for our own message display.
    [HarmonyPatch(typeof(SGMenuText), "OnUpdate", [typeof(float), typeof(float), typeof(bool), typeof(bool)])]
    [HarmonyPrefix]
    public static bool Prefix_SGMenuText_OnUpdate(float fDeltaTime, float fItemBeatScale, bool bShowInProgress, bool bHideInProgress, SGMenuText __instance)
    {
        if (__instance.textTag == "Game Center")
        {
            return false;
        }

        return true;
    }

    [HarmonyPatch(typeof(SGMenuText), "SetColor")]
    [HarmonyPrefix]
    public static bool Prefix_SGMenuText_SetColor(SGMenuText __instance)
    {
        if (__instance.textTag == "Game Center")
        {
            return false;
        }

        return true;
    }
}
