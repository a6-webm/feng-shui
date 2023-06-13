using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quad : MonoBehaviour
{
    [SerializeField]
    Point pointA;
    [SerializeField]
    Point pointB;
    [SerializeField]
    Point pointC;
    [SerializeField]
    Point pointD;

    private void Start() {
        transform.position = (pointA.p() + pointB.p() + pointC.p() + pointD.p()) / 4;
    }

    public List<Vector3> points() {
        return new List<Vector3> {pointA.p(), pointB.p(), pointC.p(), pointD.p()};
    }

    private void OnDrawGizmos() {
        if (pointA != null && pointB != null && pointC != null && pointD != null) {
            Gizmos.color = Color.red;
            // Gizmos.DrawLineStrip(new Vector3[] {pointA.p(), pointB.p(), pointC.p(), pointD.p()}, true);
            Gizmos.DrawLine(pointA.p(), transform.position);
            Gizmos.DrawLine(pointB.p(), transform.position);
            Gizmos.DrawLine(pointC.p(), transform.position);
            Gizmos.DrawLine(pointD.p(), transform.position);
            Gizmos.DrawSphere(transform.position, 0.5f);
        }
    }
}
