using Archipelago.MultiClient.Net.BounceFeatures.DeathLink;
using System;
using UnityEngine;

namespace SayonaraWildHeartsRandomizer;

public class DeathLink
{
    private MultiWorld multiWorld;
    private MessageDisplay messageDisplay;
    private Options options;
    private DeathLinkService deathLinkService = null;

    private bool ignoreDeaths = false;
    private int deathCount = 0;

    public DeathLink(MultiWorld multiWorld, MessageDisplay messageDisplay, Options options)
    {
        this.multiWorld = multiWorld;
        this.messageDisplay = messageDisplay;
        this.options = options;

        if (multiWorld.connected && options.EnableDeathLink)
        {
            Plugin.Logger.LogInfo("Enabling death link");
            deathLinkService = DeathLinkProvider.CreateDeathLinkService(multiWorld.session);
            deathLinkService.EnableDeathLink();
            deathLinkService.OnDeathLinkReceived += this.DeathLinkReceivedCallback;
        }
    }

    public void OnDeath()
    {
        if (!multiWorld.connected || ignoreDeaths || !options.EnableDeathLink)
        {
            return;
        }

        deathCount++;
        if (deathCount >= options.DeathLinkAmnesty)
        {
            deathCount = 0;
            Plugin.Logger.LogInfo("Death link sent");
            string playerName = multiWorld.session.Players.ActivePlayer.Alias;
            deathLinkService.SendDeathLink(new Archipelago.MultiClient.Net.BounceFeatures.DeathLink.DeathLink(playerName, playerName + " lost their groove."));
        }
    }

    private void DeathLinkReceivedCallback(Archipelago.MultiClient.Net.BounceFeatures.DeathLink.DeathLink deathLink)
    {
        Plugin.Logger.LogInfo("Death link received");
        string message = deathLink.Cause != null ? deathLink.Cause : (deathLink.Source + " died.");
        messageDisplay.QueueMessage(message);

        deathCount = 0;

        // Trigger death.
        if (SGFW.IsGameLogicPresent())
        {
            SGGameLogic gameLogic = SGFW.GameLogic();

            if (Time.timeScale > 0.0001f)
            {
                ignoreDeaths = true;
                try
                {
                    gameLogic.OnActiveClipRespawn(false);
                }
                catch (Exception)
                {
                }
                ignoreDeaths = false;
            }
        }
    }
}
