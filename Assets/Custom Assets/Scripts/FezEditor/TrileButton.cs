using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TrileButton : MonoBehaviour {


    public void Click() {
        LevelManager.Instance.PickTrile(gameObject);
    }

}
