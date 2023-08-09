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

    public Vector3 pointA() {
        return PointA.p();
    }

    public Vector3 pointB() {
        return PointB.p();
    }

    public float length() {
        return Vector3.Distance(PointA.p(), PointB.p());
    }
}
