using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FezEngine.Structure;

public class CameraEditor : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKeyDown(KeyCode.L))
            LevelManager.Instance.LoadLevel();

        if (Input.GetKey(KeyCode.LeftShift))
            return;

        if (Input.GetMouseButtonDown(0)) {
            RaycastHit rh;

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition),out rh)) {

                Debug.DrawLine(rh.point,transform.position,Color.red,15f);

                TrileEmplacement place = new TrileEmplacement((int)rh.transform.position.x,(int)rh.transform.position.y,(int)rh.transform.position.z);
                LevelManager.Instance.RemoveTrile(place);

            }


        }

        if (Input.GetMouseButtonDown(1)) {
            RaycastHit rh;

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out rh)) {

                Debug.DrawLine(rh.point, transform.position, Color.green, 15f);

                TrileEmplacement place = new TrileEmplacement((int)(rh.transform.position.x+rh.normal.x), (int)(rh.transform.position.y+rh.normal.y), (int)(rh.transform.position.z+rh.normal.z));
                LevelManager.Instance.AddTrile(place);

            }


        }

    }
}
