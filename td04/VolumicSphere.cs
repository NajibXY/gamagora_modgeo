using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.PlayerLoop;
using UnityEngine.SearchService;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;
using static UnityEditor.Progress;

public class VolumicSphere : MonoBehaviour {
    // Start is called before the first frame update
    public float precision;
    public float radius;
    public bool onlySecant;

    public bool isForOperandsTests;
    public bool isUnion;
    public bool isIntersect;
    public List<SphereComplete> otherSpheres;

    private List<float> radiuses;
    private List<Vector3> centers;
    void Start() {
        radiuses = new List<float>();
        centers = new List<Vector3>();

        // Init Mesh components
        gameObject.GetComponent<MeshRenderer>();
        Mesh mesh = GetComponent<MeshFilter>().mesh;

        mesh.Clear();

        // Sphere Mesh
        var myMesh = SphereMesh(12, 12);

        mesh.vertices = myMesh.Item1;
        mesh.triangles = myMesh.Item2.ToArray();

        if (isForOperandsTests) {
            /*          UnityEngine.Debug.Log(otherSpheres[0].getSphereCenter());
                        SphereVox(precision, radius, otherSpheres[0].getSphereCenter(), onlySecant);*/

            centers.Add(gameObject.transform.position);
            radiuses.Add(radius);
            foreach (SphereComplete sc in otherSpheres) {
                centers.Add(sc.getSphereCenter());
                radiuses.Add(sc.getRadius());
            }

            if (isUnion) {
                List<Cube> unionList = UnionSphereVox(precision, radiuses, centers, onlySecant);
                DrawPrimitiveCubesList(unionList, precision);
            } else if (isIntersect) {
                List<Cube> interesectList = IntersectSphereVox(precision, radiuses, centers, onlySecant);
                DrawPrimitiveCubesList(interesectList, precision);
            }
        } else {
            // Sphere Vox
            SphereVox(precision, radius, gameObject.transform.position, onlySecant);
        }
    }

    void DrawPrimitiveCubesList(List<Cube> cubes, float precision) {
        foreach (Cube cube in cubes) {
            GameObject cubePrimitive = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cubePrimitive.transform.position = cube.GetCenterCube();
            cubePrimitive.transform.localScale = new Vector3(precision, precision, precision);
        }
    }

    //todo FIIIIIIIIX
    List<Cube> IntersectSphereVox(float precision, List<float> sphereRadiuses, List<Vector3> sphereCenters, bool onlySecant) {
        List<List<Cube>> allLists = new List<List<Cube>>();
        for (int i = 0; i < sphereRadiuses.Count; i++) {
            UnityEngine.Debug.Log("sphere radius : " + sphereRadiuses[i]);
            OctreeRegular octreeReg =
                new OctreeRegular(
                    new Vector3(-sphereRadiuses[i], -sphereRadiuses[i], -sphereRadiuses[i]) + sphereCenters[i],
                    new Vector3(sphereRadiuses[i], sphereRadiuses[i], sphereRadiuses[i]) + sphereCenters[i],
                    sphereRadiuses[i], precision, sphereCenters[i]
                );

            // lvl 0
            octreeReg.CalculateNodes(octreeReg.root);
            List<Cube> listeCubes = GetCubes(octreeReg.root, precision, onlySecant);
            allLists.Add(listeCubes);
        }

        var intersectCubes = new HashSet<Cube>(allLists[0]);
        List<Cube> intersectList = intersectCubes.ToList();

        for (int i = 1; i < allLists.Count; i++) {
            // Intersection
            intersectList = FindCommonCubes(intersectList, sphereRadiuses[i], sphereCenters[i]);
        }

        return intersectList;
    }

    List<Cube> UnionSphereVox(float precision, List<float> sphereRadiuses, List<Vector3> sphereCenters, bool onlySecant) {
        List<Cube> allCubes = new List<Cube>();
        for (int i = 0; i < sphereRadiuses.Count; i++) {
            OctreeRegular octreeReg =
                new OctreeRegular(
                    new Vector3(-sphereRadiuses[i], -sphereRadiuses[i], -sphereRadiuses[i]) + sphereCenters[i],
                    new Vector3(sphereRadiuses[i], sphereRadiuses[i], sphereRadiuses[i]) + sphereCenters[i],
                    sphereRadiuses[i], precision, sphereCenters[i]
                );

            // lvl 0
            octreeReg.CalculateNodes(octreeReg.root);
            List<Cube> listeCubes = GetCubes(octreeReg.root, precision, onlySecant);
            allCubes.AddRange(listeCubes);
        }

        //List<Cube> unionList = new List<Cube>();

        //for (int j = 0; j < allLists.Count; j++) {
        //    var firstList = new HashSet<Cube>(allLists[j]);
        //    for (int i = 0; i < allLists.Count; i++) {
        //        if (i != j) {
        //            List<Cube> nonCommonCubes = ExcludeCommonCubes(firstList.ToList(), sphereRadiuses[i], sphereCenters[i]);
        //            foreach (Cube cube in nonCommonCubes) {
        //                if (!unionList.Contains(cube)) {
        //                    unionList.Add(cube);
        //                }
        //            }
        //        }
        //    }
        //}

        UnityEngine.Debug.Log("All cubes List size : " + allCubes.Count);
        return ExcludeCommonCubes(allCubes, IntersectSphereVox(precision, sphereRadiuses, sphereCenters, onlySecant));
    }

    static List<Cube> ExcludeCommonCubes(List<Cube> list, List<Cube> common) {
        List<Cube> exclusiveUnion = list.Except(common).ToList();
        UnityEngine.Debug.Log("Union size : " + exclusiveUnion.Count);
        return exclusiveUnion;
    }

    static List<Cube> FindCommonCubes(List<Cube> list1, float otherRadius, Vector3 otherCenter) {
        List<Cube> commonCubes = new List<Cube>();
        for (int i = 0; i < list1.Count; i++) {
            Cube cube = list1[i];
            if ((cube.IsInSphere(cube.GetCenterCube(), cube.CalculateSideSize(), otherCenter, otherRadius)
            || cube.IsSecante(cube.GetCenterCube(), cube.CalculateSideSize(), otherCenter, otherRadius))
            && !commonCubes.Contains(cube)) {
                commonCubes.Add(cube);
            }
        }
        if (commonCubes.Count == 0) {
            UnityEngine.Debug.Log("No common cubes found");
        }
        return commonCubes;
    }

    List<Cube> GetCubes(Cube node, float precision, bool onlySecant) {
        List<Cube> listeCubes = new List<Cube>();

        if (node.isLeaf) {
            if (onlySecant) {
                if (!node.isFull && node.isSecante) {
                    listeCubes.Add(node);
                }
            } else {
                if (node.isFull || node.isSecante) {
                    listeCubes.Add(node);
                }
            }
        } else {
            foreach (Cube child in node.children) {
                listeCubes.AddRange(GetCubes(child, precision, onlySecant));
            }
        }

        return listeCubes;

    }


    (Vector3[], List<int>) SphereMesh(int meridians, int parallels) {
        // Vertices: (meridians * (parallels - 1)) + 2 (for poles)
        Vector3[] vertices = new Vector3[meridians * (parallels - 1) + 2];
        List<int> listTriangles = new List<int>();

        // Top pole
        vertices[0] = new Vector3(0, radius, 0);

        int count = 1;
        // Generate vertices for each parallel
        for (int i = 0; i < parallels - 1; i++) {
            float phi = MathF.PI * (i + 1) / parallels;
            for (int j = 0; j < meridians; j++) {
                float theta = 2 * MathF.PI * j / meridians;
                float x = MathF.Sin(phi) * MathF.Cos(theta) * radius;
                float y = MathF.Cos(phi) * radius;
                float z = MathF.Sin(phi) * MathF.Sin(theta) * radius;
                vertices[count] = new Vector3(x, y, z);
                count++;
            }
        }

        // Bottom pole
        vertices[vertices.Length - 1] = new Vector3(0, -radius, 0);

        // Top pole triangles
        for (int i = 0; i < meridians; i++) {
            listTriangles.Add(0);
            listTriangles.Add(i + 1);
            listTriangles.Add((i + 1) % meridians + 1);
        }

        // Middle triangles between excluding poles
        for (int i = 0; i < parallels - 2; i++) {
            for (int j = 0; j < meridians; j++) {
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
        for (int i = 0; i < meridians; i++) {
            listTriangles.Add(vertices.Length - 1); // Bottom pole vertex
            listTriangles.Add((i + 1) % meridians + (parallels - 2) * meridians + 1);
            listTriangles.Add(i + (parallels - 2) * meridians + 1);
        }

        return (vertices, listTriangles);
    }


    void SphereVox(float precision, float sphereRadius, Vector3 sphereCenter, bool onlySecant) {
        OctreeRegular octreeReg =
            new OctreeRegular(
                new Vector3(-sphereRadius, -sphereRadius, -sphereRadius) + sphereCenter,
                new Vector3(sphereRadius, sphereRadius, sphereRadius) + sphereCenter,
                sphereRadius, precision, sphereCenter
            );

        // lvl 0
        octreeReg.CalculateNodes(octreeReg.root);
        DrawCube(octreeReg.root, precision, onlySecant);
    }

    void DrawCube(Cube node, float precision, bool onlySecant) {
        if (node.isLeaf) {
            if (onlySecant) {
                if (!node.isFull && node.isSecante) {
                    GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cube.transform.position = node.GetCenterCube();
                    cube.transform.localScale = new Vector3(precision, precision, precision);
                }
            } else {
                if (node.isFull || node.isSecante) {
                    GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cube.transform.position = node.GetCenterCube();
                    cube.transform.localScale = new Vector3(precision, precision, precision);
                }
            }
        } else {
            foreach (Cube child in node.children) {
                DrawCube(child, precision, onlySecant);
            }
        }
    }

    class Cube {
        public int level = 0;
        public Vector3 min;
        public Vector3 max;
        public List<Cube> children;
        public bool isSecante;
        public bool isFull;
        public bool isLeaf;

        public Cube(Vector3 min, Vector3 max, bool isFull, bool isSecante, bool isLeaf) {
            children = new List<Cube>();
            this.min = min;
            this.max = max;
            this.isFull = isFull;
            this.isSecante = isSecante;
            this.isLeaf = isLeaf;
        }

        public Vector3 GetCenterCube() {
            return new Vector3((max.x + min.x) / 2, (max.y + min.y) / 2, (max.z + min.z) / 2);
        }

        public Vector3 GetCenterOfSphere(float radius) {

            return new Vector3((max.x + min.x) / 2, (max.y + min.y) / 2, (max.z + min.z) / 2);
        }

        public float Distance(Vector3 a, Vector3 b) {
            return MathF.Sqrt(MathF.Pow(a.x - b.x, 2) + MathF.Pow(a.y - b.y, 2) + MathF.Pow(a.z - b.z, 2));
        }

        public float CalculateSideSize() {
            return Distance(min, max) / MathF.Sqrt(3);
        }

        public bool IsInSphere(Vector3 cubeCenter, float cubeSideLength, Vector3 sphereCenter, float sphereRadius) {
            // Half the side length of the cube
            float halfSideLength = cubeSideLength / 2;

            // List of cube vertices relative to the cube center
            Vector3[] vertices = new Vector3[8];
            vertices[0] = new Vector3(cubeCenter.x - halfSideLength, cubeCenter.y - halfSideLength, cubeCenter.z - halfSideLength);
            vertices[1] = new Vector3(cubeCenter.x + halfSideLength, cubeCenter.y - halfSideLength, cubeCenter.z - halfSideLength);
            vertices[2] = new Vector3(cubeCenter.x - halfSideLength, cubeCenter.y + halfSideLength, cubeCenter.z - halfSideLength);
            vertices[3] = new Vector3(cubeCenter.x + halfSideLength, cubeCenter.y + halfSideLength, cubeCenter.z - halfSideLength);
            vertices[4] = new Vector3(cubeCenter.x - halfSideLength, cubeCenter.y - halfSideLength, cubeCenter.z + halfSideLength);
            vertices[5] = new Vector3(cubeCenter.x + halfSideLength, cubeCenter.y - halfSideLength, cubeCenter.z + halfSideLength);
            vertices[6] = new Vector3(cubeCenter.x - halfSideLength, cubeCenter.y + halfSideLength, cubeCenter.z + halfSideLength);
            vertices[7] = new Vector3(cubeCenter.x + halfSideLength, cubeCenter.y + halfSideLength, cubeCenter.z + halfSideLength);

            // Check if all vertices are inside the sphere
            foreach (Vector3 vertex in vertices) {
                // Calculate the distance from the sphere center to the vertex
                float distanceSquared = (vertex.x - sphereCenter.x) * (vertex.x - sphereCenter.x)
                                      + (vertex.y - sphereCenter.y) * (vertex.y - sphereCenter.y)
                                      + (vertex.z - sphereCenter.z) * (vertex.z - sphereCenter.z);

                // If any vertex is outside the sphere, return false
                if (distanceSquared > sphereRadius * sphereRadius) {
                    return false;
                }
            }

            // If all vertices are inside the sphere, return true
            return true;
        }

        public bool IsSecante(Vector3 cubeCenter, float cubeSideLength, Vector3 sphereCenter, float sphereRadius) {
            // Half the side length of the cube
            float halfSideLength = cubeSideLength / 2;

            // Find the closest point on the cube to the sphere center
            float closestx = Math.Max(cubeCenter.x - halfSideLength, Math.Min(sphereCenter.x, cubeCenter.x + halfSideLength));
            float closesty = Math.Max(cubeCenter.y - halfSideLength, Math.Min(sphereCenter.y, cubeCenter.y + halfSideLength));
            float closestz = Math.Max(cubeCenter.z - halfSideLength, Math.Min(sphereCenter.z, cubeCenter.z + halfSideLength));

            // Calculate the distance between the closest point on the cube and the center of the sphere
            float distanceSquared = (closestx - sphereCenter.x) * (closestx - sphereCenter.x)
                                  + (closesty - sphereCenter.y) * (closesty - sphereCenter.y)
                                  + (closestz - sphereCenter.z) * (closestz - sphereCenter.z);

            // Check if the distance is less than or equal to the radius of the sphere squared
            return distanceSquared <= (sphereRadius * sphereRadius);
        }

        // To compare cubes based on their centers
        public override bool Equals(object obj) {
            if (obj is Cube otherCube) {
                Vector3 center1 = GetCenterCube();
                Vector3 center2 = otherCube.GetCenterCube();
                if (
                    Mathf.Abs(center1.x - center2.x) < 0.0001f &&
                    Mathf.Abs(center1.y - center2.y) < 0.0001f &&
                    Mathf.Abs(center1.z - center2.z) < 0.0001f
                ) {
                    return true;
                }
            }
            return false;
        }

        public override int GetHashCode() {
            return GetCenterCube().GetHashCode();
        }
    }


    class OctreeRegular {
        public Cube root;
        public bool precisionDone;
        public float sphereRadius;
        public Vector3 sphereCenter;
        public float precision;

        public OctreeRegular(Vector3 min, Vector3 max, float sphereRadius, float precision, Vector3 sphereCenter) {
            this.root = new Cube(min, max, true, true, true);
            this.precisionDone = false;
            this.sphereRadius = sphereRadius;
            this.precision = precision;
            this.sphereCenter = sphereCenter;
        }

        public void CalculateNodes(Cube node) {
            node.isFull = node.IsInSphere(node.GetCenterCube(), precision, sphereCenter, sphereRadius);
            node.isSecante = node.IsSecante(node.GetCenterCube(), precision, sphereCenter, sphereRadius);

            if (node.CalculateSideSize() <= precision) {
                node.isLeaf = true;
            } else {
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
                    new Vector3(center.x - (node.max.x - node.min.x) / 2, center.y, center.z),
                    new Vector3(node.max.x - (node.max.x - node.min.x) / 2, node.max.y, node.max.z)
                    , false, false, false)
                );
                node.children.Add(new Cube(
                    new Vector3(node.min.x, node.min.y, node.min.z + (node.max.z - node.min.z) / 2),
                    new Vector3(center.x, center.y, center.z + (node.max.z - node.min.z) / 2)
                    , false, false, false)
                );

                foreach (Cube child in node.children) {
                    CalculateNodes(child);
                }
            }
        }
    }
}
