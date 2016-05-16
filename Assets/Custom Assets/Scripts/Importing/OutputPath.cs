using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OutputPath : MonoBehaviour {

    [SerializeField]
    string editorString;
    [SerializeField]
    bool useEditor;

    public static string setPath;

    public static string OutputPathDir {
        get {
                return setPath;
        }
    }


    void Start() {
        if (useEditor) {
            setPath=editorString;
        }
    }
}
