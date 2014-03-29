Particle System Usage Documentation
==============

This document contains information on how to correctly use the particle system.

# Base function calls

Initialise
 * Create Emitter
	
Load
 * Call Emitter load function with xml file to use, including additional directory path, without “.xml”, (eg: “Effects\\Explosion”, including the “\\” and case-sensitivity)
 * Change extra default values if needed
	
Update
 * Define MultiEmitter emission points (if needed)
 * Call emission function/s (if needed)
 * Call Emitter update function, not calling this will not move the particles
 
Render
 * Call Emitter draw function
 
Unload
 * Call Emitter terminate function

Created MultiEmitters will not emit from their own position by default, you can change their EmitFromSelf boolean value at any point after initialising.

The MultiEmitter class also contains an Unload function to allow you to unload the config and load a new one without having to recreate the entire emitter, this is useful for graphics details settings.



# XML Config File

Layout design is as follows:

 * Emitter
    * Textures
       * Each texture
    * Graphs
       * Each graph
          * Points
    * Particles
       * Each particle
          * Texture
          * Life
          * Size
          * Colour
          * Rotation
          * Acceleration
          * Decay
    * Emitters
       * Each emitter (for multiemitters)
          * Origin
          * Power
          * Rate
          * Blendstates
             * States

Should be in this order, as this is the order it is read in.
Textures, graphs, particles and emitters have an unlimited count with no defined number.
Textures, graphs, particles and emitters have their name defined as the element name.
Particles and emitters are linked by their element names.
Emitters have a list of blendstates, of which at least one must be defined for rendering to work.

# Example XML

```XML
<?xml version="1.0" encoding="utf-8" ?>
<MultiEmitter>
	<Textures>
		<Smoke texture="SmokeTex"/>
		<Star texture="5x5 Star"/>
	</Textures>
	<Graphs>
		<Base type="Bezier">
			<Point x="0.0" y="0.0"/>
			<Point x="1.0" y="2.0"/>
			<Point x="1.0" y="0.0"/>
		</Base>
		<Smoke type="Bezier">
			<Point x="0.0" y="0.0"/>
			<Point x="0.25" y="1.0"/>
			<Point x="1.0" y="0.0"/>
		</Smoke>
	</Graphs>
	<Particles>
		<Base>
			<Texture texture="Smoke"/>
			<Life length="3.0"/>
			<Size scale="50.0" graph="Base"/>
			<Colour r="255" g="69" b="0" a="255" graph="Base"/>
			<Rotation angle="0.0" graph=""/>
			<Acceleration x="0.0" y="-98.0"/>
			<Decay x="1.0" y="1.0"/>
		</Base>
		<Smoke>
			<Texture texture="Smoke"/>
			<Life length="5.0"/>
			<Size scale="100.0" graph="Smoke"/>
			<Colour r="127" g="127" b="127" a="255" graph="Smoke"/>
			<Rotation angle="0.0" graph=""/>
			<Acceleration x="0.0" y="-98.0"/>
			<Decay x="0.0" y="0.0"/>
		</Smoke>
		<Spark>
			<Texture texture="Smoke"/>
			<Life length="2.0"/>
			<Size scale="5.0" graph=""/>
			<Colour r="255" g="200" b="200" a="255" graph="Base"/>
			<Rotation angle="0.0" graph=""/>
			<Acceleration x="0.0" y="98.0"/>
			<Decay x="2.0" y="2.0"/>
		</Spark>
	</Particles>
	<Emitters>
		<Smoke>
			<Origin x="0.0" y="-50.0" depth="0.5"/>
			<Power x="25.0" y="25.0"/>
			<Rate rate ="20.0" count ="1"/>
			<BlendStates>
				<State state="AlphaBlend"/>
			</BlendStates>
		</Smoke>
		<Base>
			<Origin x="0.0" y="0.0" depth="0.0"/>
			<Power x="15.0" y="25.0"/>
			<Rate rate ="40.0" count ="2"/>
			<BlendStates>
				<State state="AlphaBlend"/>
				<State state="Additive"/>
			</BlendStates>
		</Base>
		<Spark>
			<Origin x="0.0" y="0.0" depth="0.0"/>
			<Power x="200.0" y="200.0"/>
			<Rate rate ="1.0" count ="1"/>
			<BlendStates>
				<State state="AlphaBlend"/>
				<State state="Additive"/>
			</BlendStates>
		</Spark>
	</Emitters>
</MultiEmitter>
```