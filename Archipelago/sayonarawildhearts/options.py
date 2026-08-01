from dataclasses import dataclass

from Options import Choice, OptionGroup, PerGameCommonOptions, Range, Toggle

class DummyOption(Toggle):
    """
    Dummy option
    """
    display_name = "Dummy Option"

@dataclass
class SayonaraWildHeartsOptions(PerGameCommonOptions):
    dummy_option: DummyOption

sayonara_wild_hearts_option_groups = [
    OptionGroup("Game Options", [
        DummyOption
    ])
]
