using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class AOImporter : MonoBehaviour {

    public void LoadAO(Text name) {

        Debug.Log("Load");

        if (name.text.Length==0)
            return;

        LevelManager.Instance.LoadArtObject(name.text);

    }

}
