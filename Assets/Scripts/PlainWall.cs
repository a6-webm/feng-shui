using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Edge), typeof(LineRenderer))]
public class PlainWall : MonoBehaviour
{
    private Edge edge;
    private LineRenderer lineRenderer;

    void Start()
    {
        edge = GetComponent<Edge>();
        lineRenderer = GetComponent<LineRenderer>();

        transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        lineRenderer.numCapVertices = 5;
        lineRenderer.positionCount = 2;
        lineRenderer.alignment = LineAlignment.TransformZ;
        lineRenderer.SetPositions(edge.points().ToArray());
    }
}
