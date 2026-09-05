# Offipelago
Archipelago for OFF (2025).

## Requirements
- OFF (Steam)
- Visual Studio 2022

## Installation
- Add MelonLoader to your copy of OFF.
- Build the mod (see below).
- Place Offipelago.dll and Archipelago.MultiClient.Net.dll in the Mods folder.

## Building
- Create a copy of `client/Properties/GamePath.props.template`.
- Rename it to `GamePath.props`.
- Optionally change the `GamePath` property in the file, if your OFF installation is in a different place.

## OffExplorer
A custom made event browser for OFF. Allows you to look through all actors in a room.

### Controls
- I: Select next event
- K: Select previous event
- J: Reprint important object list
- L: Show code for selected event
- P: Next room
- O: Previous room
- N: Toggle noclip
- 1: Toggle Alpha in your party
- 2: Toggle Omega in your party
- 3: Toggle Epsilon in your party
- 7: Level up Alpha (May not always work)
- 8: Level up Omega (May not always work)
- 9: Level up Epsilon (May not always work)

## Development
The bulk of the mod is handled in the Patches.cs file. It contains a bunch of functions
that are run when their respective room loads. To add a new room, simply create a new
function called Post_###, where the ### is the room number to modify. Use the GetActor
function to modify an event in the room. A few functions are provided to handle patches
that are used often, such as chests.

### Using OffExplorer for development
In general, the process looks like this:
- Enter the room you wish to modify.
- Press J to output a message to the console showing the room number and any important actors.
- Find actors using the I/K keys, and view their events with L.
- Create a patch for the room to modify the actor(s).

### If you have any questions, ask WestCraft15 on Discord for help.
