# BlocksWorld

A small game focused on building stuff together.

## Game Idea
The game is built around two modes:

- Editor Mode
- Play Mode

In the Editor Mode, players can built a Scenario in a block styled world. Each Scenario can be
played in the Play Mode afterwards. Players can also start publishes Scenarios.

The Editor Mode provides features to build a world consisting of Blocks and Details. Blocks are
known from games like Minecraft, they are placed in a regular grid and built the structure of the
world. Details are models that can be freely placed in the world and provide game interaction.
Each Detail can be annotated with one or more Behaviours that define how the object interacts with
the world. Details provide Interactions that a player can choose and activate the object.

The Play Mode provides the actual game mechanics and allows players to play a Scenario.

## Screenshots

![Some Image]
(http://mq32.de/?x6c5)

![Some Other Image]
(http://mq32.de/?x6c8)

# Current Features
- Unlimited Block World
 - Expandable in every direction
 - Sparse Storage (only blocks that exist are stored)
 - Detail Objects
  - Have Behaviours
  - Have Interactions
  - Have a single model
- Multiplayer
 - Synchronized Players
 - Synchronized Blocks
 - Synchronized Detail Objects
- Jitter Physics
 - Used for collision and movement
- Model Editor
 - Model Viewing
 - Model Conversion
 - Model Adjustment
  - Edit Mesh Texture
  - Edit Mesh Size
  - Rotate Mesh

## Todo List
This list contains items that will be short term as well as long term plans

- physical detail objects
- generic interaction system with multiple interactions (Key.E)
- think about Lua script api
- add shapes to DetailObjects
- add detail editor tool
- add sub-details
- detail object classes
 - long term: class/object editor
- primitive forward lighting
 - long term: deferred renderer
- add translate options to model editor
- add model browser
- add primitive UI rendering with System.Drawing and Texture2D (y)
 - long term: Improved UI system
 - UI skin system
- game modes
 - editor mode
 - player mode
- game features
 - quest system
 - inventory
 - dialogues
 - weapons/tools
 - mobs / enemies
 - long term: professions
 - resource gathering

## Dictionary

### Edit Mode
The game mode where players can build a Scenario.

### Play Mode
The game mode where players play the actual Scenario.

### Block
A cube that defines part of the world structure. The world consists of Blocks
and each block has its own textures.
Blocks can also hold Scenario specific data that allows players to interact with them.

### Detail
A detail is an interactible world object. It is a single 3D model combined with
colliders and optionally a Behaviour,

### Behaviour
A script or script collection that defines how a Detail behaves, what Interactions
it provides and how the story progresses.

### Interaction
A trigger that can be activated by the player. Mostly bound to visible game objects.

### Scenario
A small "game" that can be played in the Play Mode. It contains game logic,
a single level and a defined win/lose condition.