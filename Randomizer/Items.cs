using Archipelago.MultiClient.Net.Models;

namespace SayonaraWildHeartsRandomizer;

public class Items
{
    private MultiWorld multiWorld;
    private MessageDisplay messageDisplay;

    private bool[] levelsUnlocked = new bool[23];
    private int bonusPoints = 0;

    const long ITEM_ID_10_BONUS_POINTS = 50;

    public Items(MultiWorld multiWorld, MessageDisplay messageDisplay)
    {
        this.multiWorld = multiWorld;
        this.messageDisplay = messageDisplay;
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

            string receivedMessage = "Received " + itemInfo.ItemName + " from " + itemInfo.Player;
            messageDisplay.QueueMessage(receivedMessage);
            Plugin.Logger.LogInfo(receivedMessage);

            if (itemInfo.ItemId >= 1 && itemInfo.ItemId <= 23)
            {
                levelsUnlocked[itemInfo.ItemId - 1] = true;
            }

            if (itemInfo.ItemId == ITEM_ID_10_BONUS_POINTS)
            {
                bonusPoints += 10;
            }

            // Update page indicators to show any newly unlocked level.
            if (SGFW.IsGameLogicPresent())
            {
                SGMenuHandler menuHandler = ((SGGameLogic.MainLogic)SGFW.GameLogic()).m_hMenuHandler;
                if (menuHandler != null)
                {
                    menuHandler.UpdateAllPageIndicators();
                }
            }
        }
    }

    public int GetStartingScore()
    {
        return bonusPoints;
    }
}
