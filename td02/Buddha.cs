using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Buddha : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // Init Mesh components
        gameObject.GetComponent<MeshRenderer>();
        Mesh mesh = GetComponent<MeshFilter>().mesh;

        mesh.Clear();


        var buddha = LoadPath("Assets/Mesh/buddha.off");

        mesh.vertices = buddha.Item1;
        mesh.triangles = buddha.Item2.ToArray();
        // todo reallocate normals
        // mesh.normals = buddha.Item3.ToArray();
    }

    // Load Budda.off
    (Vector3[], List<int>, List<Vector3>) LoadPath(string path)
    {
        IEnumerable<string> lines = File.ReadLines(path);
        IEnumerator<string> enumerLines = lines.GetEnumerator();
        enumerLines.MoveNext();
        Debug.Log(enumerLines.Current);

        enumerLines.MoveNext();
        Debug.Log(enumerLines.Current);

        // Init vertices and listTriangles
        string[] values = enumerLines.Current.Split(' ');
        Vector3[] vertices = new Vector3[int.Parse(values[0])];
        List<int> listTriangles = new List<int>();
        List<Vector3> normals = new List<Vector3>();
        int i = 0;
        List<float> xs = new List<float>();
        List<float> ys = new List<float>();
        List<float> zs = new List<float>();
        float length = 0.0f;
        int i1, i2, i3;
        Vector3 p1, p2, p3;
        while (enumerLines.MoveNext())
        {   
            values = enumerLines.Current.Split(' ');
            if (values.Length == 3)
            {
                Debug.Log("Vertice !");
                // Vertices
                values = enumerLines.Current.Split(' ');
                //todo enlever * 10
                float x = float.Parse(values[0], CultureInfo.InvariantCulture) * 10.0f;
                float y = float.Parse(values[1], CultureInfo.InvariantCulture) * 10.0f;
                float z = float.Parse(values[2], CultureInfo.InvariantCulture) * 10.0f;
                xs.Add(x);
                ys.Add(y);
                zs.Add(z);
                
                float squaredLength = Mathf.Pow(x,2) + Mathf.Pow(y,2) + Mathf.Pow(z, 2);
                if (squaredLength > length)
                {
                    length = squaredLength;
                }

                vertices[i] = new Vector3(x, y, z);
                i++;
            } 
            else if (values.Length == 5)
            {
                Debug.Log("Triangle !");

                // Index of Triangles
                i1 = int.Parse(values[1]);
                i2 = int.Parse(values[2]);
                i3 = int.Parse(values[3]);
                listTriangles.Add(i1);
                listTriangles.Add(i2);
                listTriangles.Add(i3);

                // Normal of the triangle
                p1 = vertices[i1];
                p2 = vertices[i2];
                p3 = vertices[i3];
                normals.Add(Vector3.Scale(p2 - p1, p3-p1));
            }
        }

        // Center of gravity
        float averageX = xs.Average();
        float averageY = ys.Average();
        float averageZ = zs.Average();
        Vector3 centerGravity = new Vector3(averageX, averageY, averageZ);

        float sqrtLength = Mathf.Sqrt(length);

        for (i = 0; i < vertices.Length; i++)
        {
            vertices[i] = new Vector3(vertices[i].x / sqrtLength, vertices[i].y / sqrtLength, vertices[i].z / sqrtLength) - centerGravity;
        }

        List<Vector3> normalizedNormals = new List<Vector3>();
        foreach (Vector3 normal in normals)
        {
            normalizedNormals.Add(normal.normalized);
            normalizedNormals.Add(normal.normalized);
            normalizedNormals.Add(normal.normalized);
        }
        return (vertices, listTriangles, normalizedNormals);
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
