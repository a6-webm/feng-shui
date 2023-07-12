objects with colliders to say where the furniture can't be placed
- detect collision with tag of "NoFurniture" or component of "NoFurniture" idk
- need to write a "find nearest free space" function so that furniture dropped in an invalid location can slide over to a spot that is valid
 - e.g. you dropped a chair juuuuuust slightly in a wall and it slides over to be up against the wall rather than in it
 - perhaps a "find nearest edge of the NoFurniture box you're in and line up against it"

points are just an object with a transform
walls, doors, windows etc are objects that take two points as args
some objects are flexible, e.g. walls are the length between two points
some objects aren't, e.g. a door has a premade mesh and has two child point objects

scale: 10 units = 1 meter

Localization package

100 levels?

drawing lines:
- some types of edge need to generate a flat mesh
- initially going to be coloured black, but could later be textured
- you can specify the line endings e.g. rounded end, flat end, fade out end
- can eventually apply a normal map to the ground to make it look like paper, and in the same vein the lines

Needs to have the progression of the witness
like u do groups of puzzles that only use one mechanic, and then u end up combining mechanics later in the game
like the witness, it won't be shown why a solution is incorrect (unless u use a hint)
should we show which restrictions are in effect for each level?:
- pros:
  - u can more easily make levels that focus on the interaction between two restrictions without the others getting in the way
- cons:
  - removes the task of figuring out which rules are in effect
- could maybe have the first 20-30 levels not show rules and build them up layer by layer and then later levels show which ones are in effect?
issue: right now, I'm thinking of ways to change the game mechanics so that odd looking solutions can't happen, but really we need any valid solution to look nice if we want this game to not take forever:
- does constraining levels to one room fix this?


what the player can do:
- place furniture from their inventory
- knock down certain walls?

game mechanics:
- ying yang/balance
- ba gua
- elements/colors/shapes/direction
  - kitchen is fire
- some levels in ancient china, some modern day; rules can play out differently (e.g. ba gua)
- managing chi (life chi, dead chi, killing chi, movement chi)
  - the chi flow probably shouldn't be a fluid sim or something, cause we need to keep the state space kinda discrete so we can make good puzzles lol
  - chandeliers killing chi
  - implementation: 
    - a spline, the field value being "the distance to the closest point on the spline"
- command position
  - could do only furniture with a specific mark/pattern needs to be in command position
- chinese numerology
- "social circle"? (ring of chairs/sofas)
  - need to place a coffee table in the middle
- lighting
- mirrors