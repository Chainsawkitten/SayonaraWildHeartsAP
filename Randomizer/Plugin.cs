using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SayonaraWildHeartsRandomizer;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInProcess("Sayonara Wild Hearts.exe")]
public class Plugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;
    public static Version randomizerVersion = new Version(MyPluginInfo.PLUGIN_VERSION);
    public static Options options = new();
    public static MultiWorld multiWorld = null;
    public static Locations locations = null;
    public static Items items = null;

    private bool debug = false;
    private KeyboardShortcut deathKey = new(KeyCode.D);

    private struct APInfo
    {
        public string hostname;
        public int port;
        public string slot;
        public string password;
    }

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

        APInfo apInfo = GetAPInfo();

        multiWorld = new MultiWorld(apInfo.hostname, apInfo.port, apInfo.slot, apInfo.password);
        options.Load(multiWorld);
        locations = new Locations(multiWorld);
        items = new Items(multiWorld);
    }

    private void Update()
    {
        items.Update();
        locations.Update();

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

    private string GetAPInfoFileName()
    {
        string gamepath = Application.dataPath;
        if (Application.platform == RuntimePlatform.OSXPlayer)
        {
            gamepath += "/../..";
        }
        else
        {
            gamepath += "/..";
        }

        return gamepath + "/APInfo.json";
    }

    private APInfo GetAPInfo()
    {
        APInfo apInfo = new();

        if (!File.Exists(GetAPInfoFileName()))
        {
            Logger.LogInfo("No APInfo.json file found. Creating a default one.");

            apInfo.hostname = "localhost";
            apInfo.port = 38281;
            apInfo.slot = "";
            apInfo.password = "";

            FileStream fsOut = new(GetAPInfoFileName(), FileMode.OpenOrCreate, FileAccess.Write);
            string jsonOut = JsonConvert.SerializeObject(apInfo);
            StreamWriter swOut = new(fsOut);
            swOut.Write(jsonOut);
            swOut.Close();
            fsOut.Close();
        }

        try
        {
            FileStream fsIn = new(GetAPInfoFileName(), FileMode.Open, FileAccess.Read);
            StreamReader srIn = new(fsIn);
            apInfo = JsonConvert.DeserializeObject<APInfo>(srIn.ReadToEnd());
            srIn.Close();
            fsIn.Close();
        }
        catch (Exception e)
        {
            Logger.LogError("Failed to parse APInfo.json: " + e.ToString());
        }

        if (string.IsNullOrEmpty(apInfo.hostname) || string.IsNullOrEmpty(apInfo.slot))
        {
            Logger.LogInfo("Hostname or slot empty. Check APInfo.json.");
        }

        return apInfo;
    }
}
