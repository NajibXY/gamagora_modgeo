using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;
using static UnityEditor.Progress;

public class Rectangle : MonoBehaviour
{
    // Start is called before the first frame update

    void Start()
    {
        gameObject.AddComponent<MeshFilter>();
        gameObject.AddComponent<MeshRenderer>();
        Mesh mesh = GetComponent<MeshFilter>().mesh;

        mesh.Clear();

        // mesh
        /*mesh.vertices = new Vector3[] { new Vector3(0, 0, 0), new Vector3(0, 1, 0), new Vector3(1, 1, 0), new Vector3(1, 0, 0) };
        mesh.uv = new Vector2[] { new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 1), new Vector2(1, 0) };
        mesh.triangles = new int[] { 0, 1, 2, 0, 2, 3 };*/


        // Mesh with multiple triangles working !

        int width = 800;
        int height = 2000;

        int nb_rect_w = 8;
        int nb_rect_h = 10;

        int w_offset = width / nb_rect_w;
        int h_offset = height / nb_rect_h;

        Vector3[] vertices = new Vector3[(nb_rect_w + 1) * (nb_rect_h + 1)];
        List<int> listTriangles = new List<int>();

        int x, y, z;
        z = 0;
        int i = 0;
        for (x = 0; x <= width; x += w_offset)
        {
            for (y = 0; y <= height; y += h_offset)
            {
                print(x+";"+y+";"+z+";");
                vertices[i] = new Vector3(x, y, z);
                i++;
            }
        }
        mesh.vertices = vertices;

        for (x = 0; x < nb_rect_w * (nb_rect_h + 1) - 1 ; x++)
        {
            listTriangles.Add(x);
            listTriangles.Add(x + 1);
            listTriangles.Add(x + nb_rect_h + 1);
            listTriangles.Add(x + 1);
            listTriangles.Add(x + nb_rect_h + 2);
            listTriangles.Add(x + nb_rect_h + 1);
            foreach (object o in listTriangles)
            {
                print(o);
            }
        }
        mesh.triangles = listTriangles.ToArray();
        //todo UV ?



    }
    /*
        // Update is called once per frame
        void Update()
        {
        }*/
}
