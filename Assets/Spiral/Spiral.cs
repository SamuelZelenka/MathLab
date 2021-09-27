using System.Collections.Generic;
using UnityEngine;

public class Spiral : MonoBehaviour
{
    public int _pointOnSpiral = 1;
    [Range(1,100)] public int repeatCount;
    public float height;
    public float radius;

    public BezierCurve bezierCurve;

    List<Vector3> points = new List<Vector3>();



    private void OnDrawGizmos()
    {
        points.Clear();

        Vector3 previousPosition = transform.position + new Vector3(radius, 0, 0);

        for (int i = 1; i < bezierCurve.points.Count; i++)
        {
            Matrix4x4 matrix = Matrix4x4.TRS(bezierCurve.points[i].GetColumn(3), bezierCurve.points[i].rotation, transform.localScale);
            Vector3 newPos = CalcPoint(matrix, i);
            Gizmos.DrawLine(newPos, previousPosition);
            previousPosition = newPos;
        }

        for (int i = 0; i < bezierCurve.points.Count; i++)
        {
            Gizmos.color = new Color(bezierCurve.points.Count / (i + 1),0,0);
            Vector3 point = CalcPoint(bezierCurve.points[i], i);
            Gizmos.DrawSphere(point, 1);
        }
    }

    public Vector3 CalcPoint(Matrix4x4 matrix, int pointCount)
    {
        points.Add(matrix.MultiplyVector(CalculatePointPosition(pointCount)));
        return matrix.MultiplyVector(CalculatePointPosition(pointCount));

        Vector3 CalculatePointPosition(int currrentPointCount)
        {
            float theta = ((Mathf.PI * 2) / _pointOnSpiral);
            float angle = (theta * currrentPointCount);

            return new Vector3(radius * Mathf.Cos(angle), height * currrentPointCount, radius * Mathf.Sin(angle)) + (Vector3)bezierCurve.points[currrentPointCount].GetColumn(3);
        }
    }
}
