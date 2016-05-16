using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FezEngine.Structure;
using System.Threading;
using FmbLib;
using System.IO;

public class TrileEditor : Singleton<TrileEditor> {

    public string setName { get; set; }
    public string resourcePath {
        get {
            return OutputPath.setPath;
        }
        set {
            OutputPath.setPath=value+"out/";
        }
    }

    public GameObject trilePrefab, aoPrefab, planePrefab;

    Dictionary<TrileEmplacement, GameObject> trileObjects = new Dictionary<TrileEmplacement, GameObject>();

    //Caching
    List<GameObject> trileObjectCache = new List<GameObject>();
    List<GameObject> aoObjectCache = new List<GameObject>();
    List<GameObject> planeObjectCache = new List<GameObject>();

    Dictionary<Trile, Mesh> trilesetCache = new Dictionary<Trile, Mesh>();
    Dictionary<ArtObject, Mesh> aoMeshCache = new Dictionary<ArtObject, Mesh>();
    Dictionary<string, ArtObject> aoCache = new Dictionary<string, ArtObject>();

    [HideInInspector]
    public static TrileSet s;

    Material setMat;

    int currTrileID;

    public void PickTrile(GameObject toPick) {
        currTrileID=int.Parse(toPick.name);
        ObjectProperties.Instance.SetToTrile(currTrileID);
        RotatingTrile.Instance.mf.mesh=trilesetCache[s.Triles[currTrileID]];
    }

    public void PickTrile(int id) {
        currTrileID=id;
        ObjectProperties.Instance.SetToTrile(currTrileID);
        RotatingTrile.Instance.mf.mesh=trilesetCache[s.Triles[currTrileID]];
    }

    public void LoadSet() {
        //Load the trile set 
        if (s==null) {
            s=FmbUtil.ReadObject<TrileSet>(OutputPath.OutputPathDir+"trile sets/"+setName.ToLower()+".xnb");
        } else if (s.Name!=setName) {
            s=FmbUtil.ReadObject<TrileSet>(OutputPath.OutputPathDir+"trile sets/"+setName.ToLower()+".xnb");
        }

        LoadSetMeshes();
    }

    public void LoadSetMeshes() {
        setMat=new Material(Shader.Find("Diffuse"));
        s.TextureAtlas.filterMode=FilterMode.Point;
        setMat.mainTexture=s.TextureAtlas;

        RotatingTrile.Instance.mr.material=setMat;
        PlacmentPreview.Instance.mr.material.mainTexture=setMat.mainTexture;

        foreach (Trile t in s.Triles.Values) {
            trilesetCache.Add(t, FezToUnity.TrileToMesh(t));
        }
    }

}
