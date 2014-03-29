BlastZone Design Documentation
==============

This document contains the descriptions of the technical design behind the game BlastZone.
A class diagram is contained separately to this document.

There are additional documents for the [SpritesheetAnimation](SPRITESHEETANIMATIONDOCS.md) and [Particle systems](PARTICLEDOCS.md), both are designed for use in this project but are also designed to be extensible enough to be used elsewhere.

# Base Design
The base design of the game is separated into many modules for simplicity and simple code modification with little need to modify other modules.

Core:
	The core workings of the game are managed in a Gamestate manager system, of which the root XNA game class runs this, as the user navigates between the game menus and actual gameplay, this is all stored inside the gamestate manager, each state of the game is defined by separate classes, which the gamestate manager will switch between accordingly.

Drawing:
	There are many different components for drawing the components of the game on-screen, a DrawTextExtension class defines functions for drawing fonts on the screen with special effects, a ScreenTransition class defines screen transition systems to nicely switch between game states, a SpritesheetAnimation system is used for animated sprites on a single spritesheet, this is documented seperately, a TiledTexture class is used for rendering rectangles of a moving set of texture tiles, this is used for the backgrounds in the menus.

Effects:
	A Particle system was developed alongside the main project to be attached towards the completion of the game, this is documented separately, this is used to add particle effects to the game.

Level:
	The game consists of a basic 2D level of tiles, of which a main level system manages and a LevelAesthetics class handles the drawing of the level components, this Level system makes use of the manager classes to run the needed components of the gameplay.

Managers:
	The manager classes will manage specific components of the game system, a FireManager manages the fire that is spawned upon a bomb explosion, this fire is intended to trigger immediate explosion of bombs in range and death of players in range, a FloatingAnimationManager manages animations for breaking SoftBlocks, a ParticleManager provides a static interface for emitting particles at positions on the screen, a TileObjectManager manages the tileObjects in the level such as bombs.

MovementGrid:
	With the simple 2D level and the nature of the gameplay comes the need for a way to manage movement across the grid, a GridNodeMap contains the grid with info for nodes to know what tiles are traversable or not and a GridNodeMover class is used as a common interface node for entities to use to navigate the grid.

States:
	As already mentioned, the core part of the game code uses a state system, each gameplay state is defined as a separate class, a MenuState class is defined and used at the opening of the game that allows the user to go to other states, a LobbyState class is defined to set up the players for the actual gameplay, a GameplayState class is defined to run the actual gameplay after the lobby state has finished, all of these states are managed in a single GameStateManager class based on a GameState abstract class.

TileObjects:
	As this game uses a simple 2D level of tiles, entities such as bombs use an abstract class, TileObject, of which all instances of TileObjects in the game are created by a TileObjectFactory and managed by the TileObjectManager manager class noted above.

# Components

All Components:
 * Drawing
 * DrawTextExtension
 * ScreenTransition
 * SpritesheetAnimation
 * TiledTexture
 * Effects
 * Particles
 * VectorGraph
 * Level
 * Level
 * LevelAesthetics
 * Managers
 * FireManager
 * FloatingAnimationManager
 * MouseManager (debug only)
 * ParticleManager
 * TileObjectManager
 * MovementGrid
 * GridNodeMap
 * GridNodeMover
 * States
 * ControlsState
 * GameplayState
 * GameState
 * GameStateManager
 * LobbyState
 * MenuState
 * OptionsState
 * TieScreenState
 * WinScreenState
 * TileObjects
 * Bomb
 * Powerup
 * SoftBlock
 * TileObject
 * TileObjectFactory
 * EventTimer
 * GlobalGameData
 * MainGame
 * Player
 * PlayerInputController
 * Program
 * ScoreRenderer

### Component details:

Drawing:
 * DrawTextExtension
    * Draws text to a spritebatch with an outline effect
	
 * ScreenTransition
    * Makes a screen transition for switching between game states

 * SpritesheetAnimation
    * Contains an AnimationSheet and AnimatedSprite class for using animations on a single sprite sheet with multiple sprites.
    * Refer to [SpritesheetAnimation Docs](SPRITESHEETANIMATIONDOCS.md) for more details

 * TiledTexture
    * Draws a rectangle with a tiled texture inside

Effects:
 * Particles
    * Contains Particle, Emitter and MultiEmitter classes for particle effects on floating points on the screen.
    * Refer to (Particles Docs)(PARTICLEDOCS.md) for more details

 * VectorGraphs
    * Contains a VectorGraph template along with EmptyVectorGraph, BasicVectorGraph and BezierVectorGraph implementations for use with the particle system or otherwise making a 2D graph for various uses.
    * Refer to (Particles Docs)(PARTICLEDOCS.md) for more details

Level:
 * Level
    * Handles the majority of management for gameplay

 * LevelAesthetics
    * Handles the textures and rendering for the level

Managers:
 * FireManager
    * Manages the fire in the level

 * FloatingAnimationManager
    * Manages floating animations for breaking softblocks

 * ParticleManager
    * Manages particle emitters

 * TileObjectManager
    * Contains and manages the active TileObjects

MovementGrid:
 * GridNodeMap:
    * Contains the grid for the level to use, a 2D array of boolean

 * GridNodeMover:
    * A movement node for moving entities on the grid

States:
 * ControlsState:
    * Shows the keyboard and controller controlls

 * GameState:
    * A game state abstract class for making the states of the program off
	
    * GameplayState:
       * The gameplay
    
    * LobbyState:
       * Gives the choice of what input devices and player count to use
    
    * MenuState:
       * The menu
    
    * OptionsState:
       * Gives the user options for sound volume and particle quality
    
    * TieScreenState:
       * Gives the players a "tie" game over screen
    
    * WinScreenState:
       * Gives the players a "win" game over screen
    
    * GameStateManager:
       * A class that manages all the game states

TileObjects:
 * Bomb:
    * A TileObject that explodes and creates fire tiles next to it

 * Powerup:
    * A TileObject that is collectable by players and will give bonus abilities to the player, 		is destroyable

 * SoftBlock:
    * A TileObject that poses a destroyable obstacle to the players

 * TileObject:
    * An abstract class for as a common interface for the TileObjectFactory and 			TileObjectManager to use and manage all the TileObjects in use 

 * TileObjectFactory:
    * Creates TileObjects by call and contains common values for simplicity

EventTimer:
    * A timer class used by many components in the game code

GlobalGameData:
    * A container class for the global game variables such as screen width and height

MainGame:
    * The master game code class, this is defined by XNA 

Player:
    * The visual representation of the player

PlayerInputController:
    * The input system to control the player

Program:
    * The root code class defined by XNA and runs the MainGame class components

ScoreRenderer:
    * The render system for the player scores in-game

# Level Details

The level is a simple finite 2D array of tiles, filled with evenly placed solid blocks that cannot be broken by any means, there are "soft blocks" placed in parts next to and between the solid blocks, these are breakable by bombs, the rest of the level is traversable by the players.

The softblock texture is randomised on creation of the level, when the softblocks are broken they are removed and a call to the FloatingAnimationManager renders an animated sprite of the block being destroyed.

To destroy the softblocks and (eventually) destroy the opposing players, the players place bombs, of which these bombs run on a timer, as this timer ticks the bombs glow red, emit smoke particles and pulsate in size, until they eventually explode after a couple of seconds and create fire tiles (invisible) which emit explosion particles and will destroy softblocks and/or players that happens to be in the same tile space, the default bomb fire radius is one tile from itself in every straight direction (ie, not in diagonals), the explosion will also trigger bombs to immediately explode on contact.

Players cannot walk on softblocks or bombs, when they place a bomb they can walk off it but not back onto it, allowing them to be trapped with no escape, by default players can only place one bomb at a time, though on destruction of a softblock there is a random chance a powerup is dropped, of which can be picked up by a player to increase their maximum bomb count, walking speed or bomb fire radius.