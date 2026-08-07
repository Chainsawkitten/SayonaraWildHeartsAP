from __future__ import annotations

from typing import TYPE_CHECKING

from rule_builder.rules import Has, HasAll, True_
from .levels import levels
from .regions import get_entrance_name

if TYPE_CHECKING:
    from .world import SayonaraWildHeartsWorld

def set_all_rules(world: SayonaraWildHeartsWorld) -> None:
    goal = True_()
    
    for level in levels:
        region = world.get_region(level.name)
        entrance = world.get_entrance(get_entrance_name(level.name))
        world.set_rule(entrance, Has(level.name))
        goal = goal & Has(level.name)
    
    world.set_completion_rule(goal)
