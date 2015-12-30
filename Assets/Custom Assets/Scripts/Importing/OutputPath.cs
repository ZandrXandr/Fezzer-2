using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OutputPath : MonoBehaviour {

    static string setPath,setPathExport;

    public static bool isEditor;

    public static string OutputPathDir {
        get {
            if (isEditor) {
                return "F:/Fezzer 2/out/";
            } else {
                return setPath;
            }
        }
    }
    public static string OutputPathDirExport {
        get {
            if (isEditor) {
                return "F:/Fezzer 2/export/";
            } else {
                return setPathExport;
            }
        }
    }

}
