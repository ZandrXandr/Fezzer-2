using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ModelEditorUI : Singleton<ModelEditorUI> {

    [SerializeField]
    Dropdown mode;

    public void Update() {
        ModelEditor.Instance.SetMode(mode.value);
    }

}
