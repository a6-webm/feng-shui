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
}
