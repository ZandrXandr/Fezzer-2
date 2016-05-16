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
        for(int x = 0; x < 16; x++) {
            for(int y = 0; y < 16; y++) {
                for(int z = 0; z < 16; z++) {

                    if (showAir!=model.data[x, y, z]){
                        Gizmos.color=Color.green;
                        Gizmos.DrawWireCube((new Vector3(x,y,z)+Vector3.one/2)/16-Vector3.one/2,Vector3.one/16);
                    }

                }
            }
        }
    }
}
