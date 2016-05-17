using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TrileButton : MonoBehaviour {

    public bool isLevelEditor = true;

    public void Click() {
        if (isLevelEditor)
            LevelManager.Instance.PickTrile(gameObject);
        else
            TrilesetPropertiesUI.Instance.PickTrile(gameObject);
    }

}
