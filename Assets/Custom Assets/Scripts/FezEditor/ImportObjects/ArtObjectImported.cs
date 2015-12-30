using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FezEngine.Structure;

[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
public class ArtObjectImported : MonoBehaviour {

    public ArtObject ao;
    MeshFilter mf;
    MeshRenderer mr;

    void Awake() {
        mf=GetComponent<MeshFilter>();
        mr=GetComponent<MeshRenderer>();
    }

    public void UpdateAO() {
        mf.mesh=FezToUnity.ArtObjectToMesh(ao);
        mr.material.mainTexture=ao.Cubemap;
    }
	
}
