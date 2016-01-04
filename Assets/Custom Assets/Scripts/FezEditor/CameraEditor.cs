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

    public static float placeMagnitude=1;

    public int setMode {
        set {
            myMode=(EditMode)value;
        }
    }

    bool isWire;
    public static string aoName;

	// Use this for initialization
	void Start () {
        resourcePath.text=PlayerPrefs.GetString("path");
	}

    void OnPreRender() {
        if(isWire)
            GL.wireframe=true;
    }

    // Update is called once per frame
    void Update () {

        if (Input.GetKeyDown(KeyCode.Period))
            isWire=!isWire;

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

        if (!Input.GetKey(KeyCode.E)) {
            if (PlacmentPreview.Instance.gameObject.activeSelf)
                PlacmentPreview.Instance.gameObject.SetActive(false);
            return;
        }

        //Trile preview
        {
            RaycastHit rh;

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out rh)) {
                if (!PlacmentPreview.Instance.gameObject.activeSelf)
                    PlacmentPreview.Instance.gameObject.SetActive(true);
                if(isTrile)
                    PlacmentPreview.Instance.transform.position=new Vector3(Mathf.RoundToInt(rh.point.x+((rh.normal.x)*placeMagnitude)), Mathf.RoundToInt(rh.point.y+((rh.normal.y)*placeMagnitude)), Mathf.RoundToInt(rh.point.z+((rh.normal.z)*placeMagnitude)));
                else
                    PlacmentPreview.Instance.transform.position=roundToGrid(16,new Vector3(rh.point.x+((rh.normal.x)*placeMagnitude), rh.point.y+((rh.normal.y)*placeMagnitude),rh.point.z+((rh.normal.z)*placeMagnitude)));
            } else if (PlacmentPreview.Instance.gameObject.activeSelf)
                PlacmentPreview.Instance.gameObject.SetActive(false);
        }

        if (Input.GetMouseButtonDown(0)) {
            RaycastHit rh;

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out rh)) {

                Debug.DrawLine(rh.point, transform.position, Color.red, 15f);

                if (isTrile) {
                    TrileEmplacement place = new TrileEmplacement((int)rh.transform.position.x, (int)rh.transform.position.y, (int)rh.transform.position.z);
                    LevelManager.Instance.RemoveTrile(place);
                } else {
                    Vector3 pos = roundToGrid(16, new Vector3(rh.point.x+((rh.normal.x)*placeMagnitude), rh.point.y+((rh.normal.y)*placeMagnitude), rh.point.z+((rh.normal.z)*placeMagnitude)));
                    LevelManager.Instance.GenerateAO(pos, aoName);
                }

            }


        }

        if (Input.GetMouseButtonDown(1)) {
            RaycastHit rh;

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out rh)) {

                Debug.DrawLine(rh.point, transform.position, Color.green, 15f);
                Debug.DrawLine(rh.point, rh.point+rh.normal, Color.blue, 15f);

                if (isTrile) {
                    TrileEmplacement place = new TrileEmplacement(Mathf.RoundToInt(rh.point.x+((rh.normal.x)*placeMagnitude)), Mathf.RoundToInt(rh.point.y+((rh.normal.y)*placeMagnitude)), Mathf.RoundToInt(rh.point.z+((rh.normal.z)*placeMagnitude)));
                    LevelManager.Instance.RegenTrile(place);
                }

            }
        }

        if (Input.GetMouseButtonDown(3)) {
            RaycastHit rh;

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out rh)) {
                if(rh.transform.tag=="Trile")
                    LevelManager.Instance.PickTrile(rh.transform.GetComponent<TrileImported>().myInstance.TrileId);
            }
        }
    }

    public TrileImported lastTrile;
    public ArtObjectImported lastAO;

    TrileEmplacement preTrileMove;

    public static bool isTrile;

    float dist = 1;

    float distOffset = 0;

    Plane objectDragging;

    void PropertiesEditorMode() {

        if (Input.GetMouseButtonDown(0)) {
            RaycastHit rh;

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out rh)) {
                if (rh.transform.tag=="Trile") {
                    lastTrile=rh.transform.GetComponent<TrileImported>();
                    trileBounds=LevelManager.Instance.getTrileBounds(lastTrile.myInstance.TrileId);
                    isTrile=true;
                    ObjectProperties.Instance.SetToTrile(lastTrile.myInstance.TrileId);
                } else if (rh.transform.tag=="ArtObject") {
                    lastAO=rh.transform.GetComponent<ArtObjectImported>();
                    aoBounds=LevelManager.Instance.getAOBounds(lastAO.myInstance.ArtObjectName);
                    isTrile=false;
                    ObjectProperties.Instance.SetToAO(lastAO.myInstance.ArtObjectName);
                }
            }
        } else if (Input.GetMouseButtonDown(1)) {
            //PropertiesEditor.Instance.SelectLevel();
        }

        //If we right click

        if (Input.GetMouseButtonDown(1)) {

            RaycastHit rh;

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out rh)) {

                if (rh.transform.tag=="Trile") {
                    if (lastTrile==null) {
                        isTrile=true;
                        lastTrile=rh.transform.GetComponent<TrileImported>();
                        trileBounds=LevelManager.Instance.getTrileBounds(lastTrile.myInstance.TrileId);

                    } else if (rh.transform!=lastTrile.transform) {
                        isTrile=true;
                        lastTrile=rh.transform.GetComponent<TrileImported>();
                        trileBounds=LevelManager.Instance.getTrileBounds(lastTrile.myInstance.TrileId);
                    }
                } else if (rh.transform.tag=="ArtObject") {
                    if (lastAO==null) {
                        isTrile=false;
                        lastAO=rh.transform.GetComponent<ArtObjectImported>();
                        aoBounds=LevelManager.Instance.getAOBounds(lastAO.myInstance.ArtObjectName);
                    } else if (rh.transform!=lastAO.transform) {
                        isTrile=false;
                        lastAO=rh.transform.GetComponent<ArtObjectImported>();
                        aoBounds=LevelManager.Instance.getAOBounds(lastAO.myInstance.ArtObjectName);
                    }
                }
                objectDragging.SetNormalAndPosition(rh.normal,rh.point);
            }

            if (isTrile) {
                if (lastTrile==null)
                    return;

                preTrileMove=new TrileEmplacement((int)lastTrile.transform.position.x,(int)lastTrile.transform.position.y,(int)lastTrile.transform.position.z);
                lastTrile.GetComponent<BoxCollider>().enabled=false;

            } else {
                if (lastAO==null)
                    return;

                lastAO.GetComponent<MeshCollider>().enabled=false;
            }

        }

        if (Input.GetMouseButton(1)) {

            distOffset-=Input.mouseScrollDelta.y/10;

            distOffset=Mathf.Clamp(distOffset,-1,1);

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
                    lastTrile.transform.position-=trileBounds.center;
                    lastTrile.transform.position+=transform.forward*distOffset;
                }

            } else {
                if (lastAO==null)
                    return;

                float pos;

                Ray cam = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (objectDragging.Raycast(cam, out pos)) {
                    lastAO.transform.position=cam.GetPoint(pos)-(objectDragging.normal/2);
                    lastAO.transform.position-=aoBounds.center;
                    lastAO.transform.position+=transform.forward*distOffset;
                }
            }
        }

        if (Input.GetMouseButtonUp(1)) {
            if (isTrile) {
                if (lastTrile==null)
                    return;

                lastTrile.transform.position=roundToGrid(1,lastTrile.transform.position);
                TrileEmplacement newPlacment = new TrileEmplacement(Mathf.FloorToInt(lastTrile.transform.position.x), Mathf.RoundToInt(lastTrile.transform.position.y), Mathf.RoundToInt(lastTrile.transform.position.z));

                if (LevelManager.Instance.isOccupied(newPlacment)) {
                    Debug.Log("Trile exists at " + new Vector3(newPlacment.X,newPlacment.Y,newPlacment.Z) + " id is " + LevelManager.Instance.loaded.Triles[newPlacment].TrileId);
                    lastTrile.transform.position=new Vector3(preTrileMove.X, preTrileMove.Y, preTrileMove.Z);
                    lastTrile.GetComponent<BoxCollider>().enabled=true;
                } else {
                    LevelManager.Instance.MoveTrile(preTrileMove, newPlacment, lastTrile.myInstance);
                    lastTrile.GetComponent<BoxCollider>().enabled=true;
                }

            } else {
                if (lastAO==null)
                    return;

                lastAO.transform.position=roundToGrid(16,lastAO.transform.position);
                LevelManager.Instance.MoveAO(lastAO.myInstance.Id,lastAO.transform.position);
                lastAO.GetComponent<MeshCollider>().enabled=true;
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

    Bounds trileBounds, aoBounds;

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
    Material lineMatTrile,lineMatAO;

    void OnPostRender() {
        GL.wireframe=false;
        GL.PushMatrix();
        lineMatTrile.SetPass(0);

        //Draw cube
        {
            GL.Begin(GL.LINES);
            if (lastTrile!=null) {
                for (int i = 0; i<vertsToDraw.Length; i++) {
                    Vector3 curr = vertsToDraw[i];
                    curr-=Vector3.one/2;
                    curr*=1.1f;
                    curr=new Vector3(curr.x*trileBounds.size.x,curr.y*trileBounds.size.y,curr.z*trileBounds.size.z);

                    curr+=trileBounds.center;
                    curr=lastTrile.transform.rotation*curr;
                    curr+=lastTrile.transform.position;

                    GL.Vertex(curr);
                }
            }
            GL.End();
            lineMatAO.SetPass(0);
            GL.Begin(GL.LINES);

            if (lastAO!=null) {
                GL.Color(Color.red);
                for (int i = 0; i<vertsToDraw.Length; i++) {
                    Vector3 curr = vertsToDraw[i];
                    curr-=Vector3.one/2;
                    curr*=1.1f;
                    curr=new Vector3(curr.x*aoBounds.size.x, curr.y*aoBounds.size.y, curr.z*aoBounds.size.z);

                    curr+=aoBounds.center;
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
