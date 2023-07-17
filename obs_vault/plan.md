objects with colliders to say where the furniture can't be placed?

points are just an object with a transform
walls, doors, windows etc are objects that take two points as args
some objects are flexible, e.g. walls are the length between two points
some objects aren't, e.g. a door has a premade mesh and has two child point objects

scale: 10 units = 1 meter

Localization package?
- ideally things just become apparent so there's no need for translation
- but what if we ask for money/want to talk abt Dear Modern lol

100 levels?

drawing lines:
- some types of edge need to generate a flat mesh
- initially going to be coloured black, but could later be textured
- you can specify the line endings e.g. rounded end, flat end, fade out end
- can eventually apply a normal map to the ground to make it look like paper, and in the same vein the lines

basically non-discrete sokoban
you drag furniture across the floor
use physics to move furniture, you can drag with one or two fingers, allowing for easier positioning
restrictions are mostly extra colliders for specific furniture

what the player can do:
- place furniture from their inventory
	- could add a feature where if you can get a furnishing outside the house, it goes into your inventory
- knock down/place certain walls?
- slide furniture across the floor

game mechanics:
- yin yang/balance
	- I think windows give off a bit of yang energy, not sure what to do with that lol
- ba gua
- elements/colours/shapes/direction
	- kitchen is fire
- some levels in ancient china, some modern day; rules can play out differently (e.g. ba gua)
- managing chi (life chi, dead chi, killing chi, movement chi)
	- killing chi:
		- chandeliers, hanging lights, sharp corners
	- movement chi:
		- indicator: blue lines denoting the default chi lines
		- a spline, the field value being "the distance to the closest point on the spline"
		- splines are created by path-finding between every combo of tagged entrances and windows
		- long straight corridors increase flow: the sum of the abs value of the angles to the next vertex for every vertex is how bendy the path is
- command position:
	- indicator: furnishing has symbols on its 4 sides?
	- chair can only move if it's riding a wall?
	- chair can only move if it has furniture either side of it?
	- chair can only move if chi is in front of it?
	- chair can only move if not in a spot with high chi?
	- honestly could mix and match these per furniture
- Chinese numerology
- "social circle"? (ring of chairs/sofas)
	- indicator: rug or coffee table?
	- maybe while in a social circle, furniture automatically points towards the centre?
- lighting
- mirrors
