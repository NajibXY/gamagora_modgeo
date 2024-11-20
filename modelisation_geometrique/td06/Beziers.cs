using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Beziers : MonoBehaviour
{
    public List<Vector3> points = new List<Vector3>();
    public float delta = 0.01f;

    // Hermite parameters
    private Vector3 p0, p1, v0, v1;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmos()
    {
        // Draw polygones
        Gizmos.color = Color.red;
        Debug.Log(points.Count);
        for (int i = 0; i < points.Count - 1; i++)
        {
            Gizmos.DrawLine(points[i], points[i + 1]);
        }
        if (points.Count > 0) 
            Gizmos.DrawLine(points[points.Count - 1], points[0]);
        
        Color startColor = Color.green;
        Color endColor = Color.blue;
        p0 = points[0];
        v1 = points[1];
        v0 = points[2];
        p1 = points[3];
    }

    private void OnDrawGizmosSelected()
    {
    List<Vector3> newPoints = new List<Vector3>();
    newPoints.Add(p0);
    float t = delta;
    while(t < 1.0f)
    {
        // float t = i / (float) numPoints - 1.0f;
        Vector3 p = (2.0f * t * t * t - 3.0f * t * t + 1.0f) * p0 
                    + (t * t * t - 2.0f * t * t + t) * v0 
                    + (-2.0f * t * t * t + 3.0f * t * t) * p1 
                    + (t * t * t - t * t) * v1;
        newPoints.Add(p);
        Debug.Log(p);
        t += delta;
    }
    newPoints.Add(p1);
    
    // drawing
    Gizmos.color = Color.green;;
    Gizmos.DrawSphere(p0, 0.1f);
        
    Gizmos.color = Color.yellow;
    for (int i = 0; i < newPoints.Count - 1; i++)
    {
        Gizmos.DrawLine(newPoints[i], newPoints[i + 1]);
    }
        
    Gizmos.color = Color.blue;
    Gizmos.DrawSphere(p1, 0.1f);
    
    //     if (points.Count > 0)
    //     {
    //         for (int i = 0; i < numPoints; i++)
    //         {
    //             float t = i / (float) numPoints;
    //             Vector3 p = (2.0f * t * t * t - 3.0f * t * t + 1.0f) * p0 +
    //                         (t * t * t - 2.0f * t * t + t) * v0 +
    //                         (-2.0f * t * t * t + 3.0f * t * t) * p1 +
    //                         (t * t * t - t * t) * v1;
    //             newPoints.Add(p);
    //         }
    //         newPoints.Add(p1);
    //
    //         
    //         
    //         // drawing
    //         Gizmos.color = startColor;
    //         Gizmos.DrawSphere(p0, 0.1f);
    //         
    //         Gizmos.color = Color.Lerp(startColor, endColor, t);
    //         for (int i = 0; i < newPoints.Count - 1; i++)
    //         {
    //             Gizmos.DrawLine(newPoints[i], newPoints[i + 1]);
    //         }
    //         
    //         Gizmos.color = endColor;
    //         Gizmos.DrawSphere(p1, 0.1f);
    //
    //         
    //         // Chaikin Algorithm using subdivisions
    //         // for (int i = 0; i < subdivisions; i++)
    //         // {
    //         //     List<Vector3> temp = new List<Vector3>();
    //         //     for (int j = 0; j < newPoints.Count - 1; j++)
    //         //     {
    //         //         Vector3 p0 = newPoints[j];
    //         //         Vector3 p1 = newPoints[j + 1];
    //         //         Vector3 q0 = p0 + (p1 - p0) * 0.25f;
    //         //         Vector3 q1 = p0 + (p1 - p0) * 0.75f;
    //         //         temp.Add(q0);
    //         //         temp.Add(q1);
    //         //     }
    //         //     Vector3 p00 = newPoints[newPoints.Count - 1];
    //         //     Vector3 p11 = newPoints[0];
    //         //     Vector3 q00 = p00 + (p11 - p00) * 0.25f;
    //         //     Vector3 q11 = p00 + (p11 - p00) * 0.75f;
    //         //     temp.Add(q00);
    //         //     temp.Add(q11);
    //         //     newPoints = temp;
    //         // }
    //         // Gizmos.color = Color.yellow;
    //         // for (int i = 0; i < newPoints.Count - 1; i++)
    //         // {
    //         //     Gizmos.DrawLine(newPoints[i], newPoints[i + 1]);
    //         // }
    //         // Gizmos.DrawLine(newPoints[newPoints.Count - 1], newPoints[0]);
    //         // Debug.Log(newPoints.Count);
    //     }
    //
    //
    }
}
