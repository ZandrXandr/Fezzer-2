using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class EditorRegion : MonoBehaviour {

    [SerializeField]
    bool isIn = false;

    public void SetIn(bool set) {
        isIn=set;
    }



}
