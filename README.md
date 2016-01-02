# Fezzer-2
Built for compatibilty with FezMod, Fezzer 2 is a Fez Asset Editor

To use, you need to extract the game's resources. Do this using FezParse (https://github.com/fesh0r/xnb_parse)
Then simply edit the Resource Path parameter on the LevelImported script to wherever your FezParse folder is located. (For example, F:/Users/[username]/Documents/FezParse/")


-How to set up and use-

1:Download and extract the project, either by cloning it using git or grabbing the .zip.

2:Use FezParse to extract the game files. Keep note of the path of FezParse

2:Open the project in at least Unity 5

3:Build the game using File>Build Settings>Build for your OS

4:Run the game when the window opens in the folder for it

5:Type the path to xnb parse in the Resource Path field (Example:"C:/Users/[your user]/Documents/xnb_parse/"

6:Type the level you want to load into Level Name field (bell_tower,arch,ect. You can find the names in xnb_parse/out/levels)

7:Hold right click to move the camera, hold alt to edit the terrain.


----------NOTES-------------
Currently FZ2 doesn't work as a build! Still in development!


----------TODO--------------
Preloading trilesets and art objects so they aren't loaded on the fly every time you need new ones
Level Editor (Need writing in FmbLib)
Comment a bunch so people know what stuff is
Optimize, obviously
Trile Editor (Need writing in FmbLib)
