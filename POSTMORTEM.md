BlastZone Post-mortem
==========

-- *Note: This is Jewel's Postmortem containing their own thoughts about the project, and is fairly informal, and a similar style would not be used in a professional environment* --

Here we are! The game's done! But what could we have done differently~? Well. If you ask me; a lot of things! But if you ask nearly anyone how they think of their work, I don't think you'd ever get any other answer. The end result; however, is very acceptable, remarkably so, for the timeframe, and I'm quite proud of it! You wouldn't know the hacky code existed without seeing the source itself, and I think the end result is what matters, as long as you learn what not to do next time. You have to make mistakes to learn from them.

I'd used XNA and C# before, so luckily that wasn't too much of an issue for me. I've played with both years ago, and deployed games or tests I made when I was younger, to the xbox as well. After working with C++ for a while it's still a bit strange to wrap your head around to easier-syntax and less restrictions, once again, but it's definitely a breather. Having to focus on something like C++ for so long really makes you enjoy coding in a simpler language, so the entire process was pretty fun, and I definitely like the product we made.

So! Problems! I had a rough idea going into this of what I wanted the engine to be like. I scribbed up some diagrams to help me think, and drafted up a rough system of what I thought I wanted. My plan was to upon that system until I had something I thought would be adequate. We only had a week for the planning, and due to the time limit, this wasn't quite the case. It ended up a fairly cobbled together technical document with no real time to get quite as in-depth as it would need, and as such a lot of features were left hanging as “Oh I'll just do this part when I get there”, as I didn't quite have time to plot it out in such detail that would leave me knowing if something would have adequate coupling to be accessed. I knew the layout of what I wanted to do, but not exactly how those parts would be able to interact. It's easy enough when planning to just say “this talks to this”, but in code it's another story!

So the planning was complete and I had an idea of what I wanted in the engine and how I wanted to accomplish it. My partner ended up being tasked to Particles and Animation system, lesser quantity but not lesser jobs, especailly as the Particles were a major part of making the game look as we wanted it to, and the task ended up being quite a hefty feat. I began to create some quick engine drafts of various systems, hopping between a menu and movement and entity placing and fire and such. When working with art it's best not to dwell on one section for too long, as if you focus on that section without looking at the big picture, in the end you'll end up with something far too detailed surrounded by much lesser detail. It's a focal point that you don't neccesarily want, as a game is not one system, it's many systems working in tandem, and your goal is to abstract that away from the user as much as possible to provide a seamless experience.

This process worked fairly well, as it left me being able to work on something else if I got stuck on a specific thing, such as movement. The movement system was never really planned out enough in the technical documentation, and that was a major hurdle. The systems that I worked on that took the most time, or were the most complex, was probably the Movement system and the Lobby system, both of which were not detailed on the technical documentation due to time constraints, or lack of knowing exactly how to do them without prior experimentation. In a system at a real company, this would be done in a longer timeframe beforehand, and with the ability to make rapid prototypes of specific systems to see if they work before documenting and designing the engine itself, so I'm not too disappointed, especially as the systems themselves got finished far within the alloted timeframe.

The movement system had me trying to work out the best way to have movement along a grid. The key features this needed were:

The ability to move along a grid
The ability to not pass through obstacles
The ability to snap to adjacent close pathways, to avoid having to be pixel-perfect
The ability to walk off an obstacle if already standing on one (such as a bomb)

At first I drew out a diagram and attempted to create a system of pathways much like a Graph, where each “node” (tile) had preset directions Up, Down, Left, and Right, in which it could connect to other adjacent nodes. The player would move along these pathways and if you were holding a perpendicular direction it would snap to the node when it got near enough, and continue on in that direction, along that path. When an object was placed on a node, it would know that the pathways connected to that node would be flagged as un-walkable, or if you're already on one of those pathways, to let you walk off.

This system seemed fairly flexible, also allowing for AI to traverse it using pathfinding algorithms if we got to that stage, but in the end it was deemed unneccesary, too expensive to make, taking too much time from the budget for the end result. So I went with a simpler system of moving, snapping to tiles, and not letting you move if there's something infront of you, much like the path-based system but just measuring the distance to the center of the tiles to get the “closeness” for snapping, rather than a  percent along a path. There's still a few problems with this system, such as getting “stuck” a little if you're going too fast (stuck as in unable to go in the direction you want, stuck on a seperate path until you manage to position yourself right), but ultimately it controls enough to be definitely playable on a gamepad or a keyboard.

The lobby system was the next big hurdle. I wanted it to be flexible, to provide the best user experience on both platforms, and that involved having to create one of the most annoyingly-detailed systems in the entire engine. The specs I had in mind were this:

Four slots for four controllers of any type, keyboard or gamepad
You can join in with any controller you wanted, and get put in as the “next” player
You don't have to play player 1, with gamepad 1

The XBLIG store actually will reject your game if you don't let the player play with Gamepad 2, if Gamepad 1 is taken up with, say, a Guitar controller, so a system like this is something even Microsoft's thought about, and determined provides the best end-user experience due to the freedom they have, so I wanted something similar. My setup had to involve not just gamepad's but the keyboard, too, so that was added technicality required. I ended up dwelling on the system, and making a fairly complicated setup where it checks for buttonpresses on any controller, and adds a player->controller mapping. It was a little more in-depth than this, but that's the rough gist of the end result required.

The other major system in the game was the tile-entity system, and being linked with the fire system. My partner touched a bit on it in his part of the post-mortem on how he'd go about doing it. I guess I'll touch on it a bit myself since I was the one who (poorly) concieved it's workings. This was probably the most hacked-together system of the lot. I planned too roughly for what amounts to the main workings of the game, and ended up with a bit of a mess.

I ended up having the Level own the TileObjectManager, and the FireManager. This is fine, but because of the gross coupling, I had to have a TileObject have a reference to the Level it's in, and having FireManager be public, just so I can really-grossly this.Level.FireManager.ExplodeFrom(x, y). If I could change this, or had much more time to make a more extensible engine, I'd have this work on an Event-Driven system or such. As loosely coupled as possible, basically. Loose coupling is the key to having an extensible system, I think, and it's great when you can swap in and out parts, communicating between each by only messages. It's a very rewarding system but ultimately required too much time to conceivably implement.

As for the timeline? We probably stuck too loosely to that, having some goals much earlier and some goals much later, but due to the way we designed the engine, we had a lot of systems ready early, but they weren't combined together until fairly late, so we didn't have much to show as a whole until near the end. This is mostly a bad thing, as we had to have it playtested, and a feature (powerups) had to be rushed in very last-minute. Luckily due to the engine design it was near-trivial to implement what would probably be a fairly annoying task with any other design, as we only had to make a new child of TileObject and hook the power-up functionality to player collision events that were already being called.

We didn't end up meeting much of our “extra” goals, but we did get the game into a state where it's 100% playable, pretty heavily polished, and bugfree (from what we've seen so far, maybe more will crop up when it's playtested more thourougly later, but for now, seems good), and I think that's a success. The extra goals we didn't get done were AI (a singleplayer mode, but we decided multiplayer only is okay, as it's a party-game), and networking, which was a far-stretch-goal but ultimately decided not to be worth it. Perhaps I might come back to it in the future and attempt to get networking implemented, as the event-driven nature of the engine I built would hopefully make it not too difficult an endevour (although I have heard networking is best implemented at the very start of the project). The music and sounds system was added near the end, but added much more of a vibrant feeling to the game, and required no extra classes due to XNA's wonderful inbuilt sound/music functionality. Without sound I feel games are sorely lacking, and even simple music/sfx adds a lot of character and playability.

Overall: What would I do better next time? Plan more. Everything that was poorly designed, stemmed from that. Everything else went pretty seamlessly!