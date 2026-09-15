namespace SayonaraWildHeartsRandomizer;

public class Locations
{
    private MultiWorld multiWorld;
    private LocationSender locationSender;
    private MessageDisplay messageDisplay;
    private SaveFile saveFile;

    private const long levelMultiplier = 10;
    private const int levelCount = 23;
    private const int coinCount = 5;
    private bool goaled = false;

    public Locations(MultiWorld multiWorld, MessageDisplay messageDisplay, SaveFile saveFile)
    {
        this.multiWorld = multiWorld;
        this.messageDisplay = messageDisplay;
        locationSender = new LocationSender(multiWorld, messageDisplay);
        this.saveFile = saveFile;
        this.saveFile.Load();

        // Scout all locations to get messages to display when collecting them.
        locationSender.ScoutLocations();

        ResendMissingChecks();
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
        for (int i = 0; i < levelCount; i++)
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

        if (levelIndex < 0 || levelIndex >= levelCount)
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
            locationSender.SendLocationCheckAsync(GetLevelClearLocationID(levelIndex));
            saveFile.levelsCleared[levelIndex] = true;
        }
    }

    public bool IsLevelCleared(int levelIndex)
    {
        if (levelIndex < 0 || levelIndex >= levelCount)
        {
            return false;
        }

        return saveFile.levelsCleared[levelIndex];
    }

    public int GetScore(int levelIndex)
    {
        if (levelIndex < 0 || levelIndex >= levelCount)
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
            locationSender.SendLocationCheckAsync(GetCoinCollectedLocationID(levelIndex, coin));
            saveFile.coinsCollected[levelIndex, coin] = true;
        }
    }

    public int GetCoinsCollected(int levelIndex)
    {
        if (levelIndex < 0 || levelIndex >= levelCount)
        {
            return 0;
        }

        int coins = 0;

        for (int i = 0; i < coinCount; ++i)
        {
            coins |= saveFile.coinsCollected[levelIndex, i] ? (1 << i) : 0;
        }

        return coins;
    }

    public int GetCoinsCollectedCount(int levelIndex)
    {
        if (levelIndex < 0 || levelIndex >= levelCount)
        {
            return 0;
        }

        int coins = 0;

        for (int i = 0; i < coinCount; ++i)
        {
            coins += saveFile.coinsCollected[levelIndex, i] ? 1 : 0;
        }

        return coins;
    }

    private long GetLevelClearLocationID(int levelIndex)
    {
        return (levelIndex + 1) * levelMultiplier;
    }

    private long GetCoinCollectedLocationID(int levelIndex, int coin)
    {
        return (levelIndex + 1) * levelMultiplier + coin + 1;
    }

    private void ResendMissingChecks()
    {
        // Resend missing location checks in case of disconnect.
        for (int levelIndex = 0; levelIndex < levelCount; levelIndex++)
        {
            if (saveFile.levelsCleared[levelIndex])
            {
                long id = GetLevelClearLocationID(levelIndex);
                if (!multiWorld.session.Locations.AllLocationsChecked.Contains(id))
                {
                    Plugin.Logger.LogInfo("Resent level clear " + levelIndex.ToString());
                    locationSender.SendLocationCheckAsync(id, false);
                }
            }

            for (int coin = 0; coin < coinCount; coin++)
            {
                if (saveFile.coinsCollected[levelIndex, coin])
                {
                    long id = GetCoinCollectedLocationID(levelIndex, coin);
                    if (!multiWorld.session.Locations.AllLocationsChecked.Contains(id))
                    {
                        Plugin.Logger.LogInfo("Resent collecting coin " + levelIndex.ToString() + " - " + coin.ToString());
                        locationSender.SendLocationCheckAsync(id, false);
                    }
                }
            }
        }
    }
}
