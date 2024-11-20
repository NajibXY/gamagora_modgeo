using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line : MonoBehaviour
{
    public List<Vector3> points;
    public int subdivisions = 1;
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
        for (int i = 0; i < points.Count - 1; i++)
        {
            Gizmos.DrawLine(points[i], points[i + 1]);
        }
        Gizmos.DrawLine(points[points.Count - 1], points[0]);
    }

    private void OnDrawGizmosSelected()
    {
        // Chaikin Algorithm using subdivisions
        List<Vector3> newPoints = points;

        for (int i = 0; i < subdivisions; i++)
        {
            List<Vector3> temp = new List<Vector3>();
            for (int j = 0; j < newPoints.Count - 1; j++)
            {
                Vector3 p0 = newPoints[j];
                Vector3 p1 = newPoints[j + 1];
                Vector3 q0 = p0 + (p1 - p0) * 0.25f;
                Vector3 q1 = p0 + (p1 - p0) * 0.75f;
                temp.Add(q0);
                temp.Add(q1);
            }
            Vector3 p00 = newPoints[newPoints.Count - 1];
            Vector3 p11 = newPoints[0];
            Vector3 q00 = p00 + (p11 - p00) * 0.25f;
            Vector3 q11 = p00 + (p11 - p00) * 0.75f;
            temp.Add(q00);
            temp.Add(q11);
            newPoints = temp;
        }
        Gizmos.color = Color.yellow;
        for (int i = 0; i < newPoints.Count - 1; i++)
        {
            Gizmos.DrawLine(newPoints[i], newPoints[i + 1]);
        }
        Gizmos.DrawLine(newPoints[newPoints.Count - 1], newPoints[0]);
        Debug.Log(newPoints.Count);


    }
}
