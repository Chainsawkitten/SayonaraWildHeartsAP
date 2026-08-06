from collections.abc import Mapping
from typing import Any, List

from worlds.AutoWorld import WebWorld, World

from .options import SayonaraWildHeartsOptions, sayonara_wild_hearts_option_groups
from .items import SayonaraWildHeartsItem, create_item, create_all_items, ITEM_NAME_TO_ID, FILLER_NAME
from .regions import create_all_regions
from .locations import create_all_locations, LOCATION_NAME_TO_ID
from .rules import set_all_rules

class SayonaraWildHeartsWebWorld(WebWorld):
    tutorials = []
    option_groups = sayonara_wild_hearts_option_groups

class SayonaraWildHeartsWorld(World):
    """
    Sayonara Wild Hearts is a pop album/video game about sword fights, motorbikes, skateboards, tarot and heartbreak. Let's pop!
    """

    game = "Sayonara Wild Hearts"
    web = SayonaraWildHeartsWebWorld()
    options_dataclass = SayonaraWildHeartsOptions
    options: SayonaraWildHeartsOptions

    location_name_to_id = LOCATION_NAME_TO_ID
    item_name_to_id = ITEM_NAME_TO_ID

    def create_regions(self) -> None:
        create_all_regions(self)
        create_all_locations(self)

    def create_item(self, name: str) -> SayonaraWildHeartsItem:
        return create_item(self, name)

    def create_items(self) -> None:
        create_all_items(self)

    def set_rules(self) -> None:
        set_all_rules(self)

    def get_filler_item_name(self) -> str:
        return FILLER_NAME

    def fill_slot_data(self) -> Mapping[str, Any]:
        return {
            "Seed": self.multiworld.seed_name,
            "WorldVersion": self.world_version
        }
