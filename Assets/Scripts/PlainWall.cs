using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Edge), typeof(LineRenderer))]
public class PlainWall : MonoBehaviour
{
    private Edge edge;
    private LineRenderer lineRenderer;

    // Start is called before the first frame update
    void Start()
    {
        transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        edge = GetComponent<Edge>();
        if (edge == null) { return; }

        lineRenderer = GetComponent<LineRenderer>();

        lineRenderer.numCapVertices = 5;
        lineRenderer.positionCount = 2;
        lineRenderer.alignment = LineAlignment.TransformZ;
        lineRenderer.SetPositions(edge.points().ToArray());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
