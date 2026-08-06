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

    public void Load(MultiWorld multiWorld)
    {
        JObject options;
        try
        {
            options = (JObject) multiWorld.slotData["Options"];
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
    }
}
