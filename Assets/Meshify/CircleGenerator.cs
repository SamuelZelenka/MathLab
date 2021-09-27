using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleGenerator : MonoBehaviour
{

    public float radius;
    public int resolution;


    private void Start()
    {
        MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = new Material(Shader.Find("Standard"));

        MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();

        Mesh mesh = new Mesh();

        Vector3[] vertices = GetPointsOnCircle();
        mesh.vertices = vertices;

        List<int> tris = new List<int>();

        for (int i = 2; i < vertices.Length; i++)
        {
            tris.Add(0);
            tris.Add(i - 1);
            tris.Add(i);
        }

        mesh.triangles = tris.ToArray();
        mesh.RecalculateNormals();



        for (int i = 2; i < resolution; i++)
        {
            tris.Add(0);
            tris.Add(i - 1);
            tris.Add(i);
        }
        tris.Add(0);
        tris.Add(resolution - 1);
        tris.Add(resolution);

        meshFilter.mesh = mesh;

    }

    public Vector3[] GetPointsOnCircle()
    {
        List<Vector3> points = new List<Vector3>();
        points.Add(transform.position);
        for (int i = 0; i < resolution; i++)
        {
            float angle = ((Mathf.PI * 2) / resolution) * i;

            points.Add(new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius);
        }

        return points.ToArray();
    }


    private void OnDrawGizmos()
    {
        Vector3[] points = GetPointsOnCircle();
        
        for (int i = 1; i < points.Length; i++)
        {
            Gizmos.DrawSphere(transform.position + points[i], 0.1f);
        }
    }
}
