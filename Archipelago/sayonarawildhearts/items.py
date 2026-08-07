from __future__ import annotations

from typing import TYPE_CHECKING

from BaseClasses import Item, ItemClassification
from .levels import levels

if TYPE_CHECKING:
    from .world import SayonaraWildHeartsWorld

FILLER_NAME = "10 Bonus Points"

ITEM_NAME_TO_ID : Dict[str, int] = {
    FILLER_NAME: 50,
}

ITEM_NAME_TO_CLASSIFICATION : Dict[str, ItemClassification] = {
    FILLER_NAME: ItemClassification.filler,
}

for level in levels:
    ITEM_NAME_TO_ID[level.name] = level.index
    ITEM_NAME_TO_CLASSIFICATION[level.name] = ItemClassification.progression

class SayonaraWildHeartsItem(Item):
    game = "Sayonara Wild Hearts"

def create_item(world: SayonaraWildHeartsWorld, name: str) -> SayonaraWildHeartsItem:
    return SayonaraWildHeartsItem(name, ITEM_NAME_TO_CLASSIFICATION[name], ITEM_NAME_TO_ID[name], world.player)

def create_all_items(world: SayonaraWildHeartsWorld) -> None:
    # Start with one level unlocked.
    starting_level = world.random.choice(levels)
    world.multiworld.push_precollected(world.create_item(starting_level.name))
    
    # Add remaining level unlocks to the item pool.
    itempool: list[Item] = []
    
    for level in levels:
        if level != starting_level:
            itempool.append(world.create_item(level.name))
    
    number_of_items = len(itempool)
    number_of_unfilled_locations = len(world.multiworld.get_unfilled_locations(world.player))
    needed_number_of_filler_items = number_of_unfilled_locations - number_of_items
    itempool += [world.create_filler() for _ in range(needed_number_of_filler_items)]

    world.multiworld.itempool += itempool
