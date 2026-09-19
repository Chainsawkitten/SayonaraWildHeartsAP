from dataclasses import dataclass

from Options import Choice, OptionGroup, PerGameCommonOptions, Range, Toggle

class RequiredRank(Choice):
    """
    Rank required to clear a level.
    """
    display_name = "Required Rank"
    option_gold = 1
    option_silver = 2
    option_bronze = 3
    default = 1

class EnableDeathLink(Toggle):
    """
    When you die, everyone who enabled death link dies. Of course, the reverse is true too.
    """
    display_name = "Death Link"

class DeathLinkAmnesty(Range):
    """
    How many deaths it takes to send a DeathLink.
    """
    display_name = "Death Link Amnesty"
    range_start = 1
    range_end = 10
    default = 1

@dataclass
class SayonaraWildHeartsOptions(PerGameCommonOptions):
    RequiredRank: RequiredRank
    EnableDeathLink: EnableDeathLink
    DeathLinkAmnesty: DeathLinkAmnesty

sayonara_wild_hearts_option_groups = [
    OptionGroup("Game Options", [
        RequiredRank,
        EnableDeathLink,
        DeathLinkAmnesty
    ])
]
