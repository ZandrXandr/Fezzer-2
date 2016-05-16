using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class TrixelModelGenerator {

    static List<Vector3> vertices = new List<Vector3>();
    static List<Vector2> uv = new List<Vector2>();
    static List<int> tris = new List<int>();

    static int faceCount = 0;
    static Vector2 uvPos,trixelUVSize;

    public static Mesh GetDataMesh(bool[,,] data, Vector2 _uvPos, Vector2 _trileUVSize) {
        Mesh m = new Mesh();
        uvPos=_uvPos;
        trixelUVSize=_trileUVSize;

        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        sw.Start();

        for(int y = 0; y < 16; y++) {
            GenerateTopFaces(data, y);
            GenerateBottomFaces(data,y);
        }
        for (int z = 0; z<16; z++) {
            GenerateNorthFaces(data, z);
            GenerateSouthFaces(data, z);
        }
        for(int x = 0; x < 16; x++) {
            GenerateEastFaces(data,x);
            GenerateWestFaces(data,x);
        }

        sw.Stop();

        m.vertices=vertices.ToArray();
        m.triangles=tris.ToArray();
        m.uv=uv.ToArray();

        vertices.Clear();
        tris.Clear();
        uv.Clear();
        faceCount=0;

        m.RecalculateNormals();

        return m;
    }

    private static void GenerateWestFaces(bool[,,] data, int x) {
        //The trixels we've checked already.
        bool[,] mask = new bool[16, 16];
        for (int z = 0; z<16; z++) {
            for (int y = 0; y<16; y++) {
                if (mask[z, y]||!data[x, y, z]) {
                    continue;
                }
                int width = 0;
                int height = 0;
                //Calculate width
                for (int w = z; w<16; w++) {
                    if (mask[w, y]||!data[x, y, w])
                        break;
                    if (x>0)
                        if (data[x-1, y, w])
                            break;
                    width++;
                }
                if (width==0) {
                    mask[z, y]=true;
                    continue;
                }
                //Calculate height
                for (int h = y; h<16; h++) {
                    bool continueIterating = true;
                    for (int w = 0; w<width; w++) {
                        if (mask[z+w, h]||!data[x, h, z+w])
                            continueIterating=false;
                        if (x>0)
                            if (data[x-1, h, z+w])
                                continueIterating=false;
                        if (!continueIterating)
                            break;
                    }
                    if (!continueIterating)
                        break;
                    height++;
                }

                if (height==0) {
                    mask[z, y]=true;
                    continue;
                }
                CubeWest(x, y, z, width, height);
                for (int w = 0; w<width; w++) {
                    for (int h = 0; h<height; h++) {
                        mask[z+w, y+h]=true;
                    }
                }
            }
        }
    }

    private static void GenerateEastFaces(bool[,,] data, int x) {
        //The trixels we've checked already.
        bool[,] mask = new bool[16, 16];
        for (int z = 0; z<16; z++) {
            for (int y = 0; y<16; y++) {
                if (mask[z, y]||!data[x, y, z]) {
                    continue;
                }
                int width = 0;
                int height = 0;
                //Calculate width
                for (int w = z; w<16; w++) {
                    if (mask[w, y]||!data[x, y, w])
                        break;
                    if (x<15)
                        if (data[x+1, y, w])
                            break;
                    width++;
                }
                if (width==0) {
                    mask[z, y]=true;
                    continue;
                }
                //Calculate height
                for (int h = y; h<16; h++) {
                    bool continueIterating = true;
                    for (int w = 0; w<width; w++) {
                        if (mask[z+w, h]||!data[x, h, z+w])
                            continueIterating=false;
                        if (x<15)
                            if (data[x+1, h, z+w])
                                continueIterating=false;
                        if (!continueIterating)
                            break;
                    }
                    if (!continueIterating)
                        break;
                    height++;
                }

                if (height==0) {
                    mask[z, y]=true;
                    continue;
                }
                CubeEast(x, y, z, width, height);
                for (int w = 0; w<width; w++) {
                    for (int h = 0; h<height; h++) {
                        mask[z+w, y+h]=true;
                    }
                }
            }
        }
    }

    private static void GenerateSouthFaces(bool[,,] data, int z) {
        //The trixels we've checked already.
        bool[,] mask = new bool[16, 16];
        for (int x = 0; x<16; x++) {
            for (int y = 0; y<16; y++) {
                if (mask[x, y]||!data[x, y, z]) {
                    continue;
                }
                int width = 0;
                int height = 0;
                //Calculate width
                for (int w = x; w<16; w++) {
                    if (mask[w, y]||!data[w, y, z])
                        break;
                    if (z>0)
                        if (data[w, y, z-1])
                            break;
                    width++;
                }
                if (width==0) {
                    mask[x, y]=true;
                    continue;
                }
                //Calculate height
                for (int h = y; h<16; h++) {
                    bool continueIterating = true;
                    for (int w = 0; w<width; w++) {
                        if (mask[x+w, h]||!data[x+w, h, z])
                            continueIterating=false;
                        if (z>0)
                            if (data[x+w, h, z-1])
                                continueIterating=false;
                        if (!continueIterating)
                            break;
                    }
                    if (!continueIterating)
                        break;
                    height++;
                }

                if (height==0) {
                    mask[x, y]=true;
                    continue;
                }
                CubeSouth(x, y, z, width, height);
                for (int w = 0; w<width; w++) {
                    for (int h = 0; h<height; h++) {
                        mask[x+w, y+h]=true;
                    }
                }
            }
        }
    }

    private static void GenerateNorthFaces(bool[,,] data, int z) {
        //The trixels we've checked already.
        bool[,] mask = new bool[16, 16];
        for (int x = 0; x<16; x++) {
            for (int y = 0; y<16; y++) {
                if (mask[x, y]||!data[x, y, z]) {
                    continue;
                }
                int width = 0;
                int height = 0;
                //Calculate width
                for (int w = x; w<16; w++) {
                    if (mask[w, y]||!data[w, y, z])
                        break;
                    if (z<15)
                        if (data[w, y, z+1])
                            break;
                    width++;
                }
                if (width==0) {
                    mask[x, y]=true;
                    continue;
                }
                //Calculate height
                for (int h = y; h<16; h++) {
                    bool continueIterating = true;
                    for (int w = 0; w<width; w++) {
                        if (mask[x+w, h]||!data[x+w, h, z])
                            continueIterating=false;
                        if (z<15)
                            if (data[x+w, h, z+1])
                                continueIterating=false;
                        if (!continueIterating)
                            break;
                    }
                    if (!continueIterating)
                        break;
                    height++;
                }

                if (height==0) {
                    mask[x, y]=true;
                    continue;
                }
                CubeNorth(x, y, z, width, height);
                for (int w = 0; w<width; w++) {
                    for (int h = 0; h<height; h++) {
                        mask[x+w, y+h]=true;
                    }
                }
            }
        }
    }

    private static void GenerateBottomFaces(bool[,,] data, int y) {
        //The trixels we've checked already.
        bool[,] mask = new bool[16, 16];
        for (int x = 0; x<16; x++) {
            for (int z = 0; z<16; z++) {
                if (mask[x, z]||!data[x, y, z]) {
                    continue;
                }
                int width = 0;
                int height = 0;
                //Calculate width
                for (int w = x; w<16; w++) {
                    if (mask[w, z]||!data[w, y, z])
                        break;
                    if (y>0)
                        if (data[w, y-1, z])
                            break;
                    width++;
                }
                if (width==0) {
                    mask[x, z]=true;
                    continue;
                }
                //Calculate height
                for (int h = z; h<16; h++) {
                    bool continueIterating = true;
                    for (int w = 0; w<width; w++) {
                        if (mask[x+w, h]||!data[x+w, y, h])
                            continueIterating=false;
                        if (y>0)
                            if (data[x+w, y-1, h])
                                continueIterating=false;
                        if (!continueIterating)
                            break;
                    }
                    if (!continueIterating)
                        break;
                    height++;
                }
                if (height==0) {
                    mask[x, z]=true;
                    continue;
                }
                CubeBottom(x, y, z, width, height);
                for (int w = 0; w<width; w++) {
                    for (int h = 0; h<height; h++) {
                        mask[x+w, z+h]=true;
                    }
                }
            }
        }
    }

    private static void GenerateTopFaces(bool[,,] data, int y) {
        //The trixels we've checked already.
        bool[,] mask = new bool[16, 16];
        for (int x = 0; x<16; x++) {
            for (int z = 0; z<16; z++) {
                if (mask[x, z]||!data[x, y, z]) {
                    continue;
                }
                int width = 0;
                int height = 0;
                //Calculate width
                for (int w = x; w<16; w++) {
                    if (mask[w, z]||!data[w, y, z])
                        break;
                    if (y<15)
                        if (data[w, y+1, z])
                            break;
                    width++;
                }
                if (width==0) {
                    mask[x, z]=true;
                    continue;
                }
                //Calculate height
                for (int h = z; h<16; h++) {
                    bool continueIterating = true;
                    for (int w = 0; w<width; w++) {
                        if (mask[x+w, h]||!data[x+w, y, h])
                            continueIterating=false;
                        if (y<15)
                            if (data[x+w, y+1, h])
                                continueIterating=false;
                        if (!continueIterating)
                            break;
                    }
                    if (!continueIterating)
                        break;
                    height++;
                }
                if (height==0) {
                    mask[x, z]=true;
                    continue;
                }
                CubeTop(x, y, z, width, height);
                for (int w = 0; w<width; w++) {
                    for (int h = 0; h<height; h++) {
                        mask[x+w, z+h]=true;
                    }
                }
            }
        }
    }

    public static Vector2 offset=new Vector2();

    static void CubeTop(int x, int y, int z, int width, int height) {

        vertices.Add(new Vector3(x, y+1, z+height)/16-Vector3.one/2);
        vertices.Add(new Vector3(x+width, y+1, z+height)/16-Vector3.one/2);
        vertices.Add(new Vector3(x+width, y+1, z)/16-Vector3.one/2);
        vertices.Add(new Vector3(x, y+1, z)/16-Vector3.one/2);

        int faceIndex = 4;

        Vector2 offsetUV = new Vector2(((faceIndex*16)+faceIndex*2)*trixelUVSize.x, 0);

        float thisFaceX = ((x+width)*trixelUVSize.x);
        float thisFaceXFar = ((x)*trixelUVSize.x);

        float thisFaceY = ((z+height)*trixelUVSize.y);
        float thisFaceYFar = ((z)*trixelUVSize.y);

        offsetUV+=new Vector2(Mathf.FloorToInt(offset.x+1)*trixelUVSize.x, Mathf.FloorToInt(offset.y+1)*trixelUVSize.y);

        uv.Add((Vector2)uvPos+new Vector2(thisFaceXFar, thisFaceY)+offsetUV);
        uv.Add((Vector2)uvPos+new Vector2(thisFaceX, thisFaceY)+offsetUV);
        uv.Add((Vector2)uvPos+new Vector2(thisFaceX, thisFaceYFar)+offsetUV);
        uv.Add((Vector2)uvPos+new Vector2(thisFaceXFar, thisFaceYFar)+offsetUV);

        Cube();
    }

    static void CubeBottom(int x, int y, int z, int width, int height) {

        vertices.Add(new Vector3(x, y, z)/16-Vector3.one/2);
        vertices.Add(new Vector3(x+width, y, z)/16-Vector3.one/2);
        vertices.Add(new Vector3(x+width, y, z+height)/16-Vector3.one/2);
        vertices.Add(new Vector3(x, y, z+height)/16-Vector3.one/2);

        int faceIndex = 5;

        Vector2 offsetUV = new Vector2(((faceIndex*16)+faceIndex*2)*trixelUVSize.x, 0);

        float thisFaceX = ((x+width)*trixelUVSize.x);
        float thisFaceXFar = ((x)*trixelUVSize.x);

        float thisFaceY = ((16-z)*trixelUVSize.y);
        float thisFaceYFar = ((16-z-height)*trixelUVSize.y);

        offsetUV+=new Vector2(Mathf.FloorToInt(offset.x+1)*trixelUVSize.x, Mathf.FloorToInt(offset.y+1)*trixelUVSize.y);

        uv.Add((Vector2)uvPos+new Vector2(thisFaceXFar, thisFaceY)+offsetUV);
        uv.Add((Vector2)uvPos+new Vector2(thisFaceX, thisFaceY)+offsetUV);
        uv.Add((Vector2)uvPos+new Vector2(thisFaceX, thisFaceYFar)+offsetUV);
        uv.Add((Vector2)uvPos+new Vector2(thisFaceXFar, thisFaceYFar)+offsetUV);

        Cube();
    }

    static void CubeNorth(int x, int y, int z, int width, int height) {

        vertices.Add(new Vector3(x+width, y, z+1)/16-Vector3.one/2);
        vertices.Add(new Vector3(x+width, y+height, z+1)/16-Vector3.one/2);
        vertices.Add(new Vector3(x, y+height, z+1)/16-Vector3.one/2);
        vertices.Add(new Vector3(x, y, z+1)/16-Vector3.one/2);

        int faceIndex = 0;

        Vector2 offsetUV = new Vector2(((faceIndex*16)+faceIndex*2)*trixelUVSize.x, 0);

        float thisFaceX = ((x+width)*trixelUVSize.x);
        float thisFaceXFar = ((x)*trixelUVSize.x);

        float thisFaceY = ((16-y)*trixelUVSize.y);
        float thisFaceYFar = ((16-y-height)*trixelUVSize.y);

        offsetUV+=new Vector2(Mathf.FloorToInt(offset.x+1)*trixelUVSize.x, Mathf.FloorToInt(offset.y+1)*trixelUVSize.y);

        uv.Add((Vector2)uvPos+new Vector2(thisFaceX, thisFaceY)+offsetUV);
        uv.Add((Vector2)uvPos+new Vector2(thisFaceX, thisFaceYFar)+offsetUV);
        uv.Add((Vector2)uvPos+new Vector2(thisFaceXFar, thisFaceYFar)+offsetUV);
        uv.Add((Vector2)uvPos+new Vector2(thisFaceXFar, thisFaceY)+offsetUV);

        Cube();
    }

    static void CubeSouth(int x, int y, int z, int width, int height) {

        vertices.Add(new Vector3(x, y, z)/16-Vector3.one/2);
        vertices.Add(new Vector3(x, y+height, z)/16-Vector3.one/2);
        vertices.Add(new Vector3(x+width, y+height, z)/16-Vector3.one/2);
        vertices.Add(new Vector3(x+width, y, z)/16-Vector3.one/2);

        int faceIndex = 2;

        Vector2 offsetUV = new Vector2(((faceIndex*16)+faceIndex*2)*trixelUVSize.x,0);

        float thisFaceX = ((16-x)*trixelUVSize.x);
        float thisFaceXFar = ((16-x-width)*trixelUVSize.x);

        float thisFaceY = ((16-y)*trixelUVSize.y);
        float thisFaceYFar = ((16-y-height)*trixelUVSize.y);

        offsetUV += new Vector2(Mathf.FloorToInt(offset.x+1)*trixelUVSize.x,Mathf.FloorToInt(offset.y+1)*trixelUVSize.y);

        uv.Add((Vector2)uvPos+new Vector2(thisFaceX, thisFaceY)+offsetUV);
        uv.Add((Vector2)uvPos+new Vector2(thisFaceX, thisFaceYFar)+offsetUV);
        uv.Add((Vector2)uvPos+new Vector2(thisFaceXFar, thisFaceYFar)+offsetUV);
        uv.Add((Vector2)uvPos+new Vector2(thisFaceXFar, thisFaceY)+offsetUV);

        Cube();
    }

    static void CubeWest(int x, int y, int z, int width, int height) {

        vertices.Add(new Vector3(x, y, z+width)/16-Vector3.one/2);
        vertices.Add(new Vector3(x, y+height, z+width)/16-Vector3.one/2);
        vertices.Add(new Vector3(x, y+height, z)/16-Vector3.one/2);
        vertices.Add(new Vector3(x, y, z)/16-Vector3.one/2);

        int faceIndex = 3;

        Vector2 offsetUV = new Vector2(((faceIndex*16)+faceIndex*2)*trixelUVSize.x, 0);

        float thisFaceX = ((z+width)*trixelUVSize.x);
        float thisFaceXFar = ((z)*trixelUVSize.x);

        float thisFaceY = ((16-y)*trixelUVSize.y);
        float thisFaceYFar = ((16-y-height)*trixelUVSize.y);

        offsetUV+=new Vector2(Mathf.FloorToInt(offset.x+1)*trixelUVSize.x, Mathf.FloorToInt(offset.y+1)*trixelUVSize.y);

        uv.Add((Vector2)uvPos+new Vector2(thisFaceX, thisFaceY)+offsetUV);
        uv.Add((Vector2)uvPos+new Vector2(thisFaceX, thisFaceYFar)+offsetUV);
        uv.Add((Vector2)uvPos+new Vector2(thisFaceXFar, thisFaceYFar)+offsetUV);
        uv.Add((Vector2)uvPos+new Vector2(thisFaceXFar, thisFaceY)+offsetUV);

        Cube();
    }

    static void CubeEast(int x, int y, int z, int width, int height) {

        vertices.Add(new Vector3(x+1, y, z)/16-Vector3.one/2);
        vertices.Add(new Vector3(x+1, y+height, z)/16-Vector3.one/2);
        vertices.Add(new Vector3(x+1, y+height, z+width)/16-Vector3.one/2);
        vertices.Add(new Vector3(x+1, y, z+width)/16-Vector3.one/2);

        int faceIndex = 1;

        Vector2 offsetUV = new Vector2(((faceIndex*16)+faceIndex*2)*trixelUVSize.x, 0);

        float thisFaceX = ((16-z)*trixelUVSize.x);
        float thisFaceXFar = ((16-z-width)*trixelUVSize.x);

        float thisFaceY = ((16-y)*trixelUVSize.y);
        float thisFaceYFar = ((16-y-height)*trixelUVSize.y);

        offsetUV+=new Vector2(Mathf.FloorToInt(offset.x+1)*trixelUVSize.x, Mathf.FloorToInt(offset.y+1)*trixelUVSize.y);

        uv.Add((Vector2)uvPos+new Vector2(thisFaceX, thisFaceY)+offsetUV);
        uv.Add((Vector2)uvPos+new Vector2(thisFaceX, thisFaceYFar)+offsetUV);
        uv.Add((Vector2)uvPos+new Vector2(thisFaceXFar, thisFaceYFar)+offsetUV);
        uv.Add((Vector2)uvPos+new Vector2(thisFaceXFar, thisFaceY)+offsetUV);

        Cube();
    }

    static void Cube() {

        tris.Add(faceCount*4); //1
        tris.Add(faceCount*4+1); //2
        tris.Add(faceCount*4+2); //3
        tris.Add(faceCount*4); //1
        tris.Add(faceCount*4+2); //3
        tris.Add(faceCount*4+3); //4

        faceCount++; // Add this line
    }

}
