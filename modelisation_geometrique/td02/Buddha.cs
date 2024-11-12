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


        string name = "bunny.off";

        var buddha = LoadPath("Assets/Mesh/" + name);

        mesh.vertices = buddha.Item1;
        mesh.triangles = buddha.Item2.ToArray();
        mesh.normals = buddha.Item3.ToArray();

        WriteOnPath("Assets/Mesh/", name.Split('.')[0]+".obj", mesh.vertices, mesh.triangles, mesh.normals);
    }

    void WriteOnPath(string path, string name, Vector3[] vertices, int[] triangles, Vector3[] normals)
    {
        using (StreamWriter outputFile = new StreamWriter(Path.Combine(path, name)))
        {
            outputFile.WriteLine("# " + name.Split('.')[0]);

            outputFile.WriteLine("########################### VERTICES ###########################");

            foreach (Vector3 vertice in vertices)
                outputFile.WriteLine("v {0} {1} {2}",
                    vertice.x.ToString(CultureInfo.InvariantCulture), 
                    vertice.y.ToString(CultureInfo.InvariantCulture),
                    vertice.z.ToString(CultureInfo.InvariantCulture)
                );
            
            outputFile.WriteLine("########################### NORMALS ###########################");

            foreach (Vector3 normal in normals)
                outputFile.WriteLine("vn {0} {1} {2}",
                    normal.x.ToString(CultureInfo.InvariantCulture), 
                    normal.y.ToString(CultureInfo.InvariantCulture),
                    normal.z.ToString(CultureInfo.InvariantCulture)
                );

            outputFile.WriteLine("########################### FACETS ###########################");

            int i = 0;
            while (i < triangles.Length) 
            {
                int index1 = triangles[i] + 1;
                int index2 = triangles[i + 1] + 1;
                int index3 = triangles[i + 2] + 1;

                outputFile.WriteLine("f "
                    + index1 + "//" + index1
                    + " " + index2 + "//" + index2
                    + " " + index3 + "//" + index3
                );
                i += 3;
            }
            Debug.Log("File written !");
        }
    }
    // Load Budda.off
    (Vector3[], List<int>, List<Vector3>) LoadPath(string path)
    {
        IEnumerable<string> lines = File.ReadLines(path);
        IEnumerator<string> enumerLines = lines.GetEnumerator();
        enumerLines.MoveNext();
        //Debug.Log(enumerLines.Current);

        enumerLines.MoveNext();
        //Debug.Log(enumerLines.Current);

        // Init vertices and listTriangles
        string[] values = enumerLines.Current.Split(' ');
        Vector3[] vertices = new Vector3[int.Parse(values[0])];
        List<int> listTriangles = new List<int>();
        Vector3[] normals = new Vector3[int.Parse(values[0])];
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
                //Debug.Log("Vertice !");
                // Vertices
                values = enumerLines.Current.Split(' ');
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
                //Debug.Log("Triangle !");

                // Index of Triangles
                i1 = int.Parse(values[1]);
                i2 = int.Parse(values[2]);
                i3 = int.Parse(values[3]);
                listTriangles.Add(i1);
                listTriangles.Add(i2);
                listTriangles.Add(i3);

                // Normal of the triangle applied to each vertex
                p1 = vertices[i1];
                p2 = vertices[i2];
                p3 = vertices[i3];
                normals[i1]+= Vector3.Cross(p2 - p1, p3 - p1);
                normals[i2]+= Vector3.Cross(p2 - p1, p3 - p1);
                normals[i3]+= Vector3.Cross(p2 - p1, p3 - p1);
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
        }
        
        return (vertices, listTriangles, normalizedNormals);
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
