using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SayonaraWildHeartsRandomizer;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInProcess("Sayonara Wild Hearts.exe")]
public class Plugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;
    public static MultiWorld multiWorld = null;

    private bool debug = true;
    private KeyboardShortcut deathKey = new(KeyCode.D);

    private void Awake()
    {
        // Plugin startup logic
        Logger = base.Logger;
        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");

        try
        {
            Harmony.CreateAndPatchAll(typeof(Hooks));
        }
        catch (Exception e)
        {
            Logger.LogError(e.Message);
        }

        SceneManager.sceneLoaded += OnSceneChange;

        // TODO: Don't hardcode login info.
        multiWorld = new MultiWorld("localhost", 38281, "Sayonara", "");
    }

    private void Update()
    {
        // Debug key to trigger a death.
        if (deathKey.IsPressed() && debug)
        {
            // TODO Trigger death
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
}
