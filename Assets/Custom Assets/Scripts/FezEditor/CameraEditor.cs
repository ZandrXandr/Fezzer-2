using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FezEngine.Structure;

public class CameraEditor : MonoBehaviour {

    public enum EditMode {
        WorldEditor,
        PropertiesEditor
    }

    public EditMode myMode;

	// Use this for initialization
	void Start () {
        resourcePath.text=PlayerPrefs.GetString("path");
	}
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKeyDown(KeyCode.Y)) {
            Application.CaptureScreenshot(Application.dataPath+"Screenshot.png",3);
        }

        if (Input.GetKey(KeyCode.LeftShift))
            return;

        if (myMode==EditMode.WorldEditor) {
            WorldEdtiorMode();
        } else if(myMode==EditMode.PropertiesEditor){
            PropertiesEditorMode();
        }

    }

    [SerializeField]
    UnityEngine.UI.InputField resourcePath;

    void OnApplicationQuit() {
        PlayerPrefs.SetString("path",resourcePath.text);
        PlayerPrefs.Save();
    }

    void WorldEdtiorMode() {

        if (!Input.GetKey(KeyCode.LeftControl))
            return;

        if (Input.GetKeyDown(KeyCode.S)) {
            LevelManager.Instance.SaveLevel();
        }

        if (Input.GetMouseButtonDown(0)) {
            RaycastHit rh;

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out rh)) {

                Debug.DrawLine(rh.point, transform.position, Color.red, 15f);

                TrileEmplacement place = new TrileEmplacement((int)rh.transform.position.x, (int)rh.transform.position.y, (int)rh.transform.position.z);
                LevelManager.Instance.RemoveTrile(place);

            }


        }

        if (Input.GetMouseButtonDown(1)) {
            RaycastHit rh;

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out rh)) {

                Debug.DrawLine(rh.point, transform.position, Color.green, 15f);
                Debug.DrawLine(rh.point, rh.point+rh.normal, Color.blue, 15f);

                TrileEmplacement place = new TrileEmplacement(Mathf.RoundToInt(rh.transform.position.x+rh.normal.x), Mathf.RoundToInt(rh.transform.position.y+rh.normal.y), Mathf.RoundToInt(rh.transform.position.z+rh.normal.z));
                LevelManager.Instance.AddTrile(place);

            }
        }
    }

    public TrileImported lastTrile;
    public ArtObjectImported lastAO;

    public bool isTrile;

    void PropertiesEditorMode() {

        if (Input.GetMouseButtonDown(0)) {
            RaycastHit rh;

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out rh)) {
                if (rh.transform.tag=="Trile") {
                    lastTrile=rh.transform.GetComponent<TrileImported>();
                    isTrile=true;
                } else if (rh.transform.tag=="ArtObject") {
                    lastAO=rh.transform.GetComponent<ArtObjectImported>();
                    isTrile=false;
                }
            }
        } else if (Input.GetMouseButtonDown(1)) {
            PropertiesEditor.Instance.SelectLevel();
        }

        //If we right click
        if (Input.GetMouseButton(2)) {
            if (isTrile) {
                if (lastTrile==null)
                    return;

                lastTrile.transform.rotation=Quaternion.Euler(0,lastTrile.transform.eulerAngles.y+Input.GetAxis("Mouse X"),0);

            } else {
                if (lastAO==null)
                    return;
            }
        }

        //If we let go of right click
        if (Input.GetMouseButtonUp(2)) {
            if (isTrile) {
                if (lastTrile==null)
                    return;

                lastTrile.transform.rotation=Quaternion.Euler(0, Mathf.RoundToInt(lastTrile.transform.eulerAngles.y/90)*90, 0);

            } else {
                if (lastAO==null)
                    return;
            }
        }


    }
}
