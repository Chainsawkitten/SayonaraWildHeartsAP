from __future__ import annotations

from typing import TYPE_CHECKING

from BaseClasses import Entrance, Region
from .levels import levels

if TYPE_CHECKING:
    from .world import SayonaraWildHeartsWorld

def get_entrance_name(name: str):
    return name + " Entrance"

def create_all_regions(world: SayonaraWildHeartsWorld) -> None:
    menu = Region("Menu", world.player, world.multiworld)
    
    regions = [
        menu,
    ]
    
    for level in levels:
        region = Region(level.name, world.player, world.multiworld)
        regions.append(region)
    
    world.multiworld.regions += regions
    
    # Connect regions
    for level in levels:
        region = world.get_region(level.name)
        menu.connect(region, get_entrance_name(level.name))
