using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FezEngine.Structure;
using System.Threading;
using FmbLib;
using System.IO;

public class LevelManager : Singleton<LevelManager> {

    [HideInInspector]
    public Level loaded;

    public string levelName { get; set; }
    public string resourcePath {
        get {
            return OutputPath.setPath;
        }
        set {
            OutputPath.setPath=value+"out/";
        }
    }
    string setName;

    [SerializeField]
    bool manualLoad;
    public bool loadOverFrames;

    public GameObject trilePrefab, aoPrefab, planePrefab;

    Dictionary<TrileEmplacement, GameObject> trileObjects = new Dictionary<TrileEmplacement, GameObject>();

    //Caching
    List<GameObject> trileObjectCache = new List<GameObject>();
    List<GameObject> aoObjectCache = new List<GameObject>();
    List<GameObject> planeObjectCache = new List<GameObject>();

    Dictionary<Trile, Mesh> trilesetCache = new Dictionary<Trile, Mesh>();
    Dictionary<ArtObject, Mesh> aoMeshCache = new Dictionary<ArtObject,Mesh>();
    Dictionary<string, ArtObject> aoCache = new Dictionary<string, ArtObject>();

    [HideInInspector]
    public static TrileSet s;

    int currTrileID;

    public void PickTrile(GameObject toPick) {
        currTrileID=int.Parse(toPick.name);
        ObjectProperties.Instance.SetToTrile(currTrileID);
        PlacmentPreview.Instance.mf.mesh=trilesetCache[s.Triles[currTrileID]];
        PlacmentPreview.Instance.mr.material.mainTexture=s.TextureAtlas;
        RotatingTrile.Instance.mf.mesh=trilesetCache[s.Triles[currTrileID]];
        CameraEditor.placeMagnitude=getTrileBounds(currTrileID).size.magnitude*0.45f;
        CameraEditor.isTrile=true;
    }

    public void PickTrile(int id) {
        currTrileID=id;
        ObjectProperties.Instance.SetToTrile(currTrileID);
        PlacmentPreview.Instance.mf.mesh=trilesetCache[s.Triles[currTrileID]];
        PlacmentPreview.Instance.mr.material.mainTexture=s.TextureAtlas;
        RotatingTrile.Instance.mf.mesh=trilesetCache[s.Triles[currTrileID]];
        CameraEditor.placeMagnitude=getTrileBounds(currTrileID).size.magnitude*0.45f;
        CameraEditor.isTrile=true;
    }

    public void PickAO(GameObject toPick) {
        ObjectProperties.Instance.SetToAO(toPick.name);
        PlacmentPreview.Instance.mf.mesh=aoMeshCache[aoCache[toPick.name]];
        PlacmentPreview.Instance.mr.material.mainTexture=aoCache[toPick.name].Cubemap;
        CameraEditor.placeMagnitude=getAOBounds(toPick.name).size.magnitude*0.45f;
        CameraEditor.isTrile=false;
        CameraEditor.aoName=toPick.name;
    }

    public void PickAO(string id) {
        ObjectProperties.Instance.SetToAO(id);
        PlacmentPreview.Instance.mf.mesh=aoMeshCache[aoCache[id]];
        PlacmentPreview.Instance.mr.material.mainTexture=aoCache[id].Cubemap;
        CameraEditor.placeMagnitude=getAOBounds(id).size.magnitude*0.45f;
        CameraEditor.isTrile=false;
        CameraEditor.aoName=id;
    }

    public void LoadLevel() {
        UnloadLevel();
        LoadLevel(levelName);
    }

    public void ChangeAOKeyTo(string prev, string curr) {
        if (!aoCache.ContainsKey(prev) && !aoCache.ContainsKey(curr))
            return;
        ArtObject reference =aoCache[prev];

        aoCache.Remove(prev);
        aoCache.Add(curr,reference);
    }

    public ArtObject GetAO(string name) {
        return aoCache[name];
    }
    public Trile GetTrile(int id) {
        return s.Triles[id];
    }

    void UnloadLevel() {
        Transform triles = transform.FindChild("Triles");
        Transform artObjects = transform.FindChild("ArtObjects");
        Transform planeObjects = transform.FindChild("Planes");
        Transform trileCache = transform.FindChild("TrileCache");
        Transform aoOBJCache = transform.FindChild("AOCache");
        Transform planeCache = transform.FindChild("PlaneCache");

        while (triles.childCount>0) {
            Transform t = triles.GetChild(0);
            t.SetParent(trileCache);
            t.gameObject.SetActive(false);
            trileObjectCache.Add(t.gameObject);
        }
        while (artObjects.childCount>0) {
            Transform t = artObjects.GetChild(0);
            t.SetParent(aoOBJCache);
            t.gameObject.SetActive(false);
            aoObjectCache.Add(t.gameObject);
        }
        while (planeObjects.childCount>0) {
            Transform t = planeObjects.GetChild(0);
            t.SetParent(planeCache);
            t.gameObject.SetActive(false);
            planeObjectCache.Add(t.gameObject);
        }

        List<GameObject> toDestroy = new List<GameObject>();

        for(int i = 0; i < toParentTrile.childCount; i++) {
            toDestroy.Add(toParentTrile.GetChild(i).gameObject);
        }

        while (toDestroy.Count>0) {
            Destroy(toDestroy[0]);
            toDestroy.RemoveAt(0);
        }

        visibility.Clear();
        trilesetCache.Clear();
    }

    [SerializeField]
    bool isFMB;

    public void LoadLevel(string name) {

        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        sw.Start();

        if(isFMB)
            loaded=FmbUtil.ReadObject<Level>("F:/Fezzer 2/FZ2 Export/"+levelName+".fmb");
        else
            loaded=FmbUtil.ReadObject<Level>(OutputPath.OutputPathDir+"levels/"+levelName+".xnb");

        //Load the trile set 
        if (s==null) {
            s=FmbUtil.ReadObject<TrileSet>(OutputPath.OutputPathDir+"trile sets/"+loaded.TrileSetName.ToLower()+".xnb");
        } else if (s.Name!=loaded.TrileSetName) {
            s=FmbUtil.ReadObject<TrileSet>(OutputPath.OutputPathDir+"trile sets/"+loaded.TrileSetName.ToLower()+".xnb");
        }
      

        sw.Stop();
        Debug.Log("Level loaded in about "+sw.ElapsedMilliseconds.ToString() + " ms");

        currTrileID=s.Triles.Keys.First();

        LoadUsedArtObjects();

        //Create meshes for triles
        LoadSetMeshes();
        LoadAOMeshes();

        StartCoroutine(LoadLevelCoroutine());
        ListTrilesUnderUI();
        ListAOUnderUI();
        SkyColorManager.Instance.Load();
    }

    public void LoadSetMeshes() {
        setMat=new Material(Shader.Find("Diffuse"));
        s.TextureAtlas.filterMode=FilterMode.Point;
        setMat.mainTexture=s.TextureAtlas;

        RotatingTrile.Instance.mr.material=setMat;
        PlacmentPreview.Instance.mr.material.mainTexture=setMat.mainTexture;

        foreach(Trile t in s.Triles.Values) {
            trilesetCache.Add(t,FezToUnity.TrileToMesh(t));
        }
    }

    public void LoadUsedArtObjects() {
        foreach (KeyValuePair<int, ArtObjectInstance> ao in loaded.ArtObjects) {

            string path = OutputPath.OutputPathDir+"art objects/"+ao.Value.ArtObjectName.ToLower()+".xnb";

            ArtObject aoL = FmbUtil.ReadObject<ArtObject>(path);
            if (aoCache.ContainsKey(aoL.Name)) {
                continue;
            } else aoCache.Add(aoL.Name,aoL);
        }
    }

    public void LoadAOMeshes() {
        foreach(ArtObjectInstance ao in loaded.ArtObjects.Values) {
            ArtObject aoL = aoCache[ao.ArtObjectName];
            if (aoMeshCache.ContainsKey(aoL))
                continue;
            aoMeshCache.Add(aoL,FezToUnity.ArtObjectToMesh(aoL));
        }
    }

    Dictionary<TrileEmplacement, bool> visibility = new Dictionary<TrileEmplacement, bool>();

    Material setMat;

    IEnumerator LoadLevelCoroutine() {

        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        sw.Start();

        yield return new WaitForEndOfFrame();

        //Calculate level visibility
        foreach (KeyValuePair<TrileEmplacement, TrileInstance> kvp in loaded.Triles) {

            TrileEmplacement currP = kvp.Key;

            if (kvp.Value.TrileId<0) {
                visibility.Add(currP, false);
                continue;
            }

            if (kvp.Value.ForceSeeThrough||s.Triles[kvp.Value.TrileId].SeeThrough) {
                visibility.Add(currP,true);
                continue;
            }

            TrileEmplacement[] checkPos = new TrileEmplacement[6];

            checkPos[0]=new TrileEmplacement(currP.X, currP.Y, currP.Z+1);
            checkPos[1]=new TrileEmplacement(currP.X, currP.Y, currP.Z-1);

            checkPos[2]=new TrileEmplacement(currP.X, currP.Y+1, currP.Z);
            checkPos[3]=new TrileEmplacement(currP.X, currP.Y-1, currP.Z);

            checkPos[4]=new TrileEmplacement(currP.X+1, currP.Y, currP.Z);
            checkPos[5]=new TrileEmplacement(currP.X-1, currP.Y, currP.Z);

            bool isVisible=false;

            foreach (TrileEmplacement pos in checkPos) {

                if (!loaded.Triles.ContainsKey(pos)) {
                    visibility.Add(currP, true);
                    isVisible=true;
                    break;
                } else if (loaded.Triles[pos].TrileId<0) {
                    visibility.Add(currP,true);
                    isVisible=true;
                    break;
                } else if (loaded.Triles[pos].ForceSeeThrough||s.Triles[loaded.Triles[pos].TrileId].SeeThrough) {
                    visibility.Add(currP, true);
                    isVisible=true;
                    break;
                }
            }
            if (!isVisible) {
                visibility.Add(currP,false);
            }

        }

        int index = 0;

        //Generate triles
        {

            foreach (KeyValuePair<TrileEmplacement, TrileInstance> kvp in loaded.Triles) {
                if (!visibility[kvp.Key])
                    continue;

                GameObject g = NewTrileObject(kvp.Key);

                index++;

                if (index>50) {
                    index=0;
                    if(loadOverFrames)
                        yield return new WaitForEndOfFrame();
                }
            }

            //Generate Planes
            {
                foreach(KeyValuePair<int, BackgroundPlane> kvp in loaded.BackgroundPlanes) {

                    BackgroundPlane b = kvp.Value;

                    if (b.Hidden || b.Animated || !b.Visible)
                        continue;

                    GameObject newPlane;

                    if (planeObjectCache.Count>0) {
                        newPlane=planeObjectCache[0];
                        planeObjectCache.RemoveAt(0);
                        newPlane.gameObject.SetActive(true);
                    } else {
                        newPlane=Instantiate(planePrefab);
                    }

                    newPlane.transform.rotation=b.Rotation;
                    newPlane.transform.position=b.Position-(Vector3.one/2);
                    newPlane.transform.localScale=new Vector3(-b.Size.x,-b.Size.y,b.Size.z);

                    MeshRenderer mr = newPlane.GetComponent<MeshRenderer>();

                    try {
                        Texture2D tex = FmbUtil.ReadObject<Texture2D>(OutputPath.OutputPathDir+"background planes/"+b.TextureName.ToLower()+".xnb");

                        if (tex!=null) {
                            //tex.alphaIsTransparency=true;
                            mr.material.mainTexture=tex;
                            mr.material.mainTexture.filterMode=FilterMode.Point;
                        } else
                            Debug.Log("Tex Null!");
                    } catch(System.Exception e) {
                        Debug.Log(e);
                        Debug.Log(OutputPath.OutputPathDir+"background planes/"+b.TextureName.ToLower()+".xnb");
                        Destroy(newPlane);
                        continue;
                    }

                    newPlane.name=b.TextureName;

                    index++;

                    if (index>5) {
                        index=0;
                        if(loadOverFrames)
                            yield return new WaitForEndOfFrame();
                    }

                    newPlane.transform.rotation=Quaternion.Euler(newPlane.transform.eulerAngles.x,newPlane.transform.eulerAngles.y-180,newPlane.transform.eulerAngles.z);
                    newPlane.transform.parent=transform.FindChild("Planes");
                }

            }

            //Generate Art Objects
            {

                foreach (KeyValuePair<int, ArtObjectInstance> kvp in loaded.ArtObjects) {

                    GameObject newTrile;

                    if (aoObjectCache.Count>0) {
                        newTrile=aoObjectCache[0];
                        aoObjectCache.RemoveAt(0);
                        newTrile.SetActive(true);
                    } else {
                        newTrile=Instantiate(aoPrefab);
                    }

                    MeshFilter mf = newTrile.GetComponent<MeshFilter>();
                    MeshRenderer mr = newTrile.GetComponent<MeshRenderer>();
                    MeshCollider mc = newTrile.GetComponent<MeshCollider>();
                    ArtObjectImported aoI = newTrile.GetComponent<ArtObjectImported>();

                    aoI.myInstance=kvp.Value;

                    mr.material=FezToUnity.GeometryToMaterial(aoCache[kvp.Value.ArtObjectName].Cubemap);
                    mf.mesh=aoMeshCache[aoCache[kvp.Value.ArtObjectName]];
                    mc.sharedMesh=aoMeshCache[aoCache[kvp.Value.ArtObjectName]];

                    newTrile.transform.position=kvp.Value.Position-(Vector3.one/2);
                    newTrile.transform.rotation=kvp.Value.Rotation;

                    index++;

                    if (index>5) {
                        index=0;
                        if(loadOverFrames)
                            yield return new WaitForEndOfFrame();
                    }

                    newTrile.name=kvp.Value.ArtObjectName;
                    newTrile.transform.parent=transform.FindChild("ArtObjects");
                }
            }
        }

        sw.Stop();
        Debug.Log("Objects created in " + sw.ElapsedMilliseconds + " ms");
    }

    GameObject NewTrileObject(TrileEmplacement atPos) {
        TrileInstance instance;

        if (loaded.Triles.ContainsKey(atPos)) {
            instance=loaded.Triles[atPos];
        } else {
            instance = new TrileInstance();
            instance.TrileId=currTrileID;
            instance.Position=new Vector3(atPos.X, atPos.Y, atPos.Z);
            loaded.Triles.Add(atPos,instance);
        }

        GameObject newTrile;

        if (trileObjectCache.Count>0) {
            newTrile= trileObjectCache[0];
            trileObjectCache.RemoveAt(0);
            newTrile.SetActive(true);
        } else {
            newTrile=Instantiate(trilePrefab);
        }

        MeshFilter mf = newTrile.GetComponent<MeshFilter>();
        MeshRenderer mr = newTrile.GetComponent<MeshRenderer>();
        BoxCollider bc = newTrile.GetComponent<BoxCollider>();
        TrileImported tI = newTrile.GetComponent<TrileImported>();

        tI.myInstance=instance;

        mr.material=setMat;
        mf.mesh=trilesetCache[s.Triles[instance.TrileId]];

        newTrile.transform.position=new Vector3(atPos.X, atPos.Y, atPos.Z);
        newTrile.transform.rotation=Quaternion.Euler(0, Mathf.Rad2Deg*instance.Data.PositionPhi.w, 0);

        newTrile.name=s.Triles[instance.TrileId].Name;
        newTrile.transform.parent=transform.FindChild("Triles");
        if (!trileObjects.ContainsKey(atPos))
            trileObjects.Add(atPos, newTrile);
        else
            trileObjects[atPos]=newTrile;
        return newTrile;
    }

    public void RemoveTrile(TrileEmplacement atPos) {
        if (!loaded.Triles.ContainsKey(atPos))
            return;
        loaded.Triles.Remove(atPos);
        
        Destroy(trileObjects[atPos]);
        trileObjects.Remove(atPos);

        TrileEmplacement[] checkPos = new TrileEmplacement[6];

        checkPos[0]=new TrileEmplacement(atPos.X, atPos.Y, atPos.Z+1);
        checkPos[1]=new TrileEmplacement(atPos.X, atPos.Y, atPos.Z-1);

        checkPos[2]=new TrileEmplacement(atPos.X, atPos.Y+1, atPos.Z);
        checkPos[3]=new TrileEmplacement(atPos.X, atPos.Y-1, atPos.Z);

        checkPos[4]=new TrileEmplacement(atPos.X+1, atPos.Y, atPos.Z);
        checkPos[5]=new TrileEmplacement(atPos.X-1, atPos.Y, atPos.Z);

        foreach (TrileEmplacement t in checkPos) {
            GenerateTrile(t);
        }
    }

    void UpdateCulling(TrileEmplacement around) {
        TrileEmplacement[] checkPos = new TrileEmplacement[6];

        checkPos[0]=new TrileEmplacement(around.X, around.Y, around.Z+1);
        checkPos[1]=new TrileEmplacement(around.X, around.Y, around.Z-1);

        checkPos[2]=new TrileEmplacement(around.X, around.Y+1, around.Z);
        checkPos[3]=new TrileEmplacement(around.X, around.Y-1, around.Z);

        checkPos[4]=new TrileEmplacement(around.X+1, around.Y, around.Z);
        checkPos[5]=new TrileEmplacement(around.X-1, around.Y, around.Z);

        foreach (TrileEmplacement t in checkPos) {
            GenerateTrile(t);
        }
    }

    void GenerateTrile(TrileEmplacement atPos) {

        if (!loaded.Triles.ContainsKey(atPos)) {
            return;
        }

        bool visible = isVisible(atPos);

        if (!visible) {
            if (trileObjects.ContainsKey(atPos)) {

                GameObject toRemove = trileObjects[atPos];

                trileObjects.Remove(atPos);

                Destroy(toRemove);
            }
        } else {
            if (trileObjects.ContainsKey(atPos))
                return;
            else {
                GameObject g = NewTrileObject(atPos);
            }
        }
    }

    TrileEmplacement[] checkPos = new TrileEmplacement[6];

    bool isVisible(TrileEmplacement atPos) {

        TrileEmplacement[] checkPos = new TrileEmplacement[6];

        checkPos[0]=new TrileEmplacement(atPos.X, atPos.Y, atPos.Z+1);
        checkPos[1]=new TrileEmplacement(atPos.X, atPos.Y, atPos.Z-1);

        checkPos[2]=new TrileEmplacement(atPos.X, atPos.Y+1, atPos.Z);
        checkPos[3]=new TrileEmplacement(atPos.X, atPos.Y-1, atPos.Z);

        checkPos[4]=new TrileEmplacement(atPos.X+1, atPos.Y, atPos.Z);
        checkPos[5]=new TrileEmplacement(atPos.X-1, atPos.Y, atPos.Z);

        foreach (TrileEmplacement t in checkPos) {
            if (!loaded.Triles.ContainsKey(t))
                return true;
            else if (s.Triles[loaded.Triles[t].TrileId].SeeThrough)
                return true;
        }

        return false;
    }

    public void RegenTrile(TrileEmplacement trilePos) {
        if (loaded.Triles.ContainsKey(trilePos))
            return;

        GameObject genTrile = NewTrileObject(trilePos);

        UpdateCulling(trilePos);
    }

    public void GenerateAO(Vector3 pos, string name) {
        if (!aoCache.ContainsKey(name))
            return;

        ArtObject ao = aoCache[name];

        ArtObjectInstance newImport = new ArtObjectInstance();

        newImport.ArtObjectName=name;
        newImport.Position=pos;
        newImport.Scale=Vector3.one;

        GameObject newTrile;

        if (aoObjectCache.Count>0) {
            newTrile=aoObjectCache[0];
            aoObjectCache.RemoveAt(0);
            newTrile.SetActive(true);
        } else {
            newTrile=Instantiate(aoPrefab);
        }

        MeshFilter mf = newTrile.GetComponent<MeshFilter>();
        MeshRenderer mr = newTrile.GetComponent<MeshRenderer>();
        MeshCollider mc = newTrile.GetComponent<MeshCollider>();
        ArtObjectImported aoI = newTrile.GetComponent<ArtObjectImported>();

        aoI.myInstance=newImport;

        mr.material=FezToUnity.GeometryToMaterial(aoCache[name].Cubemap);
        mf.mesh=aoMeshCache[aoCache[name]];
        mc.sharedMesh=aoMeshCache[aoCache[name]];

        newTrile.transform.position=pos;
        newTrile.transform.rotation=newImport.Rotation;

        newTrile.name=name;
        newTrile.transform.parent=transform.FindChild("ArtObjects");

    }

    [SerializeField]
    RectTransform toParentTrile,toParentAO;
    [SerializeField]
    GameObject trileButtonPrefab,aoButtonPrefab;

    int columnCount = 4;

    void ListTrilesUnderUI() {

        int trileSize = 15;

        foreach (KeyValuePair<int, Trile> t in s.Triles) {

            GameObject newButton = Instantiate(trileButtonPrefab);
            newButton.transform.SetParent(toParentTrile);

            newButton.transform.name=t.Key.ToString();

            Texture2D newTexture = new Texture2D(trileSize, trileSize);

            int pX = Mathf.FloorToInt(s.TextureAtlas.width*t.Value.AtlasOffset.x);
            int pY = Mathf.FloorToInt(s.TextureAtlas.height*t.Value.AtlasOffset.y);

            for (int x = 1; x<=trileSize; x++) {
                for (int y = 0; y<trileSize; y++) {

                    Color c = s.TextureAtlas.GetPixel(pX+x, pY+y);
                    c.a=1;

                    newTexture.SetPixel((trileSize-x)%trileSize,trileSize-y,c);
                }
            }

            newTexture.Apply();
            newTexture.filterMode=FilterMode.Point;
            newTexture.wrapMode=TextureWrapMode.Clamp;

            newButton.transform.GetChild(0).GetComponent<RawImage>().texture=newTexture;
            newButton.transform.GetChild(1).GetComponent<Text>().text=t.Key.ToString();
        }

    }

    void ListAOUnderUI() {

        HashSet<ArtObject> generatedAOs = new HashSet<ArtObject>();

        foreach (KeyValuePair<string, ArtObject> kvp in aoCache) {
            if (generatedAOs.Contains(kvp.Value))
                continue;

            GameObject newButton = Instantiate(aoButtonPrefab);
            newButton.transform.SetParent(toParentAO);

            newButton.transform.name=kvp.Value.Name;

            int aoSize = kvp.Value.Cubemap.height;

            Texture2D aoTex = new Texture2D(aoSize, aoSize);

            for (int x = 0; x<aoSize; x++) {
                for (int y = 0; y<aoSize; y++) {
                    Color c = kvp.Value.Cubemap.GetPixel(x,y);

                    c.a=1;

                    aoTex.SetPixel(x,aoSize-y,c);
                }
            }

            aoTex.Apply();
            aoTex.filterMode=FilterMode.Point;
            aoTex.wrapMode=TextureWrapMode.Clamp;

            newButton.transform.GetChild(0).GetComponent<RawImage>().texture=aoTex;

            generatedAOs.Add(kvp.Value);
        }

    }

    public void SaveLevel() {

        Debug.Log("Saving");

        string dirpath = OutputPath.OutputPathDir;
        string file = levelName + ".fmb";

        dirpath = Path.GetFullPath(Path.Combine(dirpath, @"..\"));
        dirpath=dirpath+"FZ2 Export/";

        if (!Directory.Exists(dirpath))
            Directory.CreateDirectory(dirpath);

        Debug.Log(dirpath);


        if (File.Exists(dirpath+file)) {
            File.Delete(dirpath+file);
        }
        FmbUtil.WriteObject<Level>(dirpath+file,loaded);

    }

    public Bounds getAOBounds(string name) {
        return aoMeshCache[aoCache[name]].bounds;
    }
    public Bounds getTrileBounds(int id) {
        return trilesetCache[s.Triles[id]].bounds;
    }

    public void MoveTrile(TrileEmplacement from, TrileEmplacement to, TrileInstance move) {
        Debug.Log("Removing " + new Vector3(from.X,from.Y,from.Z) + " from triles");
        loaded.Triles.Remove(from);
        loaded.Triles.Add(to,move);

        //Update the culling of the triles around the position we came from and moved to.
        UpdateCulling(from);
        UpdateCulling(to);
    }

    public void MoveAO(int id, Vector3 moveTo) {
        loaded.ArtObjects[id].Position=moveTo;
    }

    public bool isOccupied(TrileEmplacement pos) {
        return loaded.Triles.ContainsKey(pos);
    }
}
