using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Edge))]
public class MovementChi : MonoBehaviour
{
    private Edge _edge;

    void Start()
    {
        _edge = GetComponent<Edge>();
    }

    void Update()
    {
        
    }

    private void OnDrawGizmos() {
        List<Vector3> points = GetComponent<Edge>().points();
        if (points != null) {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(points[0], points[1]);
        }
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(transform.position, 0.5f);
    }
}
