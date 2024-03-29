using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ConvexShape))]
public class ConvexCollider : MonoBehaviour
{
    [SerializeField] bool PointsClockwise = true;
    [SerializeField] bool MakeConvex = true;

    public Mesh ColMesh {get; private set;}

    void Awake()
    {
        gameObject.layer = LayerMask.NameToLayer("Walls");

        float box_height = 80f;
        ConvexShape convexShape = GetComponent<ConvexShape>();
        List<Vector3> points = convexShape.points();
        if (points.Count < 3) {
            Debug.LogError("ConvexCollider for '" + gameObject.name + "' has less than 3 points");
            return;
        }

        Vector3[] vertices = new Vector3[2 * points.Count];
        for (int i = 0; i < points.Count; i++) {
            vertices[i] = points[i] + new Vector3(0, -box_height/2, 0) - transform.position;
        }
        for (int i = 0; i < points.Count; i++) {
            vertices[points.Count + i] = points[i] + new Vector3(0, box_height/2, 0) - transform.position;
        }

        List<int> triangles = new();
        // bottom face triangles
        for (int pI = 1; pI < points.Count - 1; pI++) {
            int v1 = 0;
            int v2 = pI;
            int v3 = pI + 1;
            addTriangle(v1, v2, v3, PointsClockwise, triangles);
        }
        // top face triangles
        for (int pI = points.Count + 1; pI < (points.Count*2) - 1; pI++) {
            int v1 = points.Count;
            int v2 = pI + 1;
            int v3 = pI;
            addTriangle(v1, v2, v3, PointsClockwise, triangles);
        }
        // trunk triangles
        for (int pI = 0; pI < points.Count - 1; pI++) {
            int v1 = pI;
            int v2 = pI + points.Count;
            int v3 = pI + points.Count + 1;
            addTriangle(v1, v2, v3, PointsClockwise, triangles);
            v1 = pI;
            v2 = pI + points.Count + 1;
            v3 = pI + 1;
            addTriangle(v1, v2, v3, PointsClockwise, triangles);
        }
        addTriangle(points.Count - 1, points.Count*2 - 1, points.Count, PointsClockwise, triangles);
        addTriangle(points.Count - 1, points.Count, 0, PointsClockwise, triangles);

        MeshCollider meshCollider = gameObject.AddComponent<MeshCollider>();
        meshCollider.convex = MakeConvex;
        meshCollider.isTrigger = false;
        ColMesh = new Mesh(){
            vertices = vertices,
            triangles = triangles.ToArray(),
        };
        meshCollider.sharedMesh = ColMesh;
    }

    private void addTriangle(int v1, int v2, int v3, bool cw, List<int> triangles) {
        if (cw) {
            triangles.Add(v1);
            triangles.Add(v2);
            triangles.Add(v3);
        } else {
            triangles.Add(v3);
            triangles.Add(v2);
            triangles.Add(v1);
        }
    }
}
