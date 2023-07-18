using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(QuadPositionGizmo))]
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
        if (pointA != null && pointB != null && pointC != null && pointD != null) {
            return new List<Vector3> {pointA.p(), pointB.p(), pointC.p(), pointD.p()};
        }
        return null;
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, 0.5f);
        if (pointA != null && pointB != null && pointC != null && pointD != null) {
            // Gizmos.DrawLineStrip(new Vector3[] {pointA.p(), pointB.p(), pointC.p(), pointD.p()}, true);
            Gizmos.DrawLine(pointA.p(), transform.position);
            Gizmos.DrawLine(pointB.p(), transform.position);
            Gizmos.DrawLine(pointC.p(), transform.position);
            Gizmos.DrawLine(pointD.p(), transform.position);
        }
    }
}
