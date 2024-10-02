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

public class Rectangle : MonoBehaviour {
    // Start is called before the first frame update

    void Start() {
        // Init Mesh components
        gameObject.GetComponent<MeshRenderer>();
        Mesh mesh = GetComponent<MeshFilter>().mesh;

        mesh.Clear();


        var rectangleMeshData = RectanglesMesh(2, 1, 20, 10);

        mesh.vertices = rectangleMeshData.Item1;
        mesh.triangles = rectangleMeshData.Item2.ToArray();
    }

    // Mesh with multiple triangles
    (Vector3[], List<int>) RectanglesMesh(int width, int height, int nb_rect_w, int nb_rect_h) {
        float w_offset = (float)width / nb_rect_w;
        float h_offset = (float)height / nb_rect_h;

        Vector3[] vertices = new Vector3[(nb_rect_w + 1) * (nb_rect_h + 1)];
        List<int> listTriangles = new List<int>();

        // Vertices
        int i = 0;
        for (int h = 0; h <= nb_rect_h; h++) {
            for (int w = 0; w <= nb_rect_w; w++) {
                float x = w * w_offset;
                float y = h * h_offset;
                float z = 0; // Flat grid (z=0)
                vertices[i] = new Vector3(x, y, z);
                i++;
            }
        }

        // Triangles
        for (int h = 0; h < nb_rect_h; h++) {
            for (int w = 0; w < nb_rect_w; w++) {
                // Current index
                int topLeft = h * (nb_rect_w + 1) + w;
                int topRight = topLeft + 1;
                int bottomLeft = topLeft + (nb_rect_w + 1);
                int bottomRight = bottomLeft + 1;

                // First triangle
                listTriangles.Add(topLeft);
                listTriangles.Add(bottomLeft);
                listTriangles.Add(bottomRight);

                // Second triangle
                listTriangles.Add(topLeft);
                listTriangles.Add(bottomRight);
                listTriangles.Add(topRight);
            }
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
