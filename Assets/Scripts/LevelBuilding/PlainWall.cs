using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Edge), typeof(LineRenderer))]
public class PlainWall : MonoBehaviour
{
    private Edge _edge;
    private LineRenderer _lineRenderer;

    void Start()
    {
        _edge = GetComponent<Edge>();
        _lineRenderer = GetComponent<LineRenderer>();

        transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        _lineRenderer.numCapVertices = 5;
        _lineRenderer.positionCount = 2;
        _lineRenderer.alignment = LineAlignment.TransformZ;
        _lineRenderer.SetPositions(_edge.points().ToArray());
    }

    private void OnDrawGizmos() {
        List<Vector3> points = GetComponent<Edge>().points();
        if (points != null) {
            Gizmos.color = Color.black;
            Gizmos.DrawLine(points[0], points[1]);
        }
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position, 0.5f);
    }
}
