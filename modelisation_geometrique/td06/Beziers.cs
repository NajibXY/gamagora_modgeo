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
        
        p0 = points[0];
        v1 = points[1];
        v0 = points[2];
        p1 = points[3];
    }

    private void OnDrawGizmosSelected()
    {
    // Hermite
    // List<Vector3> newPoints = Hermite();
    
    // Bernstein
    List<Vector3> newPoints = Bernstein();
    
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
    }

    public List<Vector3> Bernstein()
    {
        List<Vector3> controlPoints = new List<Vector3>();
        controlPoints.AddRange(points);
        int n = controlPoints.Count - 1;
        
        List<Vector3> newPoints = new List<Vector3>();

        float t = delta;
        
        // Bernstein polynomial
        while (t < 1.0f)
        {
            Vector3 p = Vector3.zero;
            for (int i = 0; i < controlPoints.Count; i++)
            {
                p += BinomialCoefficient(n, i) * controlPoints[i] * Mathf.Pow(1 - t, controlPoints.Count - 1 - i) * Mathf.Pow(t, i);
            }
            
            newPoints.Add(p);
            t += delta;
        }

        return newPoints;
    }
    
    private static int BinomialCoefficient(int n, int k)
    {
        if (k > n) return 0;
        if (k == 0 || k == n) return 1;

        int result = 1;
        for (int i = 1; i <= k; i++)
        {
            result *= n - (k - i);
            result /= i;
        }
        return result;
    }

    public List<Vector3> Hermite()
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
            t += delta;
        }
        
        newPoints.Add(p1);

        return newPoints;
    }
}
