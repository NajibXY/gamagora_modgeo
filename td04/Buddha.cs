using System;
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
        float radius = buddha.Item4;

        Debug.Log("Radius : " + radius);
        List<Cube> cubeGrid = MeshVox(0.12f, radius, mesh.vertices);
        foreach(Cube cube in cubeGrid)
        {
            Vector3 averagePoint = Vector3.zero;
            foreach (Vector3 point in cube.verticesInside)
            {
                averagePoint += point;
            }
            averagePoint /= cube.verticesInside.Count;
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.GetComponent<Renderer>().material.color = Color.red;
            sphere.transform.position = averagePoint;
            sphere.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
        }


        // Save the mesh
        //WriteOnPath("Assets/Mesh/", name.Split('.')[0]+".obj", mesh.vertices, mesh.triangles, mesh.normals);
    }

    /**
     * Write a mesh on a .obj file
     * @param path: path to the .obj file
     * @param name: name of the .obj file
     * @param vertices: vertices of the mesh
     * @param triangles: triangles of the mesh
     * @param normals: normals of the mesh
     */
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

    /**
     * Load a mesh from a .off file
     * @param path: path to the .off file
     * @return (vertices, listTriangles, normalizedNormals, maxDistance)
     */
    (Vector3[], List<int>, List<Vector3>, float) LoadPath(string path)
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

        // Distance max for radius
        List<float> distances = new List<float>();
        foreach (float x in xs)
        {
            distances.Add(Mathf.Sqrt(Mathf.Pow(x, 2) 
                + Mathf.Pow(ys[xs.IndexOf(x)], 2) 
                + Mathf.Pow(zs[xs.IndexOf(x)], 2))
                );  
        }
        float maxDistance = distances.Max();
        

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
        
        Debug.Log("Mesh loaded !");
        return (vertices, listTriangles, normalizedNormals, maxDistance);
    }

    List<Cube> MeshVox(float precision, float radius, Vector3[] vertices)
    {
        OctreeRegularForSurface octreeReg =
            new OctreeRegularForSurface(
                new Vector3(-radius, -radius, -radius) + Vector3.zero,
                new Vector3(radius, radius, radius) + Vector3.zero,
                radius, precision, vertices
            );

        // lvl 0
        octreeReg.CalculateNodes(octreeReg.root);
        //DrawCube(octreeReg.root, precision / 2, true);
        return GetCubes(octreeReg.root, precision / 2, true);
    }

    List<Cube> GetCubes(Cube node, float precision, bool onlySecant)
    {
        List<Cube> secantCubes = new List<Cube>();
        if (node.isLeaf)
        {
            if (onlySecant)
            {
                //if (!node.isFull && node.isSecante)
                if (node.isSecante)
                {
                    secantCubes.Add(node);
                    //GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    //cube.transform.position = node.GetCenterCube();
                    //cube.transform.localScale = new Vector3(precision, precision, precision);
                }
            }
            else
            {
                if (node.isFull || node.isSecante)
                {
                    secantCubes.Add(node);
                    //GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    //cube.transform.position = node.GetCenterCube();
                    //cube.transform.localScale = new Vector3(precision, precision, precision);
                }
            }
        }
        else
        {
            foreach (Cube child in node.children)
            {
                secantCubes.AddRange(GetCubes(child, precision, onlySecant));
                //DrawCube(child, precision, onlySecant);
            }
        }
        return secantCubes;
    }

    class Cube
    {
        public int level = 0;
        public Vector3 min;
        public Vector3 max;
        public List<Cube> children;
        public bool isSecante;
        public bool isFull;
        public bool isLeaf;
        public List<Vector3> verticesInside;

        public Cube(Vector3 min, Vector3 max, bool isFull, bool isSecante, bool isLeaf)
        {
            children = new List<Cube>();
            verticesInside = new List<Vector3>();
            this.min = min;
            this.max = max;
            this.isFull = isFull;
            this.isSecante = isSecante;
            this.isLeaf = isLeaf;
        }

        public Vector3 GetCenterCube()
        {
            return new Vector3((max.x + min.x) / 2, (max.y + min.y) / 2, (max.z + min.z) / 2);
        }

        public Vector3 GetCenterOfSphere(float radius)
        {

            return new Vector3((max.x + min.x) / 2, (max.y + min.y) / 2, (max.z + min.z) / 2);
        }

        public float Distance(Vector3 a, Vector3 b)
        {
            return MathF.Sqrt(MathF.Pow(a.x - b.x, 2) + MathF.Pow(a.y - b.y, 2) + MathF.Pow(a.z - b.z, 2));
        }

        public float CalculateSideSize()
        {
            return Distance(min, max) / MathF.Sqrt(3);
        }

        public bool IsSecante(Vector3 cubeCenter, float cubeSideLength, Vector3[] verticesList)
        {
            Debug.Log(cubeSideLength);
            bool isSec = false;
            // Check if any of the vertices is in the cube
            Debug.Log(verticesInside.Count);
            foreach (Vector3 vertex in verticesList)
            {
                // Check if the vertex is in the cube
                if (vertex.x >= cubeCenter.x - cubeSideLength / 2 && vertex.x <= cubeCenter.x + cubeSideLength / 2 &&
                    vertex.y >= cubeCenter.y - cubeSideLength / 2 && vertex.y <= cubeCenter.y + cubeSideLength / 2 &&
                    vertex.z >= cubeCenter.z - cubeSideLength / 2 && vertex.z <= cubeCenter.z + cubeSideLength / 2)
                {
                    isSec = true;
                    verticesInside.Add(vertex);
                }
            }
            return isSec;
        }

        // To compare cubes based on their centers
        public override bool Equals(object obj)
        {
            if (obj is Cube otherCube)
            {
                Vector3 center1 = GetCenterCube();
                Vector3 center2 = otherCube.GetCenterCube();
                if (
                    Mathf.Abs(center1.x - center2.x) < 0.0001f &&
                    Mathf.Abs(center1.y - center2.y) < 0.0001f &&
                    Mathf.Abs(center1.z - center2.z) < 0.0001f
                )
                {
                    return true;
                }
            }
            return false;
        }

        public override int GetHashCode()
        {
            return GetCenterCube().GetHashCode();
        }
    }


    class OctreeRegularForSurface
    {
        public Cube root;
        public bool precisionDone;
        public float sphereRadius;
        public Vector3[] vertices;
        public float precision;

        public OctreeRegularForSurface(Vector3 min, Vector3 max, float sphereRadius, float precision, Vector3[] vertices)
        {
            this.root = new Cube(min, max, true, true, true);
            this.precisionDone = false;
            this.sphereRadius = sphereRadius;
            this.precision = precision;
            this.vertices = vertices;
        }

        public void CalculateNodes(Cube node)
        {
            node.isSecante = node.IsSecante(node.GetCenterCube(), precision, vertices);

            if (node.CalculateSideSize() <= precision)
            {
                node.isLeaf = true;
            }
            else
            {
                node.isLeaf = false;
                var min = node.min;
                var max = node.max;
                //divide into 8 children
                Vector3 center = new Vector3(
                    (min.x + max.x) / 2,
                    (min.y + max.y) / 2,
                    (min.z + max.z) / 2
                );
                node.children.Add(new Cube(node.min, center, false, false, false));
                node.children.Add(new Cube(center, node.max, false, false, false));
                node.children.Add(new Cube(
                    new Vector3(center.x, min.y, min.z),
                    new Vector3(max.x, center.y, center.z),
                    false, false, false)); // Bottom-right-front
                node.children.Add(new Cube(
                    new Vector3(min.x, center.y, min.z),
                    new Vector3(center.x, max.y, center.z),
                    false, false, false)); // Top-left-front
                node.children.Add(new Cube(
                    new Vector3(center.x, center.y, min.z),
                    new Vector3(max.x, max.y, center.z),
                    false, false, false)); // Top-right-front

                node.children.Add(new Cube(
                    new Vector3(min.x, min.y, center.z),
                    new Vector3(center.x, center.y, max.z),
                    false, false, false)); // Bottom-left-back
                node.children.Add(new Cube(
                    new Vector3(center.x, min.y, center.z),
                    new Vector3(max.x, center.y, max.z),
                    false, false, false)); // Bottom-right-back
                node.children.Add(new Cube(
                    new Vector3(min.x, center.y, center.z),
                    new Vector3(center.x, max.y, max.z),
                    false, false, false)); // Top-left-back


                foreach (Cube child in node.children)
                {
                    CalculateNodes(child);
                }
            }
        }
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
