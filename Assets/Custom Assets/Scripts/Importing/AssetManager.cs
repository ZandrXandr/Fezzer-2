using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using FmbLib;
using FezEngine.Structure;
using FezEngine.Structure.Geometry;

public class AssetManager {


    //Main instance
    private static AssetManager _main;

    public static AssetManager Main {
        get{
            if (_main==null)
                _main=new AssetManager();
            return _main;
        }
    }

    //--------------------Data--------------------

    private static Dictionary<string, TrileSet> loadedSets = new Dictionary<string, TrileSet>();
    private static Dictionary<string, Level> loadedLevels = new Dictionary<string, Level>();
    private static Dictionary<string, ArtObject> loadedArtObjects = new Dictionary<string, ArtObject>();


    //-------Trile Sets-------
    public static void LoadTrileSet(string toLoad) {

        if (IsSetLoaded(toLoad))
            return;

        string filePathXNB = OutputPath.OutputPathDir+"trile sets/"+toLoad+".xnb";

        if(!File.Exists(filePathXNB) ) {
            Debug.LogError("No model!");
            return;
        }


        //---Set loader---
        TrileSet loadedSet = (TrileSet)FmbUtil.ReadObject(filePathXNB);

        loadedSet.TextureAtlas.filterMode=FilterMode.Point;

        /*//---Image loader---
        Texture2D tilesetTexture = new Texture2D(1, 1);
        byte[] image = File.ReadAllBytes(texturePath);

        tilesetTexture.LoadImage(image);
        tilesetTexture.filterMode=FilterMode.Point;

        //Assign texture to set
        //TODO, add materials somehow so we don't always need new ones
        loadedSet.TextureAtlas=tilesetTexture;*/

        loadedSets.Add(toLoad, loadedSet);
    }

    public static bool IsSetLoaded(string set) {
        return loadedSets.ContainsKey(set);
    }

    public static TrileSet GetLoadedSet(string set) {
        if (!IsSetLoaded(set))
            LoadTrileSet(set);
        return loadedSets[set];
    }
    //-------Levels-------
    public static void LoadLevel(string toLoad) {
        if (IsLevelLoaded(toLoad))
            return;

        string filePathXNB = OutputPath.OutputPathDir+"levels/"+toLoad+".xnb";

        if (!File.Exists(filePathXNB)) {
            Debug.LogError("No level called that!");
            return;
        }

        //---Loader---
        Level loadedLevel = (Level)FmbUtil.ReadObject(filePathXNB);
        loadedLevels.Add(toLoad,loadedLevel);
    }

    public static bool IsLevelLoaded(string level) {
        return loadedSets.ContainsKey(level);
    }

    public static Level GetLevel(string level) {
        if (!IsLevelLoaded(level))
            LoadLevel(level);
        return loadedLevels[level];
    }

    //-------Art Objects-------
    public static void LoadAO(string toLoad) {
        if (IsAOLoaded(toLoad))
            return;

        string filePathXNB = OutputPath.OutputPathDir+"art objects/"+toLoad+".xnb";

        if (!File.Exists(filePathXNB)) {
            Debug.LogError("No model!");
            return;
        }

        //---AO loader---
        ArtObject loaded = (ArtObject)FmbUtil.ReadObject(filePathXNB);
        loaded.Cubemap.filterMode=FilterMode.Point;

        /*/---Image loader---
        Texture2D texture = new Texture2D(1, 1);
        byte[] image = File.ReadAllBytes(texturePath);

        texture.LoadImage(image);
        texture.filterMode=FilterMode.Point;

        //Assign texture to set
        //TODO, add materials somehow so we don't always need new ones
        loaded.Cubemap=texture;*/

        loadedArtObjects.Add(toLoad,loaded);
    }

    public static bool IsAOLoaded(string ao) {
        return loadedArtObjects.ContainsKey(ao);
    }

    public static ArtObject GetAO(string ao) {
        if (!IsAOLoaded(ao))
            LoadAO(ao);
        return loadedArtObjects[ao];
    }

}
