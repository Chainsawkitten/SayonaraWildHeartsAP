# Sayonara Wild Hearts Archipelago Randomizer

[Archipelago](https://archipelago.gg/) implementation for [Sayonara Wild Hearts](https://store.steampowered.com/app/1122720/Sayonara_Wild_Hearts/).

## What's randomized?

### Goal
Achieve a Gold Rank on every level.

### Items
- Level unlocks
- Bonus points (to help get that Gold Rank)

### Locations
- Gold Ranks
- Collecting square coins

## Installing the randomizer
1. Download the x64 version of BepInEx from https://github.com/BepInEx/BepInEx/releases/tag/v5.4.23.5
1. Extract the zip file and copy the contents into Sayonara Wild Hearts' install directory. (To find the install directory: in Steam, go Manage->Browse local files).
1. Run Sayonara Wild Hearts once so BepInEx creates the necessary files.
1. Download the latest release of the randomizer, extract it in the `BepInEx/plugins` folder.
1. Run the game again. It should create an `APInfo.json` file in the install directory.

## Connecting to a multiworld
1. Enter the connection details in `APInfo.json`.
1. Run the game.

If it's not working, check for any errors in `BepInEx/LogOutput.log`.
