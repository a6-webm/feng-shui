using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class Edge : MonoBehaviour
{
    [SerializeField]
    Point pointA;
    [SerializeField]
    Point pointB;

    void Start()
    {
        transform.position = (pointA.p() + pointB.p()) / 2;
    }

    public List<Vector3> points() {
        return new List<Vector3> {pointA.p(), pointB.p()};
    }

    private void OnDrawGizmos() {
        if (pointA != null && pointB != null) {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(pointA.p(), pointB.p());
            Gizmos.DrawSphere(transform.position, 0.5f);
        }
    }
}
