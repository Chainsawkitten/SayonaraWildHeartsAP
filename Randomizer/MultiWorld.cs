using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Enums;
using System;
using System.Collections.Generic;

namespace SayonaraWildHeartsRandomizer;

public class MultiWorld
{
    public ArchipelagoSession session;
    public bool connected = false;
    public Dictionary<string, object> slotData;

    public MultiWorld(string hostname, int port, string slot, string password)
    {
        Plugin.Logger.LogMessage("Connecting to Archipelago server...");

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

            // TODO: Check version compatibility
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

            return; // Did not connect, show the user the contents of `errorMessage`
        }
    }
}
