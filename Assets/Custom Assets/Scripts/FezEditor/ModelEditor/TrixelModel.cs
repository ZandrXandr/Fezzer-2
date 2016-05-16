using UnityEngine;
using System.Collections;
using System.Linq;
using FezEngine.Structure;

public class TrixelModel : MonoBehaviour {

    [SerializeField]
    GameObject ModelObject;

    MeshFilter meshFilter;
    MeshCollider meshCollider;

    [SerializeField]
    Vector2 TrixelUVPos;
    Vector2 lastOffset;

    Vector2 texSize;
    Vector2 uv;

    [SerializeField]
    ComputeShader correctionShader;

    [SerializeField]
    Texture _Input, _Output;

    public int trileID = 16;

    public bool[,,] data = new bool[16, 16, 16];

    void Start() {
        meshFilter=ModelObject.GetComponent<MeshFilter>();
        meshCollider=ModelObject.GetComponent<MeshCollider>();

        AssetManager.LoadTrileSet("tree");

        TrileSet getSet = AssetManager.GetLoadedSet("tree");
        Trile getTrile = getSet.Triles.Values.ToList()[trileID%getSet.Triles.Count];

        data = TrixelModelImporter.GetModelDataFromTrile(getTrile);
        meshFilter.mesh=TrixelModelGenerator.GetDataMesh(data,getTrile.AtlasOffset,getSet.TextureAtlas.texelSize);
        meshFilter.GetComponent<MeshRenderer>().material.mainTexture=getSet.TextureAtlas;
        meshCollider.sharedMesh=meshFilter.mesh;

        texSize=getSet.TextureAtlas.texelSize;
        uv=getTrile.AtlasOffset;
    }
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKey(KeyCode.E)) {
            if (Input.GetMouseButtonDown(0)) {
                RaycastHit rh;
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition),out rh)) {

                    Vector3 hitPointWorld = rh.point-((rh.normal/16/2)*1.1f);
                    IntPos hitPosArray = IntPos.Vector3ToIntPos((hitPointWorld*16)+Vector3.one*8);

                    Debug.DrawLine(rh.point,rh.point-((rh.normal/16/2)*1.1f),Color.blue, 15f,false);

                    Debug.Log(hitPosArray);

                    if (hitPosArray.isContained(0, 16)) {
                        data[hitPosArray.x, hitPosArray.y, hitPosArray.z]=false;

                        TrileSet getSet = AssetManager.GetLoadedSet("tree");
                        Trile getTrile = getSet.Triles.Values.ToList()[trileID%getSet.Triles.Count];
                        meshFilter.mesh=TrixelModelGenerator.GetDataMesh(data, getTrile.AtlasOffset, getSet.TextureAtlas.texelSize);
                        meshCollider.sharedMesh=null;
                        meshCollider.sharedMesh=meshFilter.mesh;
                    }
                }
            }
            if (Input.GetMouseButtonDown(1)) {
                RaycastHit rh;
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out rh)) {
                    Vector3 hitPointWorld = rh.point+((rh.normal/16/2)*1.1f);
                    IntPos hitPosArray = IntPos.Vector3ToIntPos((hitPointWorld*16)+Vector3.one*8);

                    Debug.DrawLine(rh.point, rh.point+((rh.normal/16/2)*1.1f), Color.blue, 15f,false);

                    Debug.Log(hitPosArray);

                    if (hitPosArray.isContained(0, 16)) {
                        data[hitPosArray.x, hitPosArray.y, hitPosArray.z]=true;

                        TrileSet getSet = AssetManager.GetLoadedSet("tree");
                        Trile getTrile = getSet.Triles.Values.ToList()[trileID%getSet.Triles.Count];
                        meshFilter.mesh=TrixelModelGenerator.GetDataMesh(data, getTrile.AtlasOffset, getSet.TextureAtlas.texelSize);
                        meshCollider.sharedMesh=null;
                        meshCollider.sharedMesh=meshFilter.mesh;
                    }
                }
            }
        }
	}

    RenderTexture correctedTexutre(Texture2D input) {
        RenderTexture rt = new RenderTexture(input.width,input.height,1);
        rt.enableRandomWrite=true;
        rt.Create();
        rt.filterMode=FilterMode.Point;
        rt.wrapMode=TextureWrapMode.Repeat;

        correctionShader.SetTexture(0, "_Input", input);
        correctionShader.SetTexture(0, "_Result", rt);

        correctionShader.SetInt("height",input.height);

        correctionShader.Dispatch(0,input.width,input.height,1);

        _Input=input;
        _Output=rt;

        return rt;
    }

    public bool drawVertInfo=true;

    public void OnGUI() {

        if (!drawVertInfo)
            return;

        Color[] labelColors = new Color[] {Color.green,Color.red,Color.blue,Color.black};

        for(int i = 0; i < meshFilter.mesh.vertices.Length; i+=4) {
            for(int j = 0; j < 4; j++) {

                Vector3 v = meshFilter.mesh.vertices[i+j];

                Vector2 screenPos = Camera.main.WorldToScreenPoint(transform.TransformPoint(v));
                Vector2 actualScreenPos = new Vector2(screenPos.x, Screen.height-screenPos.y);

                Rect r = new Rect(actualScreenPos, Vector2.one*100);

                Vector3 worldPos = ((v)+Vector3.one/2)*16;
                Vector2 uvPos = meshFilter.mesh.uv[i+j];
                uvPos=new Vector2((uvPos.x-uv.x)/texSize.x, (uvPos.y-uv.y)/texSize.y);

                GUI.color=labelColors[j];
                GUI.Label(r, worldPos.ToString()+"\n"+uvPos);
            }
        }
    }
}
