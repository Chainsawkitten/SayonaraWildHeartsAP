using Newtonsoft.Json.Linq;

namespace SayonaraWildHeartsRandomizer;

public class Options
{
    // Rank required to clear a level.
    public enum ERequiredRank
    {
        Gold = 1,
        Silver = 2,
        Bronze = 3,
    }
    public ERequiredRank RequiredRank = ERequiredRank.Gold;

    // When you die, everyone who enabled death link dies. Of course, the reverse is true too.
    public bool EnableDeathLink = false;

    // How many deaths it takes to send a DeathLink.
    public int DeathLinkAmnesty = 1;

    public void Load(MultiWorld multiWorld)
    {
        JObject options;
        try
        {
            options = (JObject)multiWorld.slotData["Options"];
        }
        catch
        {
            return;
        }

        Plugin.Logger.LogInfo(options.ToString());

        if (options["RequiredRank"] != null)
        {
            RequiredRank = (ERequiredRank)int.Parse(options["RequiredRank"].ToString());
        }

        if (options["EnableDeathLink"] != null)
        {
            EnableDeathLink = int.Parse(options["EnableDeathLink"].ToString()) == 1;
        }

        if (options["DeathLinkAmnesty"] != null)
        {
            DeathLinkAmnesty = int.Parse(options["DeathLinkAmnesty"].ToString());
        }
    }
}
