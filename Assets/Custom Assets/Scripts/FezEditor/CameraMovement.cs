using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraMovement : MonoBehaviour {

    float yRot;
    float speed = 2;

    Camera c;
    //SESSAO sessao;

	// Use this for initialization
	void Start () {
        c=GetComponent<Camera>();
        //sessao=GetComponent<SESSAO>();
	}

    public bool isOrthographic { get; set; }
    public int rotation;
	
	// Update is called once per frame
	void Update () {

        if (isOrthographic) {
            //sessao.enabled=false;
            speed-=Input.mouseScrollDelta.y*0.75f;
            speed=Mathf.Clamp(speed, 0.1f, 30);

            transform.localPosition=Vector3.back*5*(speed+2);
            transform.localRotation=Quaternion.identity;
            transform.parent.rotation=Quaternion.Lerp(transform.parent.rotation,Quaternion.Euler(0,90*rotation,0),5*Time.deltaTime);

            c.orthographic=true;
            c.orthographicSize=speed;

            Vector3 move = new Vector3();

            move-=transform.parent.right*Input.GetAxis("Mouse X");
            move-=transform.parent.up*Input.GetAxis("Mouse Y");

            rotation+=Input.GetKeyDown(KeyCode.LeftArrow) ? 1: 0;
            rotation-=Input.GetKeyDown(KeyCode.RightArrow) ? 1 : 0;

            if (Input.GetMouseButton(0)) {
                transform.parent.position+=move*(speed/15);
            }

        } else {
            //sessao.enabled=true;
            c.orthographic=false;
            transform.localPosition=Vector3.zero;

            if (!Input.GetKey(KeyCode.LeftShift))
                return;

            yRot-=Input.GetAxis("Mouse Y")*(3+(speed/2));
            yRot=Mathf.Clamp(yRot, -90, 90);

            speed+=Input.mouseScrollDelta.y*2;
            speed=Mathf.Clamp(speed, 0.25f, 30);

            transform.localRotation=Quaternion.Euler(yRot, 0, 0);
            transform.parent.Rotate(0, Input.GetAxis("Mouse X")*(3+(speed/2)), 0);

            Vector3 move = new Vector3();

            move+=transform.right*Input.GetAxis("Horizontal");
            move+=transform.forward*Input.GetAxis("Vertical");

            if (Input.GetKey(KeyCode.E))
                move+=transform.up;
            else if (Input.GetKey(KeyCode.Q))
                move-=transform.up;

            transform.parent.position+=move*Time.deltaTime*speed*3;
        }
	
	}
}
