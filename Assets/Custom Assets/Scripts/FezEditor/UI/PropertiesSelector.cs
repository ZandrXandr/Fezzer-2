using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PropertiesSelector : MonoBehaviour {

    public GameObject levelProp, objectProp, editorProp;
    
    public int SetProperties {
        set {
            setInt(value);
        }
    }

    void Start() {
        setInt(0);
    }

    void setInt(int set) {

        if (set==0) {
            levelProp.SetActive(false);
            objectProp.SetActive(false);
            editorProp.SetActive(false);
        }
        if (set==1) {
            levelProp.SetActive(true);
            objectProp.SetActive(false);
            editorProp.SetActive(false);
        }
        if (set==2) {
            levelProp.SetActive(false);
            objectProp.SetActive(true);
            editorProp.SetActive(false);
        }
        if (set==3) {
            levelProp.SetActive(false);
            objectProp.SetActive(false);
            editorProp.SetActive(true);
        }

    }

}
