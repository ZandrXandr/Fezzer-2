using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FezEngine.Structure;
using System.Threading;

public class LevelImported : Singleton<LevelImported> {

    Level loaded;

    public string levelPath;
    string setName;

    [SerializeField]
    bool manualLoad;

    public GameObject trilePrefab, aoPrefab;

    List<GameObject> tileCache = new List<GameObject>();
    List<GameObject> aoCache = new List<GameObject>();

    [SerializeField]
    TrileSet s;

    void Start() {
        OutputPath.isEditor=Application.isEditor;
        loadThread=new Thread(new ThreadStart(LoadLevelThread));
        loadThread.Start();
    }

    void Update() {
        if ((Application.isEditor&&manualLoad) || Input.GetKeyDown(KeyCode.L)) {
            manualLoad=false;
            LoadLevel();
        }
    }

    void LoadLevel() {
        StopCoroutine("LoadLevelCoroutine");
        UnloadLevel();
        StartCoroutine("LoadLevelCoroutine");
    }


    void OnDestroy() {
        loadThread.Abort();
    }

    Thread loadThread;
    [SerializeField]
    bool levelReady=true,trileSet=false;

    void LoadLevelThread() {
        while (true) {

            if (!levelReady) {
                loaded=AssetManager.GetLevel(levelPath);

                trileSet=true;
                while (trileSet)
                    Thread.Sleep(1);
                try {
                    CalculateLevelVisibility();
                } catch (System.Exception e) {
                    Debug.Log(e);
                }
                levelReady=true;
            }

            Thread.Sleep(15);
        }
    }

    Dictionary<TrileEmplacement, bool> visibility = new Dictionary<TrileEmplacement, bool>();

    void CalculateLevelVisibility() {

        visibility.Clear();

        List<TrileEmplacement> positions = new List<TrileEmplacement>();

        foreach (KeyValuePair<TrileEmplacement, TrileInstance> kvp in loaded.Triles) {

            if (kvp.Value.TrileId<0)
                continue;

            positions.Clear();

            TrileEmplacement curr = kvp.Key;
            TrileEmplacement f = new TrileEmplacement(curr.X,curr.Y,curr.Z+1);
            TrileEmplacement b = new TrileEmplacement(curr.X, curr.Y, curr.Z-1);
            TrileEmplacement r = new TrileEmplacement(curr.X+1, curr.Y, curr.Z);
            TrileEmplacement l = new TrileEmplacement(curr.X-1, curr.Y, curr.Z);
            TrileEmplacement u = new TrileEmplacement(curr.X, curr.Y+1, curr.Z);
            TrileEmplacement d = new TrileEmplacement(curr.X, curr.Y-1, curr.Z);

            positions.Add(f);
            positions.Add(b);
            positions.Add(l);
            positions.Add(r);
            positions.Add(u);
            positions.Add(d);

            bool isBreak=false;

            foreach(TrileEmplacement e in positions) {

                bool exists = loaded.Triles.ContainsKey(e);

                if (!exists) {
                    visibility.Add(curr,true);
                    isBreak=true;
                    break;
                } else {

                    if (loaded.Triles[e].TrileId<0) {
                        visibility.Add(curr, false);
                        isBreak=true;
                        break;
                    }
                        
                    Trile nextTrile = s.Triles[loaded.Triles[e].TrileId];

                    if(nextTrile.SeeThrough||loaded.Triles[e].ForceSeeThrough) {
                        visibility.Add(curr, true);
                        isBreak=true;
                        break;
                    }

                }

            }
            if (isBreak)
                continue;

            visibility.Add(curr,false);

        }
    }

    [SerializeField]
    AudioSource source;

    IEnumerator LoadLevelCoroutine() {

        int iterationNumber = 0;

        levelReady=false;

        {
            yield return new WaitForSeconds(1);

            while (!trileSet)
                yield return new WaitForEndOfFrame();

            s=AssetManager.GetLoadedSet(loaded.TrileSetName.ToLower());
            trileSet=false;
        }

        {
            while (!levelReady)
                yield return new WaitForEndOfFrame();

            setName=loaded.TrileSetName.ToLower();
        }

        {

            Debug.Log("Level name is:" + loaded.Name);
            Debug.Log("Level sky name is:" + loaded.SkyName);
            Debug.Log("Level song name is:"+loaded.SongName);

            LoadLevelMusic();
        }

        foreach (KeyValuePair<TrileEmplacement, TrileInstance> t in loaded.Triles) {

            if (t.Value.TrileId<0)
                continue;

            if (!visibility[t.Key] )
                continue;

            iterationNumber++;

            if (iterationNumber>15) {
                yield return new WaitForEndOfFrame();
                iterationNumber=0;
            }

            GenerateTrile(t.Value);

        }

        foreach (KeyValuePair<int, ArtObjectInstance> kvp in loaded.ArtObjects) {

            iterationNumber++;

            if (iterationNumber>=5) {
                yield return new WaitForEndOfFrame();
                iterationNumber=0;
            }

            GenerateAO(kvp.Value);
        }

        yield return new WaitForEndOfFrame();
    }

    void LoadLevelMusic() {
        StartCoroutine(LoadMusic());
    }

    IEnumerator LoadMusic() {
        WWW load = new WWW("file:///"+OutputPath.OutputPathDirExport + "music/grave/bats.ogg");

        while (load.GetAudioClip(false).loadState!=AudioDataLoadState.Loaded)
            yield return new WaitForEndOfFrame();

        source.clip=load.GetAudioClip(false);
        source.Play();
    }
    
    void UnloadLevel() {

    }

    Vector3 placmentToVec(TrileEmplacement place) {
        return new Vector3(place.X,place.Y,place.Z);
    }

    Transform t;

    Dictionary<IntPos, TrileImported> trileGroups = new Dictionary<IntPos, TrileImported>();

    void GenerateTrile(TrileInstance instance) {

        if (t==null)
            t=new GameObject().transform;
        t.position=instance.Data.PositionPhi;
        t.transform.rotation=Quaternion.Euler(0, instance.Data.PositionPhi.w*Mathf.Rad2Deg,0);

        GetGroupFromPos(instance.Position/5).AddTrileToMesh(instance.Data.PositionPhi,s.Triles[instance.TrileId],t);
        
    }

    TrileImported GetGroupFromPos(Vector3 pos) {
        IntPos p = IntPos.Vector3ToIntPos(pos);

        TrileImported getImported;

        if (!trileGroups.ContainsKey(p)) {
            GameObject newImported = Instantiate(trilePrefab);
            getImported=newImported.GetComponent<TrileImported>();
            getImported.mr.material.mainTexture=s.TextureAtlas;
            getImported.transform.SetParent(transform.FindChild("Triles"));

            trileGroups.Add(p,getImported);
        } else {
            getImported=trileGroups[p];
        }

        return getImported;
    }

    void GenerateAO(ArtObjectInstance instance) {

        GameObject newAO;

        if (aoCache.Count>0) {
            newAO=aoCache[0];
            aoCache.Remove(newAO);
            newAO.SetActive(true);
        } else {
            newAO=Instantiate(aoPrefab);
        }

        newAO.transform.position=instance.Position;
        newAO.transform.rotation=instance.Rotation;
        newAO.transform.SetParent(transform.FindChild("ArtObjects"));

        ArtObjectImported aoi = newAO.GetComponent<ArtObjectImported>();

        aoi.ao=AssetManager.GetAO(instance.ArtObjectName.ToLower());
        newAO.transform.position-=Vector3.one/2;
        aoi.UpdateAO();
    }
}
