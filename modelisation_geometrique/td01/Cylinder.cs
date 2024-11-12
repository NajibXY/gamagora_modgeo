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

public class Cylinder : MonoBehaviour
{
    // Start is called before the first frame update

    void Start()
    {
        // Init Mesh components
        gameObject.GetComponent<MeshRenderer>();
        Mesh mesh = GetComponent<MeshFilter>().mesh;

        mesh.Clear();

        // Cylinder Mesh
        var myMesh = CylinderMesh(1,5,10);

        mesh.vertices = myMesh.Item1;
        mesh.triangles = myMesh.Item2.ToArray();
    }


    // Cylinder Mesh
    (Vector3[], List<int>) CylinderMesh(float r, float h, int m)
    {
        //size of vertices
        Vector3[] vertices = new Vector3[m * 2 + 2];
        List<int> listTriangles = new List<int>();

        float teta;
        float teta_step = 2 * Mathf.PI / m;

        int vidx = 0;
        // Top and bottom edge
        for (int i = 0; i < m; i++) {
            teta = teta_step * i;

            float x = Mathf.Cos(teta) * r;
            float y = Mathf.Sin(teta) * r;

            // Top vertex
            vertices[vidx] = new Vector3(x, h / 2, y);
            vidx++;

            // Bottom vertex
            vertices[vidx] = new Vector3(x, -h / 2, y);
            vidx++;
        }

        // Center vertices (top and bottom cap)
        vertices[vidx] = new Vector3(0, h / 2, 0);
        vidx++;
        vertices[vidx] = new Vector3(0, -h / 2, 0);

        // Facets triangles
        for (int i = 0; i < m; i++) {
            int top1 = i * 2;
            int bottom1 = top1 + 1;
            int top2 = (top1 + 2) % (m * 2);
            int bottom2 = (bottom1 + 2) % (m * 2);

            // First triangle (side)
            listTriangles.Add(top1);
            listTriangles.Add(bottom1);
            listTriangles.Add(bottom2);

            // Second triangle (side)
            listTriangles.Add(top1);
            listTriangles.Add(bottom2);
            listTriangles.Add(top2);
        }

        // Top cap triangles
        int topCenter = m * 2;
        for (int i = 0; i < m; i++) {
            int top1 = i * 2;
            int top2 = (top1 + 2) % (m * 2);

            listTriangles.Add(topCenter);
            listTriangles.Add(top1);
            listTriangles.Add(top2);
        }

        // Bottom cap triangles
        int bottomCenter = m * 2 + 1;
        for (int i = 0; i < m; i++) {
            int bottom1 = i * 2 + 1;
            int bottom2 = (bottom1 + 2) % (m * 2);

            listTriangles.Add(bottomCenter);
            listTriangles.Add(bottom2);
            listTriangles.Add(bottom1);
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
