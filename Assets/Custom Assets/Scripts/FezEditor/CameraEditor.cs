using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraEditor : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKeyDown(KeyCode.L))
            LevelManager.Instance.LoadLevel();

	}
}
