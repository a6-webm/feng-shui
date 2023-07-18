using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Quad))]
[ExecuteAlways]
public class QuadPositionGizmo : MonoBehaviour
{
    Quad quad;

    void Start() {
        if (Application.isPlaying) {
            enabled = false;
        }
    }

    void Update()
    {
        if (quad == null) {
            quad = GetComponent<Quad>();
        } else {
            List<Vector3> points = quad.points();
            if (points != null) {
                Vector3 sum = new Vector3();
                points.ForEach(p => sum += p);
                transform.position = sum / 4;
            }
        }
    }
}
