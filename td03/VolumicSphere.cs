using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.PlayerLoop;
using UnityEngine.SearchService;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;
using static UnityEditor.Progress;

public class VolumicSphere : MonoBehaviour
{
    // Start is called before the first frame update

    void Start()
    {
        // Init Mesh components
        gameObject.GetComponent<MeshRenderer>();
        Mesh mesh = GetComponent<MeshFilter>().mesh;

        mesh.Clear();

        // Sphere Mesh
                var myMesh = SphereMesh(12, 12);

                mesh.vertices = myMesh.Item1;
                mesh.triangles = myMesh.Item2.ToArray();

        // Sphere Vox
        SphereVox(0.5f, 1.0f, new Vector3(0.0f,0.0f,0.0f));
    }

    (Vector3[], List<int>) SphereMesh(int meridians, int parallels)
    {
        // Vertices: (meridians * (parallels - 1)) + 2 (for poles)
        Vector3[] vertices = new Vector3[meridians * (parallels - 1) + 2];
        List<int> listTriangles = new List<int>();

        // Top pole
        vertices[0] = new Vector3(0, 1f, 0);

        int count = 1;
        // Generate vertices for each parallel
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

        // Bottom pole
        vertices[vertices.Length - 1] = new Vector3(0, -1f, 0);

        // Top pole triangles
        for (int i = 0; i < meridians; i++)
        {
            listTriangles.Add(0);
            listTriangles.Add(i + 1);
            listTriangles.Add((i + 1) % meridians + 1);
        }

        // Middle triangles between excluding poles
        for (int i = 0; i < parallels - 2; i++)
        {
            for (int j = 0; j < meridians; j++)
            {
                int current = j + i * meridians + 1;
                int next = (j + 1) % meridians + i * meridians + 1;
                int below = j + (i + 1) * meridians + 1;
                int belowNext = (j + 1) % meridians + (i + 1) * meridians + 1;

                // First triangle
                listTriangles.Add(below);
                listTriangles.Add(next);
                listTriangles.Add(current);

                // Second triangle
                listTriangles.Add(below);
                listTriangles.Add(belowNext);
                listTriangles.Add(next);
            }
        }

        // Bottom pole triangles
        for (int i = 0; i < meridians; i++)
        {
            listTriangles.Add(vertices.Length - 1); // Bottom pole vertex
            listTriangles.Add((i + 1) % meridians + (parallels - 2) * meridians + 1);
            listTriangles.Add(i + (parallels - 2) * meridians + 1);
        }

        return (vertices, listTriangles);
    }


    void SphereVox(float precision, float sphereRadius, Vector3 sphereCenter)
    {
        float diametre = sphereRadius * 2;
        UnityEngine.Debug.Log("diametre : " + diametre);
        OctreeRegular octreeReg =
            new OctreeRegular(
                new Vector3(-diametre / 2, - diametre / 2, - diametre / 2), 
                new Vector3(diametre / 2, diametre / 2, diametre / 2),
                sphereRadius, precision, sphereCenter
            );

        // lvl 0
        octreeReg.CalculateNodes(octreeReg.root);

        // Draw - todo use secant ?
        //
        DrawCube(octreeReg.root, precision);


        /*        // Create cubes representing the sphere
                Vector3 scaleVox = new Vector3(1f / subdivisions, 1f / subdivisions, 1f / subdivisions);
                for (int i = 0; i < subdivisions; i++)
                {
                    for (int j = 0; j < subdivisions; j++)
                    {
                        for (int k = 0; k < subdivisions; k++)
                        {
                            float x = (float)i / (float)subdivisions;
                            float y = (float)j / (float)subdivisions;
                            float z = (float)k / (float)subdivisions;

                            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);

                            cube.transform.position = new Vector3(x, y, z);
                            cube.transform.localScale = scaleVox;

                        }
                    }
                }*/
    }

    void DrawCube(Cube node, float precision)
    {
        if (node.isLeaf)
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = node.GetCenterCube() ;
            cube.transform.localScale = new Vector3(precision, precision, precision);
        } else
        {
            foreach (Cube child in node.children)
            {
                DrawCube(child, precision);
            }
        }
        
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

        public Cube(Vector3 min, Vector3 max, bool isFull, bool isSecante, bool isLeaf)
        {
            children = new List<Cube>();
            this.min = min;
            this.max = max;
            this.isFull = isFull;
            //todo How to use isSecante ?
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

        public bool IsInSphere(Vector3 point, float radius)
        {
            float distanceWithCenter = Distance(point, new Vector3(0, 0, 0));
            //todo check if distance is correct
            UnityEngine.Debug.Log("distance with center : " + distanceWithCenter);
            UnityEngine.Debug.Log(Math.Abs(radius - distanceWithCenter) <= 0.001f);
            return Math.Abs(radius-distanceWithCenter) <= 0.001f;
        }

        public float CalculateSideSize()
        {
            return Distance(min, max) / MathF.Sqrt(3) ;
        }
    }


    class OctreeRegular
    {
        public Cube root;
        public bool precisionDone;
        public float sphereRadius;
        public Vector3 sphereCenter;
        public float precision;

        public OctreeRegular(Vector3 min, Vector3 max, float sphereRadius, float precision, Vector3 sphereCenter)
        {
            this.root = new Cube(min, max, true, true, true);
            this.precisionDone = false;
            this.sphereRadius = sphereRadius;
            this.precision = precision;
            this.sphereCenter = sphereCenter;
        }

        public void CalculateNodes(Cube node)
        {
            UnityEngine.Debug.Log("On calcule des gosses");
            UnityEngine.Debug.Log("Center : " + node.GetCenterCube());
            node.isFull = node.IsInSphere(node.GetCenterCube(), sphereRadius);

            UnityEngine.Debug.Log("node amplitude : " + node.Distance(node.min, node.max));
            //if (node.isFull & node.Distance(node.min, node.max) <= precision)
            UnityEngine.Debug.Log("Cube side size : " + node.CalculateSideSize());


            // Define which are the leafs to display

            /*if (node.isFull & node.CalculateSideSize() <= precision)
            {
                UnityEngine.Debug.Log(node.Distance(node.min, node.max));
                node.isLeaf = true;
            }*/
            if (node.CalculateSideSize() <= precision)
            {
                UnityEngine.Debug.Log(node.Distance(node.min, node.max));
                node.isLeaf = true;
            }
            else
            {
                node.isLeaf = false;

                //divide into 8 children
                Vector3 center = node.GetCenterCube();
                node.children.Add(new Cube(node.min, center, false, false, false));
                node.children.Add(new Cube(center, node.max, false, false, false));
                node.children.Add(new Cube(
                    new Vector3(node.min.x, center.y, node.min.z),
                    new Vector3(center.x, center.y + (node.max.y - node.min.y) / 2, center.z),
                    false, false, false)
                );
                node.children.Add(new Cube(
                    new Vector3(node.min.x + (node.max.x - node.min.x) / 2, node.min.y, node.min.z),
                    new Vector3(center.x + (node.max.x - node.min.x) / 2, center.y, center.z),
                    false, false, false)
                );
                node.children.Add(new Cube(
                    new Vector3(center.x, center.y, center.z - (node.max.z - node.min.z) / 2),
                    new Vector3(node.max.x, node.max.y, node.max.z - (node.max.z - node.min.z) / 2),
                    false, false, false)
                );
                node.children.Add(new Cube(
                    new Vector3(node.min.x + (node.max.x - node.min.x) / 2, node.min.y, node.min.z + (node.max.z - node.min.z) / 2),
                    new Vector3(center.x + (node.max.x - node.min.x) / 2, center.y, center.z + (node.max.z - node.min.z) / 2),
                    false, false, false)
                );
                node.children.Add(new Cube(
                    new Vector3(center.x - (node.max.x - node.min.x) / 2, center.y , center.z),
                    new Vector3(node.max.x - (node.max.x - node.min.x) / 2, node.max.y, node.max.z)
                    , false, false, false)
                );
                node.children.Add(new Cube(
                    new Vector3(node.min.x, node.min.y, node.min.z + (node.max.z - node.min.z) / 2),
                    new Vector3(center.x, center.y, center.z + (node.max.z - node.min.z) / 2)
                    , false, false, false)
                );

                foreach (Cube child in node.children)
                {
                    CalculateNodes(child);
                }
            }
        }
    }
}
