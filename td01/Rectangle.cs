using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;

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

        int width = 360;
        int height = 120;

        int nb_rect_w = 6;
        int nb_rect_h = 4;

        int w_offset = width / nb_rect_w;
        int h_offset = height / nb_rect_h;

        Vector3[] vertices = new Vector3[(nb_rect_w + 1) * (nb_rect_h + 1)];
        List<int> listTriangles = new List<int>();

        int x, y, z;
        z = 0;
        int i = 0;
        for (x=0; x<=width; x+=w_offset)
        {
            for (y = 0; y <= height; y += h_offset)
            {
                vertices[0] = new Vector3 (x, y, z);
                i++;
            }
        }
        mesh.vertices = vertices;
        print(vertices.Length);

        //todo UV
        //todo pourquoi 30 sa mère ????
        for (x=0; x< 29; x++)
        {
            listTriangles.Add(x);
            listTriangles.Add(x+1);
            listTriangles.Add(x+5);
            listTriangles.Add(x+1);
            listTriangles.Add(x+6);
            listTriangles.Add(x+5);
            print(x);
        }
        mesh.triangles = listTriangles.ToArray();

    }
/*
    // Update is called once per frame
    void Update()
    {
    }*/
}
