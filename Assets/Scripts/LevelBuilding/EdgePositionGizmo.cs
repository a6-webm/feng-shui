using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Edge))]
[ExecuteAlways]
public class EdgePositionGizmo : MonoBehaviour
{
    private Edge _edge;

    void Start() {
        if (Application.isPlaying) {
            enabled = false;
        }
    }

    void Update()
    {
        if (_edge == null) {
            _edge = GetComponent<Edge>();
        } else {
            List<Vector3> points = _edge.points();
            if (points != null) {
                transform.position = (points[0] + points[1]) / 2;
            }
        }
    }
}
