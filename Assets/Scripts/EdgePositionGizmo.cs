using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class EdgePositionGizmo : MonoBehaviour
{
    Edge edge;

    void Start() {
        if (Application.isPlaying) {
            enabled = false;
        }
    }

    void Update()
    {
        if (edge == null) {
            edge = GetComponent<Edge>();
        } else {
            transform.position = (edge.points()[0] + edge.points()[1]) / 2;
            transform.rotation = Quaternion.LookRotation(edge.points()[0] - edge.points()[1]);
        }
    }
}
