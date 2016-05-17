using UnityEngine;
using System.Collections;
using FezEngine.Structure;

public class ModelEditor : Singleton<ModelEditor> {

    public TrileSet currentSet;
    public Trile currentTrile;
    public ArtObject currentAO;

    public TrixelModel editableModel;

    public ModelEditorMode mode;

    public void SetMode(int setMode) {
        mode=(ModelEditorMode)setMode;

        if (mode==ModelEditorMode.None) {
            editableModel.gameObject.SetActive(false);
        } else if (mode==ModelEditorMode.Trile) {
            editableModel.gameObject.SetActive(true);
            editableModel.mode=EditableModelMode.Trile;
        } else if (mode==ModelEditorMode.ArtObject) {
            editableModel.gameObject.SetActive(true);
            editableModel.mode=EditableModelMode.ArtObject;
        } else if (mode==ModelEditorMode.TrileSet) {
            editableModel.gameObject.SetActive(true);
            editableModel.mode=EditableModelMode.Trileset;
        }
    }

}
public enum ModelEditorMode {
    None,
    Trile,
    ArtObject,
    TrileSet
}
