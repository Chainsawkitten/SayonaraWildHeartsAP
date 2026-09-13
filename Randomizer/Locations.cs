using Newtonsoft.Json;
using System;
using System.IO;
using UnityEngine;

namespace SayonaraWildHeartsRandomizer;

public class Locations
{
    private MultiWorld multiWorld;
    private LocationSender locationSender;
    private MessageDisplay messageDisplay;

    private bool[] levelsCleared = new bool[23];
    private int[] levelScores = new int[23];
    private bool[,] coinsCollected = new bool[23, 5];

    private const long levelMultiplier = 10;
    private bool goaled = false;

    public Locations(MultiWorld multiWorld, MessageDisplay messageDisplay)
    {
        this.multiWorld = multiWorld;
        this.messageDisplay = messageDisplay;
        this.locationSender = new LocationSender(multiWorld);
        Load();
    }

    public void Update()
    {
        CheckGoal();
    }

    public void CheckGoal()
    {
        if (!multiWorld.connected || goaled)
        {
            return;
        }

        // The goal is getting the required rank in all levels.
        for (int i = 0; i < 23; i++)
        {
            if (!IsLevelCleared(i))
            {
                return;
            }
        }

        Plugin.Logger.LogInfo("Goal completed.");
        goaled = true;
        multiWorld.session.SetGoalAchieved();
        messageDisplay.QueueMessage("Victory!");
    }

    public void ClearLevel(int levelIndex, int score)
    {
        if (!multiWorld.connected)
        {
            return;
        }

        if (levelIndex < 0 || levelIndex >= 23)
        {
            return;
        }

        if (score > levelScores[levelIndex])
        {
            levelScores[levelIndex] = score;
        }

        int targetScore = SGFW.GameProfile.GetLevelRankScore(levelIndex, ((int)Plugin.options.RequiredRank) - 1);
        if (score < targetScore)
        {
            return;
        }

        if (!levelsCleared[levelIndex])
        {
            Plugin.Logger.LogInfo("Level clear " + levelIndex.ToString());

            long locationID = (levelIndex + 1) * levelMultiplier;
            locationSender.SendLocationCheckAsync(locationID);
            levelsCleared[levelIndex] = true;
        }
    }

    public bool IsLevelCleared(int levelIndex)
    {
        if (levelIndex < 0 || levelIndex >= 23)
        {
            return false;
        }

        return levelsCleared[levelIndex];
    }

    public int GetScore(int levelIndex)
    {
        if (levelIndex < 0 || levelIndex >= 23)
        {
            return 0;
        }

        return levelScores[levelIndex];
    }

    public void CollectCoin(int levelIndex, int coin)
    {
        if (!multiWorld.connected)
        {
            return;
        }

        if (!coinsCollected[levelIndex, coin])
        {
            Plugin.Logger.LogInfo("Collected secret banana " + levelIndex.ToString() + " - " + coin.ToString());

            long locationID = (levelIndex + 1) * levelMultiplier + coin + 1;
            locationSender.SendLocationCheckAsync(locationID);
            coinsCollected[levelIndex, coin] = true;
        }
    }

    public int GetCoinsCollected(int levelIndex)
    {
        if (levelIndex < 0 || levelIndex >= 23)
        {
            return 0;
        }

        int coins = 0;

        for (int i = 0; i < 5; ++i)
        {
            coins |= coinsCollected[levelIndex, i] ? (1 << i) : 0;
        }

        return coins;
    }

    public int GetCoinsCollectedCount(int levelIndex)
    {
        if (levelIndex < 0 || levelIndex >= 23)
        {
            return 0;
        }

        int coins = 0;

        for (int i = 0; i < 5; ++i)
        {
            coins += coinsCollected[levelIndex, i] ? 1 : 0;
        }

        return coins;
    }

    private struct SaveData
    {
        public bool[] levelsCleared;
        public int[] levelScores;
        public bool[,] coinsCollected;
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
        catch(Exception e)
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
