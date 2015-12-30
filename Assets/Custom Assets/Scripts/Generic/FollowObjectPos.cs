using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FollowObjectPos : MonoBehaviour {

    [SerializeField]
    Transform target;
	
	// Update is called once per frame
	void LateUpdate () {
        transform.position=target.position;
	}
}
