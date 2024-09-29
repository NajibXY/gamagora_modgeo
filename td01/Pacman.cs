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

public class Pacman : MonoBehaviour
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

        // Sphere Mesh
        int meridians = 20;
        int parallels = 10;

        (Vector3[] vertices, List<int> triangles) myMesh = SphereMesh(meridians, parallels);

        mesh.vertices = myMesh.Item1;
        mesh.triangles = myMesh.Item2.ToArray();
    }

    (Vector3[], List<int>) SphereMesh(int meridians, int parallels) {
        // Vertices: (meridians * (parallels - 1)) + 2 (for poles)
        Vector3[] vertices = new Vector3[meridians * (parallels - 1) + 2];
        List<int> listTriangles = new List<int>();

        // Top pole
        vertices[0] = new Vector3(0, 0, 1); // Position the top pole along the z-axis

        int count = 1;
        // Generate vertices for each parallel
        for (int i = 0; i < parallels - 1; i++) {
            float phi = MathF.PI * (i + 1) / parallels;
            for (int j = 0; j < meridians; j++) {
                float theta = 2 * MathF.PI * j / meridians;
                float x = MathF.Sin(phi) * MathF.Cos(theta);
                float z = MathF.Cos(phi); // Position the z-coordinate based on cosine
                float y = MathF.Sin(phi) * MathF.Sin(theta); // Y-axis remains unchanged
                vertices[count] = new Vector3(x, y, z);
                count++;
            }
        }

        // Bottom pole
        vertices[vertices.Length - 1] = new Vector3(0, 0, -1); // Position the bottom pole along the z-axis

        // Top pole triangles
        for (int i = 0; i < meridians; i++) {
            listTriangles.Add(0); // Top pole vertex
            listTriangles.Add(i + 1); // Current vertex
            listTriangles.Add((i + 1) % meridians + 1); // Next vertex
        }

        // Middle triangles between excluding poles
        for (int i = 0; i < parallels - 2; i++) {
            for (int j = 0; j < meridians; j++) {
                int current = j + i * meridians + 1;
                int next = (j + 1) % meridians + i * meridians + 1;
                int below = j + (i + 1) * meridians + 1;
                int belowNext = (j + 1) % meridians + (i + 1) * meridians + 1;

                // First triangle
                listTriangles.Add(current);
                listTriangles.Add(next);
                listTriangles.Add(below);

                // Second triangle
                listTriangles.Add(next);
                listTriangles.Add(belowNext);
                listTriangles.Add(below);
            }
        }

        // Bottom pole triangles
        for (int i = 0; i < meridians; i++) {
            listTriangles.Add(vertices.Length - 1); // Bottom pole vertex
            listTriangles.Add((i + 1) % meridians + (parallels - 2) * meridians + 1);
            listTriangles.Add(i + (parallels - 2) * meridians + 1);
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
