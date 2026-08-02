using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Rewired.Platforms.Switch.NintendoSwitchInputManager;
using static SGGameLogic;
using static SGScoreHandler;

namespace SayonaraWildHeartsRandomizer;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInProcess("Sayonara Wild Hearts.exe")]
public class Plugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;

    private bool debug = true;
    private KeyboardShortcut deathKey = new(KeyCode.D);

    private void Awake()
    {
        // Plugin startup logic
        Logger = base.Logger;
        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");

        try
        {
            Harmony.CreateAndPatchAll(typeof(Plugin));
        }
        catch (Exception e)
        {
            Logger.LogError(e.Message);
        }

        SceneManager.sceneLoaded += OnSceneChange;
    }

    private void Update()
    {
        // Debug key to trigger a death.
        if (deathKey.IsPressed() && debug)
        {

        }
    }

    private void OnSceneChange(Scene scene, LoadSceneMode mode)
    {
        Logger.LogMessage("Scene changed: " + scene.name);

        /*Logger.LogMessage("Game objects");
        GameObject[] rootObjects = scene.GetRootGameObjects();
        foreach (GameObject gameObject in rootObjects)
        {
            PrintGameObject(gameObject);
        }*/
    }

    private void PrintGameObject(GameObject gameObject, int depth = 0)
    {
        string details = "";
        for (int i = 0; i < depth; i++)
        {
            details += " ";
        }
        details += gameObject.name;
        details += " - ";

        MonoBehaviour[] scripts = gameObject.GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour script in scripts)
        {
            details += script.GetScriptClassName() + ", ";
        }

        Logger.LogMessage(details);

        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            PrintGameObject(gameObject.transform.GetChild(i).gameObject, depth + 1);
        }
    }

    [HarmonyPatch(typeof(SGMenuHandler), "UpdateLevelPageIndicators")]
    [HarmonyPrefix]
    public static bool Prefix_SGMenuHandler_UpdateLevelPageIndicators(SGMenuHandler.MENUPAGE hMenuPage, SGMenuHandler __instance) {
        Logger.LogInfo("UpdateLevelPageIndicators");
        return true;
    }

    [HarmonyPatch(typeof(SGMenuHandler), "UnlockNextLevel")]
    [HarmonyPrefix]
    public static bool Prefix_SGMenuHandler_UnlockNextLevel(SGMenuHandler __instance)
    {
        Logger.LogInfo("UnlockNextLevel");
        return true;
    }

    [HarmonyPatch(typeof(SGGameLogic.MainLogic), "GotoGameViewState")]
    [HarmonyPrefix]
    public static bool Prefix_SGGameLogic_MainLogic_GotoGameViewState(GAMEVIEWSTATE nState, SGGameLogic.MainLogic __instance)
    {
        switch (nState)
        {
            case GAMEVIEWSTATE.LEVELEND:
                Logger.LogInfo("Level end");
                break;
            case GAMEVIEWSTATE.IMPACT:
            case GAMEVIEWSTATE.FALL:
                Logger.LogInfo("Death");
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
        switch (nEventID)
        {
            case SCOREEVENT.SECRETBANANA:
                Logger.LogInfo("Collected secret banana");
                break;
            case SCOREEVENT.RESPAWN:
                Logger.LogInfo("Respawn");
                break;
            case SCOREEVENT.LEVELCLEAR:
                Logger.LogInfo("Level clear");
                break;
        }

        return true;
    }
}
