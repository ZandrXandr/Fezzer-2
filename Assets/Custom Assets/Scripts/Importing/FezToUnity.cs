using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using FezEngine.Structure.Geometry;
using FezEngine.Structure;

class FezToUnity {

    public static Mesh ArtObjectToMesh(ArtObject ao) {
        Mesh m = new Mesh();

        Vector3[] vertices = new Vector3[ao.Geometry.Vertices.Length];
        int[] triangles = new int[ao.Geometry.Indices.Length];
        Vector2[] uv = new Vector2[vertices.Length];

        for (int i = 0; i < vertices.Length; i++) {
            vertices[i] = ao.Geometry.Vertices[i].Position;
            uv[i] = ao.Geometry.Vertices[i].TextureCoordinate;
        }

        for (int i = 0; i < triangles.Length; i += 3) {
            triangles[i] = ao.Geometry.Indices[i+2];
            triangles[i+1] = ao.Geometry.Indices[i+1];
            triangles[i+2] = ao.Geometry.Indices[i];
        }

        m.vertices = vertices;
        m.triangles = triangles;
        m.uv = uv;

        m.RecalculateNormals();
        m.Optimize();

        return m;
    }

    public static Mesh TrileToMesh(Trile t) {
        Mesh m = new Mesh();

        Vector3[] vertices = new Vector3[t.Geometry.Vertices.Length];
        int[] triangles = new int[t.Geometry.Indices.Length];
        Vector2[] uv = new Vector2[vertices.Length];

        for (int i = 0; i < vertices.Length; i++) {
            vertices[i] = t.Geometry.Vertices[i].Position;
            uv[i] = t.Geometry.Vertices[i].TextureCoordinate;
        }

        for (int i = 0; i < triangles.Length; i += 3) {
            triangles[i] = t.Geometry.Indices[i+2];
            triangles[i+1] = t.Geometry.Indices[i+1];
            triangles[i+2] = t.Geometry.Indices[i];
        }

        m.vertices = vertices;
        m.triangles = triangles;
        m.uv = uv;

        m.RecalculateNormals();
        m.Optimize();

        return m;
    }

    public static Material GeometryToMaterial(Texture2D cubeMap) {
        Material m =  new Material(Shader.Find("Standard"));

        m.mainTexture = cubeMap;
        m.mainTexture.filterMode = FilterMode.Point;

        return m;
    }

}

