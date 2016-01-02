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

        if (Input.GetKeyDown(KeyCode.U))
            myMode=(EditMode)(((int)myMode+1)%2);

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

    float dist = 1;

    Plane objectDragging;

    void PropertiesEditorMode() {

        if (Input.GetMouseButtonDown(0)) {
            RaycastHit rh;

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out rh)) {
                if (rh.transform.tag=="Trile") {
                    lastTrile=rh.transform.GetComponent<TrileImported>();
                    drawSizeTrile=LevelManager.s.Triles[lastTrile.myInstance.TrileId].Size;
                    isTrile=true;
                } else if (rh.transform.tag=="ArtObject") {
                    lastAO=rh.transform.GetComponent<ArtObjectImported>();
                    drawSizeAO=LevelManager.Instance.getAOBounds(lastAO.myInstance.Id);
                    isTrile=false;
                }
            }
        } else if (Input.GetMouseButtonDown(1)) {
            //PropertiesEditor.Instance.SelectLevel();
        }

        //If we right click

        if (Input.GetMouseButtonDown(1)) {

            RaycastHit rh;

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out rh)) {
                objectDragging.SetNormalAndPosition(rh.normal,rh.point);
            }

            if (isTrile) {
                if (lastTrile==null)
                    return;

                lastTrile.GetComponent<BoxCollider>().enabled=false;

            } else {
                if (lastAO==null)
                    return;

                lastAO.GetComponent<BoxCollider>().enabled=true;
            }

        }

        if (Input.GetMouseButton(1)) {

            RaycastHit rh;

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out rh) && !Input.GetKey(KeyCode.Space)) {
                objectDragging.SetNormalAndPosition(rh.normal, rh.point);
            }

            if (isTrile) {
                if (lastTrile==null)
                    return;

                float pos;

                Ray cam = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (objectDragging.Raycast(cam, out pos)) {
                    lastTrile.transform.position=cam.GetPoint(pos)+(objectDragging.normal/2);
                }

            } else {
                if (lastAO==null)
                    return;

                float pos;

                Ray cam = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (objectDragging.Raycast(cam, out pos)) {
                    lastAO.transform.position=cam.GetPoint(pos)-(objectDragging.normal/2);
                }
            }
        }

        if (Input.GetMouseButtonUp(1)) {
            if (isTrile) {
                if (lastTrile==null)
                    return;

                lastTrile.transform.position=roundToGrid(1,lastTrile.transform.position);
                lastTrile.GetComponent<BoxCollider>().enabled=false;

            } else {
                if (lastAO==null)
                    return;

                lastAO.transform.position=roundToGrid(2,lastAO.transform.position);
                lastAO.GetComponent<BoxCollider>().enabled=true;
            }
        }

        //If we middle click
        if (Input.GetMouseButton(2)) {
            if (isTrile) {
                if (lastTrile==null)
                    return;

                lastTrile.transform.rotation=Quaternion.Euler(0,lastTrile.transform.eulerAngles.y-Input.GetAxis("Mouse X")*5,0);

            } else {
                if (lastAO==null)
                    return;

                lastAO.transform.rotation=Quaternion.Euler(0, lastAO.transform.eulerAngles.y-Input.GetAxis("Mouse X")*5, 0);
            }
        }

        //If we let go of middle click
        if (Input.GetMouseButtonUp(2)) {
            if (isTrile) {
                if (lastTrile==null)
                    return;

                lastTrile.transform.rotation=Quaternion.Euler(0, Mathf.RoundToInt(lastTrile.transform.eulerAngles.y/90)*90, 0);

            } else {
                if (lastAO==null)
                    return;

                lastAO.transform.rotation=Quaternion.Euler(0, Mathf.RoundToInt(lastAO.transform.eulerAngles.y/90)*90,0);
            }
        }


    }

    Vector3 drawSizeAO,drawSizeTrile;
    Vector3 aoCenter, trileCenter;
    Vector3[] vertsToDraw = {
     new Vector3(0,0,0) , new Vector3(0,1,0)
    ,new Vector3(0,0,0) , new Vector3(1,0,0)
    ,new Vector3(0,0,0) , new Vector3(0,0,1)
    ,new Vector3(1,1,1) , new Vector3(0,1,1)
    ,new Vector3(1,1,1) , new Vector3(1,1,0)
    ,new Vector3(1,1,1) , new Vector3(1,0,1)
    ,new Vector3(0,0,1) , new Vector3(1,0,1)
    ,new Vector3(1,0,0) , new Vector3(1,0,1)
    ,new Vector3(1,1,0) , new Vector3(1,1,1)
    ,new Vector3(0,1,1) , new Vector3(1,1,1)
    ,new Vector3(0,1,0) , new Vector3(0,1,1)
    ,new Vector3(0,1,0) , new Vector3(1,1,0)
    ,new Vector3(1,0,0) , new Vector3(1,1,0)
    ,new Vector3(0,0,1) , new Vector3(0,1,1)
    };

    [SerializeField]
    Material lineMat;

    void OnPostRender() {
        if (!lineMat) {
            Debug.LogError("Please Assign a material on the inspector");
            return;
        }
        GL.PushMatrix();
        lineMat.SetPass(0);
        GL.Begin(GL.LINES);

        //Draw cube
        {
            if (lastTrile!=null) {
                GL.Color(Color.green);
                for (int i = 0; i<vertsToDraw.Length; i++) {
                    Vector3 curr = vertsToDraw[i];
                    curr-=Vector3.one/2;
                    curr*=1.1f;
                    curr=new Vector3(curr.x*drawSizeTrile.x,curr.y*drawSizeTrile.y,curr.z*drawSizeTrile.z);

                    curr=lastTrile.transform.rotation*curr;
                    curr+=lastTrile.transform.position;

                    GL.Vertex(curr);
                }
            }

            if (lastAO!=null) {
                GL.Color(Color.green);
                for (int i = 0; i<vertsToDraw.Length; i++) {
                    Vector3 curr = vertsToDraw[i];
                    curr-=Vector3.one/2;
                    curr*=1.1f;
                    curr=new Vector3(curr.x*drawSizeAO.x, curr.y*drawSizeAO.y, curr.z*drawSizeAO.z);

                    curr=lastAO.transform.rotation*curr;
                    curr+=lastAO.transform.position;

                    GL.Vertex(curr);
                }
            }

        }
        GL.End();
        GL.PopMatrix();
    }

    Vector3 roundToGrid(float gridSize, Vector3 input) {
        int gridPosx = Mathf.RoundToInt(input.x*gridSize);
        int gridPosy = Mathf.RoundToInt(input.y*gridSize);
        int gridPosz = Mathf.RoundToInt(input.z*gridSize);

        return new Vector3((float)gridPosx/gridSize, (float)gridPosy/gridSize, (float)gridPosz/gridSize);
    }
}
