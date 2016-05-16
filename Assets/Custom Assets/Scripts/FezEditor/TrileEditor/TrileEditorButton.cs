using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TrileEditorButton : MonoBehaviour {

    public void Click() {
        TrileEditor.Instance.PickTrile(gameObject);
    }
}
