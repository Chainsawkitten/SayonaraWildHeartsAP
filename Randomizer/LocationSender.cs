using Archipelago.MultiClient.Net.Exceptions;
using Archipelago.MultiClient.Net.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace SayonaraWildHeartsRandomizer;

// c-wspp-websocket-sharp does not actually do SendAsync async, so we need to implement our own threading.
public class LocationSender
{
    private static MultiWorld multiWorld;
    private static Mutex mutex = new Mutex();
    private static Queue<long> locations = new Queue<long>();
    private Thread thread;
    private MessageDisplay messageDisplay;
    private Dictionary<long, string> locationMessages = new Dictionary<long, string>();

    public LocationSender(MultiWorld multiWorld, MessageDisplay messageDisplay)
    {
        LocationSender.multiWorld = multiWorld;
        this.messageDisplay = messageDisplay;

        thread = new Thread(ProcessChecks);
        thread.IsBackground = true;
        thread.Start();
    }
         
    public void SendLocationCheckAsync(long locationID, bool displayMessage = true)
    {
        mutex.WaitOne();
        locations.Enqueue(locationID);
        mutex.ReleaseMutex();

        if (displayMessage)
        {
            string message = "Found AP item";
            if (locationMessages.ContainsKey(locationID))
            {
                message = locationMessages[locationID];
            }
            messageDisplay.QueueMessage(message);
        }
    }

    private static void ProcessChecks()
    {
        List<long> locationIDs = new List<long>();

        while (true)
        {
            // Poll new locations to check.
            mutex.WaitOne();

            while (locations.Count > 0)
            {
                locationIDs.Add(locations.Dequeue());
            }

            mutex.ReleaseMutex();

            // Send all the locations.
            if (locationIDs.Count > 0)
            {
                try
                {
                    multiWorld.session.Locations.CompleteLocationChecks(locationIDs.ToArray());
                }
                catch (ArchipelagoSocketClosedException e)
                {
                    Plugin.Logger.LogError("Tried to send check but server connection was closed.");
                    Plugin.Logger.LogError(e.ToString());
                }

                locationIDs.Clear();
            }

            Thread.Sleep(10);
        }
    }

    public void ScoutLocations()
    {
        try
        {
            multiWorld.session.Locations.ScoutLocationsAsync(ScoutLocationCallback, multiWorld.session.Locations.AllLocations.ToArray());
        }
        catch (Exception e)
        {
            Plugin.Logger.LogError(e.ToString());
        }
    }

    private void ScoutLocationCallback(Dictionary<long, ScoutedItemInfo> itemInfos)
    {
        foreach (KeyValuePair<long, ScoutedItemInfo> itemInfo in itemInfos)
        {
            string message = "Sent " + itemInfo.Value.ItemName + " to " + itemInfo.Value.Player.Name;
            if (itemInfo.Value.Player.Slot == multiWorld.session.ConnectionInfo.Slot)
            {
                message = "Found your " + itemInfo.Value.ItemName;
            }
            locationMessages[itemInfo.Key] = message;
        }
    }
}
