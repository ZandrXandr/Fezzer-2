using UnityEngine;
using System.Collections;

public class FontFilterSetter : MonoBehaviour {

    [SerializeField]
    Font[] fontsToSet;

    [SerializeField]
    FilterMode filterSet;

	// Use this for initialization
	void Start () {
        foreach (Font f in fontsToSet)
            f.material.mainTexture.filterMode=filterSet;
	
	}
}
