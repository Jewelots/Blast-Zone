SpritesheetAnimation Usage Documentation
==============

This document details how to correctly use the SpritesheetAnimation system.

# Base function calls

Initialise
 * Create AnimationSheet

Load
 * Call AnimationSheet load function with xml file to use, including additional directory path, without “.xml”, (eg: “Spritesheets\\softblocks”, including the “\\” and case-sensitivity)
 * Change extra default values if needed
 * Create each AnimatedSprite here, using the sheet, define the default values you want
 
Update
 * Modify each AnimatedSprite's positions, rotation, scale, etc
 * Call each AnimatedSprite's Update function to go through the frames in time
 
Render
 * Call each AnimatedSprite's draw function
 
Unload
 * Destroy each AnimatedSprite
 * Call AnimationSheet Unload function,

The AnimationSheet class also contains an Unload function to allow you to unload the config and load a new one without having to recreate the entire emitter, this is useful for graphics details settings or switching levels/characters.

AnimationSheets can have multiple textures, using the same animations, by default the first texture is used but an AnimatedSprite may have its SetTexture function called at any time after the sheet has been loaded to set it to another texture.

# XML Config File

Layout design is as follows:

 * Animation sheet
    * Each texture
       * Texture name
    * Optional mesh declare, uses per-pixel if not defined
    * Default framerate
    * Default sprite scale
    * Each animation, width and height defined if no mesh
       * Frame coords, optional flip mode
       * End return

Mesh declaration is optional, if defined the x and y coords for each frame will use this grid, otherwise they use the pixel coords of the sheet.
The width and height in each animation must be defined if there is no mesh.
Frame rate is in frames per second.
Sprite scale is in pixel multiplication.
Infinite amount of animations and frames can be defined to the same area.

# Example XML

```XML
<?xml version="1.0" encoding="utf-8" ?>
<Spritesheet>
	<Textures>
   		<Texture name="player1"/>
   		<Texture name="player2"/>
    		<Texture name="player3"/>
    		<Texture name="player4"/>
  	</Textures>
  	<Mesh x="4" y="2"/>
	<Framerate default="4.0"/>
  	<Scale default="3.0"/>
	<Animations>
		<StandDown>
			<Frame x="0" y="0"/>
			<End return="StandDown"/>
		</StandDown>
		<WalkDown>
			<Frame x="0"  y="0"/>
			<Frame x="1" y="0"/>
			<Frame x="0"  y="0"/>
			<Frame x="1" y="0" flip="H"/>
			<End return="StandDown"/>
		</WalkDown>
		<StandLeft>
			<Frame x="0" y="1"/>
			<End return="StandLeft"/>
		</StandLeft>
		<WalkLeft>
			<Frame x="0"  y="1"/>
			<Frame x="1" y="1"/>
			<Frame x="0"  y="1"/>
			<Frame x="1" y="1"/>
			<End return="StandLeft"/>
		</WalkLeft>
		<StandRight>
			<Frame x="0"  y="1" flip="H"/>
			<End return="StandRight"/>
		</StandRight>
		<WalkRight>
			<Frame x="0"  y="1" flip="H"/>
			<Frame x="1" y="1" flip="H"/>
			<Frame x="0"  y="1" flip="H"/>
			<Frame x="2" y="1" flip="H"/>
			<End return="StandRight"/>
		</WalkRight>
		<StandUp>
			<Frame x="2" y="0"/>
			<End return="StandUp"/>
		</StandUp>
		<WalkUp>
			<Frame x="2" y="0"/>
			<Frame x="3" y="0"/>
			<Frame x="2" y="0"/>
			<Frame x="3" y="0" flip="H"/>
			<End return="StandUp"/>
		</WalkUp>
		<Win>
			<Frame x="3" y="1"/>
			<End return="Win"/>
		</Win>
	</Animations>
</Spritesheet>
```