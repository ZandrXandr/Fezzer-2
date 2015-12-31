using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using FezEngine.Structure.Geometry;
using FezEngine.Structure;

class FezToUnity {

    public static Mesh ArtObjectToMesh(ArtObject ao) {

        Mesh m = new Mesh();

        List<Vector3> verts = new List<Vector3>();
        List<int> tris = new List<int>();
        List<Vector2> uv = new List<Vector2>();

        for(int i = 0; i < ao.Geometry.Vertices.Length; i++) {
            verts.Add(ao.Geometry.Vertices[i].Position);

            uv.Add(ao.Geometry.Vertices[i].TextureCoordinate);
        }

        for (int i = 0; i<ao.Geometry.Indices.Length; i+=3) {
            tris.Add(ao.Geometry.Indices[i+2]);
            tris.Add(ao.Geometry.Indices[i+1]);
            tris.Add(ao.Geometry.Indices[i]);
        }

        m.vertices=verts.ToArray();
        m.triangles=tris.ToArray();
        m.uv=uv.ToArray();

        m.RecalculateNormals();
        m.Optimize();

        return m;
    }

    public static Mesh TrileToMesh(Trile t) {

        Mesh m = new Mesh();

        List<Vector3> verts = new List<Vector3>();
        List<int> tris = new List<int>();
        List<Vector2> uv = new List<Vector2>();

        for (int i = 0; i<t.Geometry.Vertices.Length; i++) {
            verts.Add(t.Geometry.Vertices[i].Position);

            uv.Add(t.Geometry.Vertices[i].TextureCoordinate);
        }

        for (int i = 0; i<t.Geometry.Indices.Length; i+=3) {
            tris.Add(t.Geometry.Indices[i+2]);
            tris.Add(t.Geometry.Indices[i+1]);
            tris.Add(t.Geometry.Indices[i]);
        }

        m.vertices=verts.ToArray();
        m.triangles=tris.ToArray();
        m.uv=uv.ToArray();

        m.RecalculateNormals();
        m.Optimize();

        return m;
    }

    public static Material GeometryToMaterial(Texture2D cubeMap) {
        Material m =  new Material(Shader.Find("Diffuse"));

        m.mainTexture=cubeMap;
        m.mainTexture.filterMode=FilterMode.Point;

        return m;
    }

}

