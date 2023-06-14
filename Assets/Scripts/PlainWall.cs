using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Edge), typeof(MeshFilter))]
public class PlainWall : MonoBehaviour
{
    Edge edge;
    MeshFilter meshFilter;

    // Start is called before the first frame update
    void Start()
    {
        edge = GetComponent<Edge>();
        if (edge == null) { return; }

        meshFilter = gameObject.GetComponent<MeshFilter>();

        draw();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void draw() {
        float thickness = 0.5f;
        float meshDensity = 0.1f;
        Vector3 pointA = edge.points()[0];
        Vector3 pointB = edge.points()[1];
        Mesh mesh = new Mesh();
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        float progress = 0f;
        float distance = Vector3.Distance(pointA, pointB);
        Vector3 dirVec = (pointB - pointA).normalized;
        Vector3 norm2d = Vector3.down;
        Vector3 thickVec = Vector3.Cross(dirVec, norm2d) * thickness / 2f;

        vertices.Add(pointA + thickVec);
        vertices.Add(pointA - thickVec);
        Debug.DrawLine(pointA, pointA + Vector3.up, Color.cyan, 100f);
        Debug.DrawLine(pointB, pointB + Vector3.up, Color.cyan, 100f);
        progress += meshDensity;
        while (progress < distance) {
            Vector3 progPoint = Vector3.Lerp(pointA, pointB, progress);
            Debug.DrawLine(progPoint, progPoint + Vector3.up, Color.cyan, 100f);
            addMeshSegment(ref vertices, ref triangles, progPoint, thickVec);
            progress += meshDensity;
        }
        addMeshSegment(ref vertices, ref triangles, pointB, thickVec);
        for (int i = 0; i < vertices.Count; i++) { vertices[i] -= transform.position; }

        mesh.vertices = vertices.ToArray();
        mesh.uv = new Vector2[vertices.Count];
        mesh.triangles = triangles.ToArray();

        meshFilter.sharedMesh = mesh;
    }

    private void addMeshSegment(ref List<Vector3> vertices, ref List<int> triangles, Vector3 point, Vector3 thickVec) {
        vertices.Add(point + thickVec);
        vertices.Add(point - thickVec);

        int len = vertices.Count;
        triangles.Add(len-4); triangles.Add(len-3); triangles.Add(len-2);
        triangles.Add(len-1); triangles.Add(len-2); triangles.Add(len-3);
    }
}
