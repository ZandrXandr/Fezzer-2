# Fezzer-2
Built for compatibilty with FezMod, Fezzer 2 is a Fez Asset Editor

To use, you need to extract the game's resources. Do this using FezParse (https://github.com/fesh0r/xnb_parse)
Then simply edit the Resource Path parameter on the LevelImported script to wherever your FezParse folder is located. (For example, F:/Users/[username]/Documents/FezParse/")


-How to set up and use-
1:Download and extract the project, either by cloning it using git or grabbing the .zip.
2:Use FezParse to extract the game files. Keep note of the path of FezParse
2:Open the project in at least Unity 5
3:Open the scene "Test" under "Custom Assets>Scenes"
4:Select the "Level" object on the hierarchy, and change "Resource Path" to the FezParse folder (Example:C:/Users/[user]/Documents/FezParse/"
5:Change the "Level Name" to whatever the name of the level is, in all lowercase, without any extension (Example, bell_tower. level names can be found in your "[fezparse]/out/levels" folder) 
6:Hit the play button on the top of your screen
7:Hit the L key on your keyboard, and wait for the level to load in.
6:Right click to place new triles, left click to remove, click a trile on the left panel to select it.

----------NOTES-------------
Currently FZ2 doesn't work as a build! Still in development!


----------TODO--------------
Preloading trilesets and art objects so they aren't loaded on the fly every time you need new ones
Level Editor (Need writing in FmbLib)
Comment a bunch so people know what stuff is
Optimize, obviously
Trile Editor (Need writing in FmbLib)
