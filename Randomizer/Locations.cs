using Archipelago.MultiClient.Net.Exceptions;

namespace SayonaraWildHeartsRandomizer;

public class Locations
{
    private MultiWorld multiWorld;

    private bool[] levelsCleared = new bool[23];
    private bool[,] coinsCollected = new bool[23, 5];

    private const long levelMultiplier = 10;

    public Locations(MultiWorld multiWorld)
    {
        this.multiWorld = multiWorld;
    }

    public void ClearLevel(int levelIndex)
    {
        if (!multiWorld.connected)
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
}
