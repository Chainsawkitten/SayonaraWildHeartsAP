from __future__ import annotations

from typing import TYPE_CHECKING

from BaseClasses import Region, Location
from .levels import levels

if TYPE_CHECKING:
    from .world import SayonaraWildHeartsWorld

class SayonaraWildHeartsLocation(Location):
    game = "Sayonara Wild Hearts"

def get_clear_location_name(name: str) -> str:
    return name + " - Clear"

def get_coin_location_name(name: str, coin: int) -> str:
    return name + " - Coin " + str(coin + 1)

LOCATION_NAME_TO_ID : Dict[str, int] = {
}

for level in levels:
    LOCATION_NAME_TO_ID[get_clear_location_name(level.name)] = level.index * 10
    
    for i in range(level.coins):
        LOCATION_NAME_TO_ID[get_coin_location_name(level.name, i)] = level.index * 10 + i + 1

def create_all_locations(world: SayonaraWildHeartsWorld) -> None:
    for level in levels:
        region : Region = world.get_region(level.name)
        
        # Clearing the level.
        location_name = get_clear_location_name(level.name)
        region.locations.append(SayonaraWildHeartsLocation(world.player, location_name, LOCATION_NAME_TO_ID[location_name], region))
        
        # Collecting the square coins.
        for i in range(level.coins):
            location_name = get_coin_location_name(level.name, i)
            region.locations.append(SayonaraWildHeartsLocation(world.player, location_name, LOCATION_NAME_TO_ID[location_name], region))
