using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace SayonaraWildHeartsRandomizer;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInProcess("Sayonara Wild Hearts.exe")]
public class Plugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;

    private void Awake()
    {
        // Plugin startup logic
        Logger = base.Logger;
        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");

        Harmony.CreateAndPatchAll(typeof(Plugin));
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
}
