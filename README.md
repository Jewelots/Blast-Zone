Blast-Zone
==========

This product is not meant for commercial use and all resources are used entirely for non-commercial use, and are sourced in [RESOURCES.md](RESOURCES.md)

An in depth design document can be found at [DESIGNDOC.md](DESIGNDOC.md) and cointains description for each of the classes contained within the code.

A post-mortem can be found at [POSTMORTEM.md](POSTMORTEM.md) and contains thoughts on the project after completion.

![Menu](/wiki-images/menu.png)

![Example](/wiki-images/examplestart.png)

# What is it?

Blast Zone was a gamedev project for a school assignment. The assignment was to create a completely functional 2D game using a cross-platform toolkit in only four weeks. The assignment was in teams of two programmers, but contacting outside artist help was allowed.

## How we settled on the idea

I mainly took over as the project lead, since I had more design experience, and had a more refined idea of what I wanted to accomplish with the project.

I settled on these simple ideals:
 * Simple predefined rules
 * Not many graphics/assets needed
 * Not many sounds needed
 * Simple single-screen-sized level(s)
 * Visually impressive if done well
 * Can be easily polished
 
With these ideals in mind, I ended up on the idea of recreating Bomberman. Bomberman fulfils all those goals, and with the added bonus that the time other groups spent on creating an idea and creating/finding assets, could be used to further polish and develop the engine itself, as the idea was simple and cemented. Extra polish, since it was a modern engine, was decided to be particles and high-detail graphics (not neccesarily the individual sprites themselves, but the smooth scaling and movement).

The only thing left to settle on was the cross-platform toolkit we were going to use. XNA was decided even though it's depreciated, as it's simple, powerful, and builds easily to PC and XBOX360.

The alternatives were discarded for various reasons:
 * Playstation Mobile SDK: Almost no documentation, unable to build to PC for quick prototyping
 * Monogame: Very similar to XNA but Mac/Linux/Android/iOS instead of XBOX360. Runs off the XNA Content importing system so it didn't really provide any advantage over XNA itself.

# Game pitch

After the idea was settled, we had to pitch it to the other groups, to demonstrate we had a thorough idea of what we were creating. Our pitch notes were expanded on in the speech, but the framework of what we expanded on was these simple ideals:

 * Based as a simple "party" PvP game
 * Very simple gameplay
 * Players move around a simple 2D level
 * Players place bombs
 * Bombs explode in a cross shape
 * Players must avoid their oponent's bombs and their own
 * The last player standing wins
 
With the extra ideal of
 * Randomly found powerups alter the players abilities to make them more powerful

# Initial Notes

A collaboration of notes on what the game should contain. Fairly messy but contains our initial rough ideas.

Platforms:
 * PC (majority of market (comparatively))
 * XBOX360

Mechanics:
 * Core:
    * Player moves around 2D map,
    * Player can charge and place bombs
    * Player has to avoid bombs
    * Bombs break certain obstacles
    * Player has to utilise bombs to defeat oposing NPCs, players or obstacles
    * 1-4 players
    * Particles
	
 * Optional (in order of preference/difficulty):
    * powerups
    * extended level obstacles
    * AI
    * different types of bombs
    * network multiplayer