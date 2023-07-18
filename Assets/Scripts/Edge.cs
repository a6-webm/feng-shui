using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EdgePositionGizmo))]
public class Edge : MonoBehaviour
{
    [SerializeField]
    Point pointA;
    [SerializeField]
    Point pointB;

    private void Start() {
        transform.position = (pointA.p() + pointB.p()) / 2;
    }

    public List<Vector3> points() {
        if (pointA != null && pointB != null) {
            return new List<Vector3> {pointA.p(), pointB.p()};
        }
        return null;
    }

    private void OnDrawGizmos() {
        if (pointA != null && pointB != null) {
            Gizmos.color = Color.black;
            Gizmos.DrawLine(pointA.p(), pointB.p());
        }
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(transform.position, 0.5f);
    }
}
