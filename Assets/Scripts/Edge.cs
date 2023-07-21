using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EdgePositionGizmo))]
public class Edge : MonoBehaviour
{
    [SerializeField] Point PointA;
    [SerializeField] Point PointB;

    private void Start() {
        transform.position = (PointA.p() + PointB.p()) / 2;
    }

    public List<Vector3> points() {
        if (PointA != null && PointB != null) {
            return new List<Vector3> {PointA.p(), PointB.p()};
        }
        return null;
    }

    private void OnDrawGizmos() {
        if (PointA != null && PointB != null) {
            Gizmos.color = Color.black;
            Gizmos.DrawLine(PointA.p(), PointB.p());
        }
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(transform.position, 0.5f);
    }
}
