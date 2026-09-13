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
    private SaveFile saveFile;

    private const long levelMultiplier = 10;
    private bool goaled = false;

    public Locations(MultiWorld multiWorld, MessageDisplay messageDisplay, SaveFile saveFile)
    {
        this.multiWorld = multiWorld;
        this.messageDisplay = messageDisplay;
        this.locationSender = new LocationSender(multiWorld);
        this.saveFile = saveFile;
        this.saveFile.Load();
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

        if (score > saveFile.levelScores[levelIndex])
        {
            saveFile.levelScores[levelIndex] = score;
        }

        int targetScore = SGFW.GameProfile.GetLevelRankScore(levelIndex, ((int)Plugin.options.RequiredRank) - 1);
        if (score < targetScore)
        {
            return;
        }

        if (!saveFile.levelsCleared[levelIndex])
        {
            Plugin.Logger.LogInfo("Level clear " + levelIndex.ToString());

            long locationID = (levelIndex + 1) * levelMultiplier;
            locationSender.SendLocationCheckAsync(locationID);
            saveFile.levelsCleared[levelIndex] = true;
        }
    }

    public bool IsLevelCleared(int levelIndex)
    {
        if (levelIndex < 0 || levelIndex >= 23)
        {
            return false;
        }

        return saveFile.levelsCleared[levelIndex];
    }

    public int GetScore(int levelIndex)
    {
        if (levelIndex < 0 || levelIndex >= 23)
        {
            return 0;
        }

        return saveFile.levelScores[levelIndex];
    }

    public void CollectCoin(int levelIndex, int coin)
    {
        if (!multiWorld.connected)
        {
            return;
        }

        if (!saveFile.coinsCollected[levelIndex, coin])
        {
            Plugin.Logger.LogInfo("Collected secret banana " + levelIndex.ToString() + " - " + coin.ToString());

            long locationID = (levelIndex + 1) * levelMultiplier + coin + 1;
            locationSender.SendLocationCheckAsync(locationID);
            saveFile.coinsCollected[levelIndex, coin] = true;
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
            coins |= saveFile.coinsCollected[levelIndex, i] ? (1 << i) : 0;
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
            coins += saveFile.coinsCollected[levelIndex, i] ? 1 : 0;
        }

        return coins;
    }
}
