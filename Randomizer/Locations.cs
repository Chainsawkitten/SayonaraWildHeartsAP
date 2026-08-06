using Archipelago.MultiClient.Net.Exceptions;

namespace SayonaraWildHeartsRandomizer;

public class Locations
{
    private MultiWorld multiWorld;


    private bool[] levelsCleared = new bool[23];
    private int[] levelScores = new int[23];
    private bool[,] coinsCollected = new bool[23, 5];

    private const long levelMultiplier = 10;

    public Locations(MultiWorld multiWorld)
    {
        this.multiWorld = multiWorld;
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

        // TODO Required rank.
        int targetScore = 0;
        if (score < targetScore)
        {
            return;
        }

        if (!levelsCleared[levelIndex])
        {
            Plugin.Logger.LogInfo("Level clear " + levelIndex.ToString());

            try
            {
                long locationID = (levelIndex + 1) * levelMultiplier;
                multiWorld.session.Locations.CompleteLocationChecks([locationID]);
                levelsCleared[levelIndex] = true;
            }
            catch (ArchipelagoSocketClosedException e)
            {
                Plugin.Logger.LogError("Tried to send check but server connection was closed.");
                Plugin.Logger.LogError(e.ToString());
            }
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

            try
            {
                long locationID = (levelIndex + 1) * levelMultiplier + coin + 1;
                multiWorld.session.Locations.CompleteLocationChecks([locationID]);
                coinsCollected[levelIndex, coin] = true;
            }
            catch (ArchipelagoSocketClosedException e)
            {
                Plugin.Logger.LogError("Tried to send check but server connection was closed.");
                Plugin.Logger.LogError(e.ToString());
            }
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
}
