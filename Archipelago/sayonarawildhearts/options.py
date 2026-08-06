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
    default = 3

@dataclass
class SayonaraWildHeartsOptions(PerGameCommonOptions):
    RequiredRank: RequiredRank

sayonara_wild_hearts_option_groups = [
    OptionGroup("Game Options", [
        RequiredRank
    ])
]
