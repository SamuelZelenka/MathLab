using System.Collections.Generic;
using UnityEngine;

public class BezierCurve : MonoBehaviour
{
    public Transform p0, p1, p2, p3;
    public Transform cube;
    public  List<Matrix4x4> points = new List<Matrix4x4>();
    public float speed;

    float timeStamp;

    private void Awake()
    {
        for (float i = 0; i < 1; i += 0.01f)
        {
            points.Add(GetPoint(i, p0.position, p1.position, p2.position, p3.position));
        }
    }
    private void Update()
    {
        Matrix4x4 point = GetPoint(Time.time * speed % 1, p0.position, p1.position, p2.position, p3.position);
        cube.rotation = point.rotation;
        cube.position = point.GetColumn(3);
    }

    private void OnDrawGizmos()
    {
        for (int i = 1; i < points.Count; i++)
        {
            Gizmos.DrawLine(points[i - 1].GetColumn(3), points[i].GetColumn(3));
        }
    }

    private Matrix4x4 GetPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        t = Mathf.Clamp01(t);
        float oneMinusT = 1f - t;

        p0 = p0 * ( -Mathf.Pow(t,3) + 3 * Mathf.Pow(t, 2) - 3 * t + 1);
        p1 = p1 * ( 3 * Mathf.Pow(t,3) - 6 * Mathf.Pow(t,2) + 3 * t );
        p2 = p2 * (-3 * Mathf.Pow(t, 3) + 3 * Mathf.Pow(t, 2));
        p3 = p3 * Mathf.Pow(t, 3);

        Vector3 position = p0 + p1 + p2 + p3;
        Quaternion rotation = Quaternion.LookRotation(GetFirstDerivative(p0, p1, p2, p3, t));

        return Matrix4x4.TRS(position, rotation, Vector3.one);
    }

    public static Vector3 GetFirstDerivative(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        p0 = p0 * (-3 * Mathf.Pow(t, 2) + 6 * t - 3);
        p1 = p1 * (9 * Mathf.Pow(t, 2) - 12 * t + 3);
        p2 = p2 * (-9 * Mathf.Pow(t, 2) + 6 * t);
        p3 = p3 * (3 * Mathf.Pow(t, 2));

        return p0 + p1 + p2 + p3;
    }
    public Vector3 GetVelocity(float t)
    {
        return transform.TransformPoint(GetFirstDerivative(p0.position, p1.position, p2.position, p3.position, t)) - transform.position;
    }
}
