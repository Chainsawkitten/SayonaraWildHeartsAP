from __future__ import annotations

from typing import TYPE_CHECKING

from BaseClasses import Item, ItemClassification
from rule_builder.rules import Has

if TYPE_CHECKING:
    from .world import SayonaraWildHeartsWorld

class LevelData():
    name: str
    index: int
    coins : int
    
    def __init__(self, name, index, coins):
        self.name = name
        self.index = index
        self.coins = coins

levels = [
    LevelData("Claire de Lune", index=1, coins=2),
    LevelData("Heartbreak I", index=2, coins=2),
    LevelData("Doki Doki Rush", index=3, coins=4),
    LevelData("Fighting Hearts", index=4, coins=3),
    LevelData("Begin Again", index=5, coins=3),
    LevelData("Heartbreak II", index=6, coins=3),
    LevelData("Forest Ghost", index=7, coins=2),
    LevelData("Forest Dub", index=8, coins=1),
    LevelData("Laser Love", index=9, coins=2),
    LevelData("Dead of Night", index=10, coins=4),
    LevelData("Heartbreak III", index=11, coins=2),
    LevelData("Hearts & Swords", index=12, coins=4),
    LevelData("Parallel Universes", index=13, coins=3),
    LevelData("Mine", index=14, coins=3),
    LevelData("Heartbreak IV", index=15, coins=3),
    LevelData("Night Drift", index=16, coins=3),
    LevelData("Reverie", index=17, coins=4),
    LevelData("The World We Knew", index=18, coins=3),
    LevelData("Heartbreak V", index=19, coins=3),
    LevelData("Transonic Gravity", index=20, coins=3),
    LevelData("Hate Skulls", index=21, coins=2),
    LevelData("Inside", index=22, coins=4),
    LevelData("Wild Hearts Never Die", index=23, coins=5),
]
