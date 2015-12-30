using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class HeightMap {

    public Texture2D texture;

    public Color[,] colors;

    public float minValue = 0, maxValue = 1;

    public void GenerateValues() {
        GenerateValues(texture);
    }

    public void GenerateValues(Texture2D tex) {
        colors=new Color[tex.width,tex.height];

        for(int x = 0; x < tex.width; x++) {
            for (int y = 0; y<tex.height; y++) {
                colors[x, y]=tex.GetPixel(x,y);
            }
        }
    }

}
