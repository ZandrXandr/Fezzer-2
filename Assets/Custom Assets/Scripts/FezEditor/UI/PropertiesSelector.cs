using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PropertiesSelector : MonoBehaviour {

    public List<GameObject> objects;
    
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
            foreach (GameObject g in objects) {
                g.SetActive(false);
            }
        } else {
            int id = set-1;
            if (id>=objects.Count)
                return;

            foreach (GameObject g in objects) {
                g.SetActive(false);
            }

            objects[id].SetActive(true);
        }

    }

}
