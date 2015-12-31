using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FezEngine.Structure;

[RequireComponent(typeof(MeshRenderer),typeof(MeshFilter))]
public class TrileImported : MonoBehaviour {

    MeshFilter mf;
    [HideInInspector]
    public MeshRenderer mr;

    void Awake() {
        mf=GetComponent<MeshFilter>();
        mr=GetComponent<MeshRenderer>();
    }



}
