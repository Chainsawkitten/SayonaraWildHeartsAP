using Archipelago.MultiClient.Net.Models;

namespace SayonaraWildHeartsRandomizer;

public class Items
{
    private MultiWorld multiWorld;

    private bool[] levelsUnlocked = new bool[23];

    public Items(MultiWorld multiWorld)
    {
        this.multiWorld = multiWorld;
    }

    public bool IsLevelLocked(int levelIndex)
    {
        if (levelIndex < 0 || levelIndex > 22)
        {
            return false;
        }

        return !levelsUnlocked[levelIndex];
    }

    public void Update()
    {
        if (!multiWorld.connected)
        {
            return;
        }

        while (multiWorld.session.Items.Any())
        {
            ItemInfo itemInfo = multiWorld.session.Items.DequeueItem();

            // TODO Show some kind of in-game message.
            Plugin.Logger.LogInfo("Received " + itemInfo.ItemName + " from " + itemInfo.Player);

            if (itemInfo.ItemId >= 1 && itemInfo.ItemId <= 23)
            {
                levelsUnlocked[itemInfo.ItemId - 1] = true;
            }
        }
    }
}
