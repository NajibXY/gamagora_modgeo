using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.PlayerLoop;
using UnityEngine.SearchService;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;
using static UnityEditor.Progress;

public class Cone : MonoBehaviour
{
    // Start is called before the first frame update

    void Start()
    {
        // Init Mesh components
        gameObject.AddComponent<MeshFilter>();
        gameObject.AddComponent<MeshRenderer>();
        gameObject.GetComponent<MeshRenderer>();
        Mesh mesh = GetComponent<MeshFilter>().mesh;

        mesh.Clear();

        // Cone Mesh
        var myMesh = ConeMesh(1, 4, 30);

        mesh.vertices = myMesh.Item1;
        mesh.triangles = myMesh.Item2.ToArray();
    }

    (Vector3[], List<int>) ConeMesh(float radius, float height, int segments) {
        // Vertices
        Vector3[] vertices = new Vector3[segments + 2];
        List<int> listTriangles = new List<int>();

        // Top of the cone
        vertices[0] = new Vector3(0, height, 0);

        // Base vertices
        float angleStep = 2 * MathF.PI / segments;
        for (int i = 0; i < segments; i++) {
            float theta = i * angleStep;
            float x = MathF.Cos(theta) * radius;
            float z = MathF.Sin(theta) * radius;
            vertices[i + 1] = new Vector3(x, 0, z);
        }

        // Base
        vertices[vertices.Length - 1] = new Vector3(0, 0, 0);

        // Side triangles
        for (int i = 0; i < segments; i++) {
            listTriangles.Add(0);
            listTriangles.Add(i + 1);
            listTriangles.Add((i + 1) % segments + 1);
        }

        // Base triangles
        for (int i = 0; i < segments; i++) {
            listTriangles.Add(vertices.Length - 1);
            listTriangles.Add((i + 1) % segments + 1);
            listTriangles.Add(i + 1);
        }

        return (vertices, listTriangles);
    }


    /*
        // Update is called once per frame
        void Update()
        {
        }
    */
}
