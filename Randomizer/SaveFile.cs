using Newtonsoft.Json;
using System;
using System.IO;
using UnityEngine;

namespace SayonaraWildHeartsRandomizer;

public class SaveFile
{
    private MultiWorld multiWorld;

    public bool[] levelsCleared = new bool[23];
    public int[] levelScores = new int[23];
    public bool[,] coinsCollected = new bool[23, 5];

    private struct SaveData
    {
        public bool[] levelsCleared;
        public int[] levelScores;
        public bool[,] coinsCollected;
    }

    public SaveFile(MultiWorld multiWorld)
    {
        this.multiWorld = multiWorld;
    }

    public void Save()
    {
        if (!Plugin.multiWorld.connected)
        {
            return;
        }

        try
        {
            FileStream fsOut = new(GetSaveFileName(), FileMode.OpenOrCreate, FileAccess.Write);
            fsOut.SetLength(0);

            SaveData saveData = new SaveData();

            saveData.levelsCleared = levelsCleared;
            saveData.levelScores = levelScores;
            saveData.coinsCollected = coinsCollected;

            string json = JsonConvert.SerializeObject(saveData);
            StreamWriter swOut = new(fsOut);
            swOut.Write(json);
            swOut.Close();
            fsOut.Close();
        }
        catch (Exception e)
        {
            Plugin.Logger.LogError("Failed to save file " + e.ToString());
        }
    }

    public void Load()
    {
        if (!Plugin.multiWorld.connected)
        {
            return;
        }

        if (!File.Exists(GetSaveFileName()))
        {
            return;
        }

        try
        {
            FileStream fsIn = new(GetSaveFileName(), FileMode.Open, FileAccess.Read);
            StreamReader srIn = new(fsIn);

            SaveData saveData = JsonConvert.DeserializeObject<SaveData>(srIn.ReadToEnd());
            levelsCleared = saveData.levelsCleared;
            levelScores = saveData.levelScores;
            coinsCollected = saveData.coinsCollected;

            srIn.Close();
            fsIn.Close();
        }
        catch (Exception e)
        {
            Plugin.Logger.LogError("Failed to parse json: " + e.ToString());
            return;
        }
    }

    private string GetSaveFileName()
    {
        string seed = multiWorld.slotData["Seed"].ToString();
        string slot = multiWorld.session.ConnectionInfo.Slot.ToString();
        return Application.persistentDataPath + "/AP_" + seed + "_" + slot + ".sav";
    }
}
