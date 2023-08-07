using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.AI.NavMeshPathStatus;

[RequireComponent(typeof(Edge))]
public class MovementChi : MonoBehaviour
{
    [SerializeField] float LineSmoothness = 1.3f;
    [SerializeField] float LineResolution = 3f;

    private Edge _edge;
    private NavMeshPath _navMeshPath;
    private List<Vector3> _smoothLine;
    private LineRenderer _lineRenderer;

    void Start()
    {
        _edge = GetComponent<Edge>();
        // _lineRenderer = GetComponent<LineRenderer>();
    }

    void FixedUpdate() {
        Vector3 ptA = _edge.points()[0];
        Vector3 ptB = _edge.points()[1];
        _navMeshPath = new();
        NavMesh.CalculatePath(ptA, ptB, NavMesh.AllAreas, _navMeshPath);
        _smoothLine = smoothLine(_navMeshPath.corners);
    }

    private void OnDrawGizmos() {
        List<Vector3> points = GetComponent<Edge>().points();
        if (_navMeshPath != null && _smoothLine != null) {
            Gizmos.color = _navMeshPath.status == PathComplete ? Color.yellow : Color.red;
            Gizmos.DrawLineStrip(_smoothLine.ToArray(), false);
            _smoothLine.ForEach(p => Gizmos.DrawSphere(p, 0.1f));
            Gizmos.color = Color.red;
            Gizmos.DrawLineStrip(_navMeshPath.corners, false);
            foreach (var p in _navMeshPath.corners) { Gizmos.DrawSphere(p, 0.2f); }
        }
        if (points != null) {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(points[0], points[1]);
        }
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(transform.position, 0.5f);
    }

    private List<Vector3> smoothLine(Vector3[] knots) {
        if (knots.Length <= 2) {
            return new List<Vector3>(knots);
        }
        List<Vector3> smoothLine = new();
        Vector3 outCtrl = (knots[1] - knots[0]).normalized * LineSmoothness;
        for (int i = 0; i < knots.Length - 2; i++) {
            Vector3 pt = knots[i];
            Vector3 nextPt = knots[i+1];
            Vector3 nextNextPt = knots[i+2];

            var a = pt - nextPt;
            var b = nextNextPt - nextPt;
            Vector3 nextInCtrl = ((pt - nextPt).normalized - (nextNextPt - nextPt).normalized).normalized
                * LineSmoothness;

            addCurve(
                ref smoothLine,
                pt,
                pt + outCtrl,
                nextPt + nextInCtrl,
                nextPt
            );

            outCtrl = -nextInCtrl;
        }
        addCurve(
            ref smoothLine,
            knots[^2],
            knots[^2] + outCtrl,
            knots[^1] + (knots[^2] - knots[^1]).normalized * LineSmoothness,
            knots[^1]
        );
        smoothLine.Add(knots[^1]);
        return smoothLine;
    }

    private void addCurve(ref List<Vector3> smoothLine, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3) {
        int subDivisions = (int)(LineResolution * (p0 - p3).magnitude);
        for (int sub = 0; sub < subDivisions; sub++) {
            smoothLine.Add(cubicLerp(
                sub * (1f/subDivisions), // TODO do we want this to have more points at the start and end?
                p0, p1, p2, p3
            ));
        }
    }

    private Vector3 cubicLerp(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3) {
        return p0 * (  -Mathf.Pow(t, 3) + 3*Mathf.Pow(t, 2) - 3*t + 1)
             + p1 * ( 3*Mathf.Pow(t, 3) - 6*Mathf.Pow(t, 2) + 3*t)
             + p2 * (-3*Mathf.Pow(t, 3) + 3*Mathf.Pow(t, 2))
             + p3 *     Mathf.Pow(t, 3);
    }
}
