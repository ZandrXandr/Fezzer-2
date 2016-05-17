using UnityEngine;
using System.Collections;

public class DrawCubeDataGizmos : MonoBehaviour {

    [SerializeField]
    TrixelModel model;

    [SerializeField]
    bool showAir,draw=true;
	


	void OnDrawGizmos() {
        if (!draw)
            return;
        foreach(IntPos p in model.data) {
            Gizmos.DrawWireCube((new Vector3(p.x, p.y, p.z)+Vector3.one/2)/16-Vector3.one/2, Vector3.one/16);
        }
    }
}
