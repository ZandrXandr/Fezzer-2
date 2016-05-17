using UnityEngine;
using FezEngine.Structure;
using System.Collections;
using System.Linq;

public class TrixelModelImporter : MonoBehaviour {

    [SerializeField]
    GameObject importerObject;
    static MeshCollider importerCollider;

    private enum TrixelState {
        Unchecked,
        isCollider,
        isExposedToEdge,
        isNotExposedToEdge
    }

	// Use this for initialization
	void Start () {
        importerObject=new GameObject();
        importerObject.name="Importer Collider";
        importerObject.transform.SetParent(transform);
        importerObject.transform.localPosition=Vector3.zero;
        importerCollider=importerObject.AddComponent<MeshCollider>();
	}


    public bool[,,] GetModelDataFromFile(string path) {

        

        return null;
    }

    public static void SetMeshCollider(Trile trile) {
        Mesh trileMesh = FezToUnity.TrileToMesh(trile);

        importerCollider.gameObject.SetActive(true);
        importerCollider.sharedMesh=null;
        importerCollider.sharedMesh=trileMesh;
    }

    public static bool[,,] GetModelDataFromTrile(Trile trile) {

        int size = 16;

        TrixelState[,,] states = new TrixelState[size, size, size];
        bool[,,] returnArray = new bool[size, size, size];

        Vector3[] dirs = new Vector3[] {Vector3.up,Vector3.down,Vector3.right,Vector3.left,Vector3.forward,Vector3.back};

        //Pass 1, put the mesh collider into the array's data.
        for(int x = -1; x < size+2; x++) {
            for (int y = -1; y<size+2; y++) {
                for (int z = -1; z<size+2; z++) {
                    try {
                        states[x, y, z]=TrixelState.Unchecked;
                    }
                    catch {

                    }
                    foreach (Vector3 d in dirs) {
                        Vector3 pos = (((new Vector3(x,y,z)-Vector3.one/2)/size)-Vector3.one/2) + importerCollider.transform.position;

                        RaycastHit rh;

                        if (Physics.Raycast(new Ray(pos,d),out rh, 1f/size)) {
                            rh.point-=importerCollider.transform.position;
                            IntPos hitPos = IntPos.Vector3ToIntPos((rh.point*16)-rh.normal/2)+(size/2);
                            try {
                                states[hitPos.x, hitPos.y, hitPos.z]=TrixelState.isCollider;
                            } catch {

                            }
                        }
                    }
                }
            }
        }

        //Pass 2, check if each trixel exposed to in any of the 6 cardinal directions.
        for (int x = 0; x<size; x++) {
            for (int y = 0; y<size; y++) {
                for (int z = 0; z<size; z++) {
                    int blockCount = 0;

                    foreach(Vector3 d in dirs) {
                        Vector3 currPos = new Vector3(x,y,z);
                        IntPos currPosInt = new IntPos(x,y,z);

                        bool isBlocked = false;

                        while (currPosInt.isContained(0, size)) {
                            TrixelState getState = states[currPosInt.x, currPosInt.y, currPosInt.z];
                            if (getState==TrixelState.isCollider || getState==TrixelState.isNotExposedToEdge) {
                                isBlocked=true;
                                break;
                            }
                            currPos+=d;
                            currPosInt=IntPos.Vector3ToIntPos(currPos);
                        }
                        if (isBlocked)
                            blockCount++;
                    }

                    if (blockCount==6) {
                        states[x, y, z]=TrixelState.isNotExposedToEdge;
                    } else {
                        states[x, y, z]=TrixelState.isExposedToEdge;
                    }
                }
            }
        }



        //Pass 3, check if each trixel is exposed to a trixel that's exposed to air.
        //This is actually done twice, just in case.
        for (int i = 0; i<2; i++) {
            TrixelState[,,] tempStates=new TrixelState[16,16,16];

            for (int x = 0; x<size; x++) {
                for (int y = 0; y<size; y++) {
                    for (int z = 0; z<size; z++) {
                        tempStates[x, y, z]=states[x, y, z];
                    }
                }
            }

            for (int x = 0; x<size; x++) {
                for (int y = 0; y<size; y++) {
                    for (int z = 0; z<size; z++) {

                        if (states[x, y, z]!=TrixelState.isNotExposedToEdge)
                            continue;

                        bool isBlocked = true;

                        foreach (Vector3 d in dirs) {
                            Vector3 currPos = new Vector3(x, y, z);
                            IntPos currPosInt = new IntPos(x, y, z);

                            while (currPosInt.isContained(0, size)) {
                                TrixelState getState = states[currPosInt.x, currPosInt.y, currPosInt.z];
                                if (getState==TrixelState.isExposedToEdge) {
                                    isBlocked=false;
                                    break;
                                } else if(getState==TrixelState.isCollider || getState==TrixelState.isNotExposedToEdge){
                                    break;
                                }
                                currPos+=d;
                                currPosInt=IntPos.Vector3ToIntPos(currPos);
                            }
                            if (!isBlocked) {
                                tempStates[x, y, z]=TrixelState.isExposedToEdge;
                                break;
                            }
                        }
                    }
                }
            }
            states=tempStates;
        }

        for (int x = 0; x<size; x++) {
            for (int y = 0; y<size; y++) {
                for (int z = 0; z<size; z++) {
                    if (states[x, y, z]==TrixelState.isCollider||states[x, y, z]==TrixelState.isNotExposedToEdge) {
                        returnArray[x, y, z]=true;
                    } else if (states[x, y, z]==TrixelState.isExposedToEdge) {
                        returnArray[x, y, z]=false;
                    }
                }
            }
        }
        importerCollider.gameObject.SetActive(false);
        return returnArray;
    }
}
