using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FezEngine.Structure;
using System;

public class TrixelModel : MonoBehaviour {

    [SerializeField]
    GameObject ModelObject;
    MeshFilter meshFilter;
    MeshCollider meshCollider;

    [SerializeField]
    ComputeShader correctionShader;

    //Data that's filled.
    public HashSet<IntPos> data = new HashSet<IntPos>();

    public EditableModelMode mode;

    public Trile trile {
        get {
            return ModelEditor.Instance.currentTrile;
        }
    }

    public TrileSet set {
        get {
            return ModelEditor.Instance.currentSet;
        }
    }

    void Start() {
        meshFilter=ModelObject.GetComponent<MeshFilter>();
        meshCollider=ModelObject.GetComponent<MeshCollider>();
    }
	
	// Update is called once per frame
	void Update () {

        if (mode==EditableModelMode.Trileset)
            return;

        if (Input.GetKey(KeyCode.E)) {
            if (Input.GetMouseButtonDown(0)) {
                RaycastHit rh;
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition),out rh)) {

                    Vector3 hitPointWorld = rh.point-((rh.normal/16/2)*1.1f);
                    IntPos hitPosArray = IntPos.Vector3ToIntPos((hitPointWorld*16)+Vector3.one*8);

                    if (hitPosArray.isContained(0, 16)) {
                        if (data.Contains(hitPosArray))
                            data.Remove(hitPosArray);

                        UpdateMesh();
                    }
                }
            }
            if (Input.GetMouseButtonDown(1)) {
                RaycastHit rh;
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out rh)) {
                    Vector3 hitPointWorld = rh.point+((rh.normal/16/2)*1.1f);
                    IntPos hitPosArray = IntPos.Vector3ToIntPos((hitPointWorld*16)+Vector3.one*8);

                    if (hitPosArray.isContained(0, 16)) {
                        if (!data.Contains(hitPosArray))
                            data.Add(hitPosArray);
                        UpdateMesh();
                    }
                }
            }
        }
	}

    internal void SetMeshCollider(Trile t) {
        TrixelModelImporter.SetMeshCollider(t);
    }

    internal void GenerateDataFromTrile() {

        bool[,,] getData = TrixelModelImporter.GetModelDataFromTrile(trile);

        data.Clear();

        for(int x = 0; x < 16; x++) {
            for (int y = 0; y<16; y++) {
                for (int z = 0; z<16; z++) {
                    if (getData[x, y, z])
                        data.Add(new IntPos(x,y,z));
                }
            }
        }

        UpdateMesh();
    }

    public void UpdateMesh() {
        meshFilter.mesh=TrixelModelGenerator.GetDataMesh(data, trile.AtlasOffset, set.TextureAtlas.texelSize);
        meshCollider.sharedMesh=null;
        meshCollider.sharedMesh=meshFilter.mesh;
        meshFilter.GetComponent<MeshRenderer>().material.mainTexture=set.TextureAtlas;
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

        return rt;
    }
}

public enum EditableModelMode {
    Trile,
    ArtObject,
    Trileset
}
