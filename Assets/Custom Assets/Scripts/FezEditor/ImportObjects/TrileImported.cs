using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FezEngine.Structure;

[RequireComponent(typeof(MeshRenderer),typeof(MeshFilter))]
public class TrileImported : MonoBehaviour {

    MeshFilter mf;
    [HideInInspector]
    public MeshRenderer mr;

    public int trilesImported = 0;

    void Awake() {
        mf=GetComponent<MeshFilter>();
        mr=GetComponent<MeshRenderer>();
    }

    public void AddTrileToMesh(Vector3 pos, Trile trile,Transform t) {
        if (mf.mesh==null)
            mf.mesh=new Mesh();

        Mesh toMerge = FezToUnity.TrileToMesh(trile);
        CombineInstance[] combine = new CombineInstance[2];

        combine[0].mesh=toMerge;
        combine[0].transform=t.localToWorldMatrix;
        combine[1].mesh=mf.mesh;
        combine[1].transform=transform.localToWorldMatrix;

        Mesh result = new Mesh();

        result.CombineMeshes(combine);

        //Debug.Log(toMerge.vertexCount+"|"+mf.mesh.vertexCount+"|"+ result.vertexCount);

        mf.mesh=result;
        trilesImported++;
    }

}
