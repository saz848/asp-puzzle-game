# asp-puzzle-game
### By: Sarah Ahmad and David Hofferber

## Initial Puzzle Game
We wanted to build on a puzzle game that was designed in Rob Zubek's Game Design Studio course (Winter 2018). The original game, [RacingThruTime](https://github.com/saz848/RacingThruTime), consisted of levels created by hand. In this game, the player moves autonomously in the world, and the world must be manipulated so that the player reaches the goal. To manipulate the world, you need to click on puzzle pieces and then rotate them so that the player is able to find a path to the goal.

We have multiple types of puzzle pieces that can be rotated: lines, crosses, and corner tiles. Each of these pieces occupy a 5x5 grid space and their center of rotation is centered in this space. To save space in the Unity scene, we scale these puzzle pieces down to a 2.5x2.5 grid space. There are also non-rotatable pieces that can be added in between rotatable tiles, though these were primarily used for handmade level design. The non-rotatable pieces were often used to trap either an enemy or the player. 

In the original game, we also had several different types of enemies. Some enemies only killed the player on contact, whereas some enemies froze the puzzle piece they were on, thus making the piece non-rotatable. 

## Level Generation using ASP

### Design - Initial Prolog

### Creating Levels using ASP
// need something here about how the prolog was then converted to ASP and how ASP works a lot better for our use

Since we ended up with a significant number of models to choose from for our levels, we needed to parse through the model json to attain a subset of the models for our levels. We decided to create 10 Unity scenes so that we could have a different set of models to choose from for each of these scenes. Each subset contained 1,000 different possible levels, and we randomly select one level to generate from each time the game is played. 

We then parse the json represenation of the level, which is in the form of `inst(10,10,cross)`. The x and y coordinates are the first two parameters, and the third parameter is the type of puzzle piece. We can then instantiate a GameObject prefab using this information at the given location. We do not need to consider the rotation of the piece because the puzzle piece can be rotated by the player during gameplay. The start and end tiles are hard coded at the specific locations. 

## How to Play
You can select any Unity Level scene to start playing. To rotate a tile, you click on the tile and then use either the left and right arrow keys or Z and X. The player is the little green triangle, and the goal is the yellow square. You can speed up if you need to by pressing the spacebar key. 

## Future Iterations
One thing that we haven't included yet in this version of the puzzle game is the enemies. Our first step will be to instantiate at most two regular enemies on a level at the center of cross tiles. As we continue to build the model and add the different types of enemies, we will need to consider the constraints on whether enemies disable tile rotation and whether the enemies block a path to the goal. We will also need to determine how many enemies can be included in a level at a maximum. 

