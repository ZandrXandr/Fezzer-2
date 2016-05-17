using UnityEngine;
using System.Collections;

public class CopyEnabled : MonoBehaviour {

    [SerializeField]
    GameObject[] targets;

    void OnDisable() {
        foreach (GameObject g in targets)
            g.SetActive(false);
    }

    void OnEnable() {
        foreach (GameObject g in targets)
            g.SetActive(true);
    }
}
