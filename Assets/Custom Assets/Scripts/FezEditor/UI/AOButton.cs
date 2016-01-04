using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AOButton : MonoBehaviour {

    public void Click() {
        LevelManager.Instance.PickAO(gameObject);
    }
}
