using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RotatingTrile : Singleton<RotatingTrile> {

    [SerializeField]
    float rotate;

    [HideInInspector]
    public MeshFilter mf;
    [HideInInspector]
    public MeshRenderer mr;
	
    void Start() {
        transform.rotation=Quaternion.identity;
        mf=GetComponent<MeshFilter>();
        mr=GetComponent<MeshRenderer>();
    }

	// Update is called once per frame
	void Update () {

        transform.Rotate(0,rotate*Time.deltaTime,0);
	
	}
}
