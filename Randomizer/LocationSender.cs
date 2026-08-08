using Archipelago.MultiClient.Net.Exceptions;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace SayonaraWildHeartsRandomizer;

// c-wspp-websocket-sharp does not actually do SendAsync async, so we need to implement our own threading.
public class LocationSender
{
    private static MultiWorld multiWorld;
    private static Mutex mutex = new Mutex();
    private static Queue<long> locations = new Queue<long>();
    private Thread thread;

    public LocationSender(MultiWorld multiWorld)
    {
        LocationSender.multiWorld = multiWorld;

        thread = new Thread(ProcessChecks);
        thread.IsBackground = true;
        thread.Start();
    }
         
    public void SendLocationCheckAsync(long locationID)
    {
        mutex.WaitOne();
        locations.Enqueue(locationID);
        mutex.ReleaseMutex();
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
}
