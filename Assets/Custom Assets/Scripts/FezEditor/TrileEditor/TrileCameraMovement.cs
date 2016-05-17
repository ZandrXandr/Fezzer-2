using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TrileCameraMovement : MonoBehaviour {
    public float distance = 5.0f;
    public float xSpeed = 120.0f;
    public float ySpeed = 120.0f;

    public float yMinLimit = -20f;
    public float yMaxLimit = 80f;

    public float distanceMin = .5f;
    public float distanceMax = 15f;

    float x = 0.0f;
    float y = 0.0f;

    // Use this for initialization
    void Start() {
        Vector3 angles = transform.eulerAngles;
        x=angles.y;
        y=angles.x;
    }

    Vector3 offset;

    void LateUpdate() {

        if (Input.GetKey(KeyCode.LeftShift)) {
            Vector3 move = new Vector3();
            move-=transform.right*Input.GetAxis("Mouse X")*0.02f;
            move-=transform.up*Input.GetAxis("Mouse Y")*0.02f;
            offset+=move;
        } else if (Input.GetMouseButton(2)) {
            x+=Input.GetAxis("Mouse X")*xSpeed*distance*0.03f;
            y-=Input.GetAxis("Mouse Y")*ySpeed*distance*0.03f;
        }

        y=ClampAngle(y, yMinLimit, yMaxLimit);

        Quaternion rotation = Quaternion.Euler(y, x, 0);

        distance=Mathf.Clamp(distance-Input.GetAxis("Mouse ScrollWheel")*5, distanceMin, distanceMax);

        Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
        Vector3 position = rotation*negDistance;

        transform.rotation=rotation;
        transform.position=position+offset;
    }

    public static float ClampAngle(float angle, float min, float max) {
        if (angle<-360F)
            angle+=360F;
        if (angle>360F)
            angle-=360F;
        return Mathf.Clamp(angle, min, max);
    }

}
