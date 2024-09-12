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

public class Rectangle : MonoBehaviour
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

        // Simple rectangle mesh
        /*mesh.vertices = new Vector3[] { new Vector3(0, 0, 0), new Vector3(0, 1, 0), new Vector3(1, 1, 0), new Vector3(1, 0, 0) };
        mesh.uv = new Vector2[] { new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 1), new Vector2(1, 0) };
        mesh.triangles = new int[] { 0, 1, 2, 0, 2, 3 };*/


        // Mesh with multiple triangles working

        /*int width = 800;
        int height = 2000;

        int nb_rect_w = 8;
        int nb_rect_h = 10;

        var rectangleMeshData = RectanglesMesh(width, height, nb_rect_w, nb_rect_h);

        mesh.vertices = rectangleMeshData.Item1;
        mesh.triangles = rectangleMeshData.Item2.ToArray();*/

        //todo UV ?

        // Cylinder Mesh 

        /*        var cylinderMeshData = CylinderMesh(200, 800, 18);

                mesh.vertices = cylinderMeshData.Item1;
                mesh.triangles = cylinderMeshData.Item2.ToArray();

                foreach (object o in mesh.triangles)
                {
                    print(o);
                }
        */
        // Sphere Mesh
        var sphereMesh = SphereMesh(12, 12);

        mesh.vertices = sphereMesh.Item1;
        mesh.triangles = sphereMesh.Item2.ToArray();

        print(mesh.triangles.Length);
    }

    // Mesh with multiple triangles
    (Vector3[], List<int>) RectanglesMesh(int width, int height, int nb_rect_w, int nb_rect_h)
    {
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
                print(x + ";" + y + ";" + z + ";");
                vertices[i] = new Vector3(x, y, z);
                i++;
            }
        }

        for (x = 0; x < nb_rect_w * (nb_rect_h + 1) - 1; x++)
        {
            listTriangles.Add(x);
            listTriangles.Add(x + 1);
            listTriangles.Add(x + nb_rect_h + 1);
            listTriangles.Add(x + 1);
            listTriangles.Add(x + nb_rect_h + 2);
            listTriangles.Add(x + nb_rect_h + 1);
        }

        foreach (object o in listTriangles)
        {
            print(o);
        }

        return (vertices, listTriangles);
    }

    // Cylinder Mesh
    (Vector3[], List<int>) CylinderMesh(float r, float h, int m)
    {   
        //todo size of vertices
        Vector3[] vertices = new Vector3[m * 2];
        List<int> listTriangles = new List<int>();
            
        float x, y, z;
        float teta;
        // Meridian points
        int vidx = 0;
        float teta_step = 2 * Mathf.PI / m;
        for (int i = 0; i < m; i++)
        {
            teta = teta_step * i;

            x = Mathf.Cos(teta) * r;
            y = Mathf.Sin(teta) * r;
            z = h / 2;
            Vector3 p1 = new Vector3(x, z, y);
            print("P : " + x + ";" + y + ";" + -z + ";");
            print("P' : " + x + ";" + y + ";" + z + ";");
            Vector3 p2 = new Vector3(x, -z, y);

            vertices[vidx] = p1;
            vertices[vidx+1] = p2;

            vidx += 2;
        }

        // Facets triangles
        for (int i = 0; i < m * 2; i += 2)
        {
            listTriangles.Add(i % (m * 2));
            listTriangles.Add((i + 3) % (m * 2));
            listTriangles.Add((i + 1) % (m * 2));
            listTriangles.Add(i % (m * 2));
            listTriangles.Add((i + 2) % (m * 2));
            listTriangles.Add((i + 3) % (m * 2));
        }

        //todo
        // Basis facets triangles
/*        for (int i = 0; i < m; i += 1)
        {
            listTriangles.Add(i * 2 % (m * 2));
        }

        for (int i = 0; i < m; i += 1)
        {
            listTriangles.Add(((i * 2) + 1) % (m * 2));
        }*/

        foreach (object o in listTriangles) {
            print(o);
        }

        return (vertices, listTriangles);
    }


    (Vector3[], List<int>) SphereMesh(int meridians, int parallels)
    {
        //todo size of vertices
        Vector3[] vertices = new Vector3[meridians * parallels + 2];
        List<int> listTriangles = new List<int>();

        vertices[0] = new Vector3(0, 1, 0);

        int count = 0;
        for (int i = 0; i < parallels - 1; i++)
        {
            float phi = MathF.PI * (i + 1) / parallels;
            for (int j = 0; j < meridians; j++)
            {
                float theta = 2 * MathF.PI * j / meridians;
                float x = MathF.Sin(phi) * MathF.Cos(theta);
                float y = MathF.Cos(phi);
                float z = MathF.Sin(phi) * MathF.Sin(theta);
                vertices[count] = new Vector3(x, y, z);
                count++;
            }
        }

        vertices[vertices.Length - 1] = new Vector3(0, -1, 0);

        // Bottom and top triangles
        for (int i = 0; i < meridians; i++)
        {
            listTriangles.Add(0);
            listTriangles.Add(((i + 1) % meridians) + 1);
            listTriangles.Add(i + 1);
            listTriangles.Add(vertices.Length - 1);
            listTriangles.Add(i + (parallels - 2) * meridians + 1);
            listTriangles.Add(((i + 1) % meridians) + (parallels - 2) * meridians + 1);
        }
        // Add triangles per parallele and meridian
        for (int i = 0; i < parallels - 2; i++)
        {
            for (int j = 0; j < meridians; j++)
            {
                listTriangles.Add(j + i * meridians + 1);
                listTriangles.Add(((j + 1) % meridians) + i * meridians + 1);
                listTriangles.Add(j + ((i + 1) % parallels) * meridians + 1);

                listTriangles.Add(((j + 1) % meridians) + i * meridians + 1);
                listTriangles.Add(((j + 1) % meridians) + ((i + 1) % parallels) * meridians + 1);
                listTriangles.Add(j + ((i + 1) % parallels) * meridians + 1);
            }
        }

        return (vertices, listTriangles);
    }
        /*
            // Update is called once per frame
            void Update()
            {
            }*/
}
