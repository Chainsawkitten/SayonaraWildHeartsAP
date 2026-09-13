using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Enums;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace SayonaraWildHeartsRandomizer;

public class MultiWorld
{
    public ArchipelagoSession session;
    public bool connected = false;
    public Dictionary<string, object> slotData;
    private MessageDisplay messageDisplay;

    public MultiWorld(string hostname, int port, string slot, string password, MessageDisplay messageDisplay)
    {
        Plugin.Logger.LogMessage("Connecting to Archipelago server...");
        this.messageDisplay = messageDisplay;

        if (hostname != "localhost")
        {
            session = ArchipelagoSessionFactory.CreateSession("wss://" + hostname, port);
        }
        else
        {
            session = ArchipelagoSessionFactory.CreateSession(hostname, port);
        }

        LoginResult result;
        try
        {
            result = session.TryConnectAndLogin("Sayonara Wild Hearts", slot, ItemsHandlingFlags.AllItems, password: password);
            slotData = session.DataStorage.GetSlotData();

            // Check if we are compatible with the AP world version.
            if (!slotData.ContainsKey("WorldVersion"))
            {
                throw new Exception("Slot data did not contain WorldVersion field");
            }
            JArray versionArray = (JArray)slotData["WorldVersion"];
            Version worldVersion = new Version((int)versionArray[0], (int)versionArray[1], (int)versionArray[2]);

            if (worldVersion.Major < Plugin.randomizerVersion.Major)
            {
                throw new Exception("AP world version " + worldVersion + " is incompatible with randomizer version " + Plugin.randomizerVersion + ". Please update the AP world and regenerate (or downgrade the randomizer).");
            }
            else if (worldVersion.Major > Plugin.randomizerVersion.Major || worldVersion.Minor > Plugin.randomizerVersion.Minor)
            {
                throw new Exception("AP world version " + worldVersion + " is newer than randomizer version " + Plugin.randomizerVersion + ". Please update the randomizer.");
            }
        }
        catch (Exception e)
        {
            result = new LoginFailure(e.GetBaseException().Message);
            connected = false;
        }

        if (result.Successful)
        {
            connected = true;

            Plugin.Logger.LogMessage("Connected to Archipelago server as " + slot);
            messageDisplay.QueueMessage("Connected to Archipelago");
        }
        else
        {
            LoginFailure failure = (LoginFailure)result;
            string errorMessage = $"Failed to Connect to Archipelago server as " + slot;
            foreach (string error in failure.Errors)
            {
                errorMessage += $"\n    {error}";
            }
            foreach (ConnectionRefusedError error in failure.ErrorCodes)
            {
                errorMessage += $"\n    {error}";
            }

            connected = false;

            Plugin.Logger.LogError(errorMessage);
            messageDisplay.QueueMessage("Failed to connect to Archipelago. Check APInfo.json");

            return; // Did not connect, show the user the contents of `errorMessage`
        }
    }
}
