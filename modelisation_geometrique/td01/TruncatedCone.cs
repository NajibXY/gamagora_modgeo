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

public class TruncatedCone : MonoBehaviour
{
    // Start is called before the first frame update

    void Start()
    {
        // Init Mesh components
        gameObject.GetComponent<MeshRenderer>();
        Mesh mesh = GetComponent<MeshFilter>().mesh;

        mesh.Clear();

        // Cone Mesh
        var myMesh = TruncatedConeMesh(1, 0.2f, 3, 20);

        mesh.vertices = myMesh.Item1;
        mesh.triangles = myMesh.Item2.ToArray();
    }

    (Vector3[], List<int>) TruncatedConeMesh(float bottomRadius, float topRadius, float height, int segments) {
        Vector3[] vertices = new Vector3[(segments + 1) * 2 + 2];
        List<int> listTriangles = new List<int>();

        // Bottom base vertices
        for (int i = 0; i < segments; i++) {
            float theta = i * (2 * MathF.PI / segments);
            float x = MathF.Cos(theta) * bottomRadius;
            float z = MathF.Sin(theta) * bottomRadius;
            vertices[i] = new Vector3(x, 0, z);
        }

        // Top base vertices
        for (int i = 0; i < segments; i++) {
            float theta = i * (2 * MathF.PI / segments);
            float x = MathF.Cos(theta) * topRadius;
            float z = MathF.Sin(theta) * topRadius;
            vertices[i + segments] = new Vector3(x, height, z);
        }

        // Center of bottom base
        vertices[vertices.Length - 2] = new Vector3(0, 0, 0);
        // Center of top base
        vertices[vertices.Length - 1] = new Vector3(0, height, 0);

        // Side triangles 
        for (int i = 0; i < segments; i++) {
            // Side triangles
            // Bottom, next bottom, top
            listTriangles.Add(i);
            listTriangles.Add((i + 1) % segments);
            listTriangles.Add(i + segments);
            // Next bottom, next top, current top
            listTriangles.Add((i + 1) % segments);
            listTriangles.Add((i + 1) % segments + segments);
            listTriangles.Add(i + segments);
        }

        // Base triangles
        for (int i = 0; i < segments; i++) {
            // Bottom base
            listTriangles.Add(vertices.Length - 2);
            listTriangles.Add(i);
            listTriangles.Add((i + 1) % segments);

            // Top base
            listTriangles.Add(vertices.Length - 1);
            listTriangles.Add(i + segments);
            listTriangles.Add((i + 1) % segments + segments);
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
