using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.AI.NavMeshPathStatus;

[RequireComponent(typeof(Edge))]
public class MovementChi : MonoBehaviour
{
    [SerializeField] float LineSmoothness = 2.18f;
    [SerializeField] float LineSmoothnessScaling = 0.25f;
    [SerializeField] float LineResolution = 3f;
    [SerializeField] float ColliderResolution = 0.4f;
    [SerializeField] float ColliderWidth = 4f;
    private const float LINEHEIGHT = 5f;
    private Edge _edge;
    private NavMeshPath _navMeshPath;
    private LineRenderer _lineRenderer;
    private List<GameObject> _boxColObjs = new();
    private List<GameObject> _capsuleColObjs = new();

    void Start()
    {
        FindAnyObjectByType<Player>().DeselectEvent += newChiLine;
        gameObject.layer = LayerMask.NameToLayer("Chi");
        _edge = GetComponent<Edge>();
        var lineObj = transform.Find("ChiLine");
        _lineRenderer = lineObj.GetComponent<LineRenderer>();
        _lineRenderer.widthMultiplier = ColliderWidth;
        newChiLine();
    }

    private void newChiLine(GameObject furniture = null) {
        if (furniture != null && furniture.GetComponent<ChiPhobic>() != null) return;
        _navMeshPath = new();
        NavMesh.CalculatePath(_edge.pointA(), _edge.pointB(), NavMesh.AllAreas, _navMeshPath);
        var smooth = smoothLine(_navMeshPath.corners, LineResolution);
        _lineRenderer.positionCount = smooth.Count;
        _lineRenderer.SetPositions(smooth.ToArray());
        handleColliders(smoothLine(_navMeshPath.corners, ColliderResolution));
    }

    private void OnDrawGizmos() {
        List<Vector3> points = GetComponent<Edge>().points();
        if (_navMeshPath != null) {
            Gizmos.color = _navMeshPath.status == PathComplete ? Color.yellow : Color.red;
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

    private void handleColliders(List<Vector3> line) {
        var capDiff = line.Count - _capsuleColObjs.Count;
        if (capDiff > 0) {
            for (int i = 0; i < capDiff; i++) {
                GameObject obj = new(){ name = "chiCapsule"};
                obj.transform.SetParent(transform);
                obj.layer = LayerMask.NameToLayer("Chi");
                var col = obj.AddComponent<CapsuleCollider>();
                col.isTrigger = true;
                col.tag = "Chi";
                _capsuleColObjs.Add(obj);
            }
        }
        var boxDiff = line.Count-1 - _boxColObjs.Count;
        if (boxDiff > 0) {
            for (int i = 0; i < boxDiff; i++) {
                GameObject obj = new(){ name = "chiBox"};
                obj.transform.SetParent(transform);
                obj.layer = LayerMask.NameToLayer("Chi");
                var col = obj.AddComponent<BoxCollider>();
                col.isTrigger = true;
                _boxColObjs.Add(obj);
            }
        }
        for (int i = 0; i < line.Count; i++) {
            var obj = _capsuleColObjs[i];
            obj.SetActive(true);
            obj.transform.position = line[i];
            obj.transform.localScale = new Vector3(ColliderWidth, LINEHEIGHT + ColliderWidth, ColliderWidth);
        }
        for (int i = line.Count; i < _capsuleColObjs.Count; i++) {
            _capsuleColObjs[i].SetActive(false);
        }
        for (int i = 0; i < line.Count-1; i++) {
            var obj = _boxColObjs[i];
            obj.SetActive(true);
            var pt = line[i];
            var nextPt = line[i+1];
            obj.transform.position = (nextPt + pt)/2;
            obj.transform.rotation = Quaternion.LookRotation(nextPt - pt, Vector3.up);
            obj.transform.localScale = new Vector3(ColliderWidth, LINEHEIGHT, (nextPt - pt).magnitude);
        }
        for (int i = line.Count-1; i < _boxColObjs.Count; i++) {
            _boxColObjs[i].SetActive(false);
        }
    }

    // private Mesh lineToMesh(List<Vector3> line) {
    //     if (line.Count <= 1) {
    //         return null;
    //     }
    //     float width = 3f;
    //     float height = 3f;
    //     line = line.ConvertAll(p => transform.InverseTransformPoint(p));
    //     List<Vector3> vertices = new();
    //     var firstPerp = Vector3.Cross((line[1] - line[0]).normalized, Vector3.up);
    //     addSquare(line[0], firstPerp, Vector3.up, width, height, vertices);
    //     List<int> triangles = new();
    //     addQuad(0, 1, 2, 3, triangles);
    //     for (int i = 0; i < line.Count-2; i++) {
    //         var pt = line[i];
    //         var nextPt = line[i+1];
    //         var nextNextPt = line[i+2];

    //         var a = pt - nextPt;
    //         var b = nextNextPt - nextPt;
    //         Vector3 nextPerp = Vector3.Cross((b.normalized - a.normalized).normalized, Vector3.up);
    //         addSquare(nextPt, nextPerp, Vector3.up, width, height, vertices);

    //         addTrunk(i * 4, triangles); // i * 4 because 4 points are added per loop
    //     }
    //     var lastPerp = Vector3.Cross((line[^1] - line[^2]).normalized, Vector3.up);
    //     addSquare(line[^1], lastPerp, Vector3.up, width, height, vertices);
    //     int v = (line.Count-2) * 4;
    //     addTrunk(v, triangles);
    //     addQuad(v+4, v+7, v+6, v+5, triangles);

    //     return new Mesh(){
    //         vertices = vertices.ToArray(),
    //         triangles = triangles.ToArray(),
    //     };
    // }

    // private void addSquare(Vector3 centre, Vector3 rightDir, Vector3 upDir, float width, float height, List<Vector3> vertices) {
    //     vertices.Add(centre - rightDir * width/2 + upDir * height/2);
    //     vertices.Add(centre + rightDir * width/2 + upDir * height/2);
    //     vertices.Add(centre + rightDir * width/2 - upDir * height/2);
    //     vertices.Add(centre - rightDir * width/2 - upDir * height/2);
    // }

    // private void addTrunk(int v, List<int> triangles) {
    //     addQuad(v+4, v+5, v+1, v+0, triangles);
    //     addQuad(v+5, v+6, v+2, v+1, triangles);
    //     addQuad(v+6, v+7, v+3, v+2, triangles);
    //     addQuad(v+7, v+4, v+0, v+3, triangles);
    // }

    // private void addQuad(int v0, int v1, int v2, int v3, List<int> triangles) {
    //     triangles.Add(v0); triangles.Add(v2); triangles.Add(v1);
    //     triangles.Add(v0); triangles.Add(v3); triangles.Add(v2);
    // }

    private List<Vector3> smoothLine(Vector3[] knots, float resolution) {
        if (knots.Length <= 2) {
            return new List<Vector3>(knots);
        }
        List<Vector3> smoothLine = new();
        Vector3 outCtrl = (knots[1] - knots[0]).normalized;
        for (int i = 0; i < knots.Length - 2; i++) {
            Vector3 pt = knots[i];
            Vector3 nextPt = knots[i+1];
            Vector3 nextNextPt = knots[i+2];

            var a = pt - nextPt;
            var b = nextNextPt - nextPt;
            Vector3 nextInCtrl = (a.normalized - b.normalized).normalized;

            addCurve(
                smoothLine,
                pt,
                pt + outCtrl * sigmoidAt0(a.magnitude, LineSmoothness, LineSmoothnessScaling),
                nextPt + nextInCtrl * sigmoidAt0(a.magnitude, LineSmoothness, LineSmoothnessScaling),
                nextPt,
                resolution
            );

            outCtrl = -nextInCtrl;
        }
        addCurve(
            smoothLine,
            knots[^2],
            knots[^2] + outCtrl * sigmoidAt0((knots[^2] - knots[^1]).magnitude, LineSmoothness, LineSmoothnessScaling),
            knots[^1] + (knots[^2] - knots[^1]).normalized * sigmoidAt0((knots[^2] - knots[^1]).magnitude, LineSmoothness, LineSmoothnessScaling),
            knots[^1],
            resolution
        );
        smoothLine.Add(knots[^1]);
        return smoothLine;
    }

    private void addCurve(List<Vector3> smoothLine, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float resolution) {
        int subDivisions = (int)(resolution * (p0 - p3).magnitude);
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

    private float sigmoidAt0(float x, float height, float pitch) {
        return height * (2f / (1f + Mathf.Exp(-pitch*x)) - 1f);
    }
}
