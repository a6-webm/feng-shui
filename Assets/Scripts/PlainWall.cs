using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Edge), typeof(LineRenderer))]
public class PlainWall : MonoBehaviour
{
    Edge edge;
    LineRenderer lineRenderer;

    // Start is called before the first frame update
    void Start()
    {
        edge = GetComponent<Edge>();
        if (edge == null) { return; }

        lineRenderer = GetComponent<LineRenderer>();

        lineRenderer.numCapVertices = 5;
        lineRenderer.positionCount = 2;
        lineRenderer.SetPositions(edge.points().ToArray());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
