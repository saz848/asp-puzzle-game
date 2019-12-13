# asp-puzzle-game
### By: Sarah Ahmad and David Hofferber

## Initial Puzzle Game
We wanted to build upon a puzzle game that was designed in Rob Zubek's Game Design Studio course (Winter 2018). The original game, [RacingThruTime](https://github.com/saz848/RacingThruTime), consisted of levels created by hand. In this game, the player moves autonomously in the world, and the world must be manipulated so that the player reaches the goal. To manipulate the world, you need to click on puzzle pieces and then rotate them so that the player is able to find a path to the goal.

We have multiple types of puzzle pieces that can be rotated: lines, crosses, and corner tiles. Each of these pieces occupy a 5x5 grid space and their center of rotation is centered in this space. To save space in the Unity scene, we scale these puzzle pieces down to a 2.5x2.5 grid space. There are also non-rotatable pieces that can be added in between rotatable tiles, though these were primarily used for handmade level design. The non-rotatable pieces were often used to trap either an enemy or the player. 

In the original game, we also had several different types of enemies. Some enemies only killed the player on contact, whereas some enemies froze the puzzle piece they were on, thus making the piece non-rotatable. 

## Level Generation using ASP

### Design - Initial Prolog
Initial design for the level generation was attempted via Prolog, largely due to an initial misunderstanding regarding the AnsProlog format and the fact that ASP looks significantly like standard Prolog with a few tweaks. Switching to ASP made life much simpler when generating models, as it was easily able to turn relatively bulky Prolog code, like this:
```
pairs(N, R) :-
	succ(N0, N),
	bagof(P, pair(N0, P), R).

pair(N, X-Y) :-
	between(0, N, X),
	between(0, N, Y).

shape_pos(X, Y) :-
	pairs(6, R),
	member(X-Y, R),
	0 is mod(X, 5),
	0 is mod(Y, 5).

initial_shape(T, X, Y) :-
    member(T, ['cross', 'corner', 'line']),
    shape_pos(X, Y).
```

Into a much cleaner generator:
```
shape(cross;line;corner;empty).
coords(X, Y) :- X = (0..n)*5, Y = (0..n)*5.
1 {inst(X, Y, T): shape(T)} 1 :- coords(X, Y).
```

In short, switching to ASP made our lives significantly easier, as despite using proper ASP relatively last minute, it was much easier for the task at hand than using Prolog was.

### Creating Levels using ASP

Once we got the initial level generation working, we added additional constraints to improve the "quality" of the level. This includes things like ensuring that at least a certain amount of cells are empty, ensuring that the same peace isn't repeatedly added to the entirety of the map, and ensuring that a piece is not surrounded by empty spots unless it itself is empty. This significantly decreased the number of models the code generated after running it for several minutes. Despite adding additional constraints, the file size still grows very quickly for a 4x4 grid, resulting in us generating ~2 million models.

Since we ended up with a significant number of models to choose from for our levels, we needed to parse through the model JSON to attain a subset of the models for our levels. We decided to create 10 Unity scenes so that we could have a different set of models to choose from for each of these scenes. A python script was developed to create the subset JSONs by selected 10,000 random models from our outputted set of models, and further segmenting that into 10 different subsets, each containing 1,000 different possible levels. The Unity scenes in question pull from the associated segmented JSON, and we randomly select one level to generate from each time the game is played. 

We then parse the JSON represenation of the level, which is in the form of `inst(10,10,cross)`. The x and y coordinates are the first two parameters, and the third parameter is the type of puzzle piece. We can then instantiate a GameObject prefab using this information at the given location. We do not need to consider the rotation of the piece because the puzzle piece can be rotated by the player during gameplay. The start and end tiles are hard coded at the specific locations.

The JSON containing all 2 million generated models, as well as the smaller segmented jsons, can be downloaded [from this Google Drive folder](https://drive.google.com/drive/folders/1izZUHmV_7Y5GmlJufRCTfJggH4vHG2Jw?usp=sharing).

In order to generate your own JSONs, the following commands should be used (assuming a Windows OS):
```
clingo puzzle.lp -n0 --outf=2 > output.json
python choose_random_levels.py
```

Please note that we're relatively unsure how large the JSON will grow to be if you attempt to run the program via clingo until completion. To be safe, we recommend halting the program via `CTRL+C` being inputted to the console after ~5 minutes of runtime, which will result in a file size between 750 MB to 1 GB depending on your CPU and the total time it ran.

## How to Play
You can select any Unity Level scene to start playing. To rotate a tile, you click on the tile and then use either the left and right arrow keys or Z and X. The player is the little green triangle, and the goal is the yellow square. You can speed up if you need to by pressing the spacebar key. 

## Future Iterations
One thing that we haven't included yet in this version of the puzzle game is the enemies. One thought we had for how to initially develop them would be to instantiate at some amount of regular enemies on a level at the center of cross tiles, as generating them on cross tiles guarantees that players can defeat the level so long as they play carefully. As we continue to build the model and add the different types of enemies, we will need to consider the constraints on whether enemies disable tile rotation and whether the enemies block a path to the goal. We will also need to determine how many enemies can be included in a level at a maximum.

Additionally, the game itself only rotates tile by tile at the moment. This was a cosntraint that was added into the initial levels of the original game, but was tweaked later on in special cases. It would be interesting to explore variations, such as rotation each class or category of tiles at the same time in a level, or alternatively pairing tiles of a certain color together. This seems relatively viable, as we would just expand our `inst/3` predicate to a new `inst/5` form in which we now consider the assigned category (which could either be assigned depending on object type, or a different trait like color), and now must also consider the initial rotation of all pieces in order to guarantee solvability.

