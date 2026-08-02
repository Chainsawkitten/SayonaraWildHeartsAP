# Build instructions
Building from source is only necessary if you want to contribute. If you want to play, get the prebuilt releases and check the [install instructions](README.md).

## Building the randomizer
1. Copy the `Sayonara Wild Hearts_Data` folder from the install directory to `Randomizer`.
1. Open `Randomizer/SayonaraWildHeartsRandomizer.csproj` in Visual Studio and build it.
1. The binaries will be in `Randomizer\bin\Debug\net35`.

## Building the Archipelago world
1. Clone the [Archipelago repository](https://github.com/ArchipelagoMW/Archipelago).
1. Copy `Archipelago\sayonarawildhearts` into the `worlds` folder in Archipelago.
1. Run Archipelago from source: `python ./Launcher.py`
1. Open `Build APWorlds`.
