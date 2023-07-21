using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ConvexShapePosGizmo))]
public class ConvexShape : MonoBehaviour
{
    [SerializeField] List<Point> _points;

    void Start()
    {
        Vector3 sumPos = new Vector3(0,0,0);
        points().ForEach(p => sumPos += p);
        transform.position = (sumPos) / _points.Count;
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, 0.5f);
        if (_points != null && _points.Count > 0) {
            points().ForEach(p => Gizmos.DrawLine(p, transform.position));
        }
    }

    public List<Vector3> points() {
        List<Vector3> outList = new List<Vector3>();
        if (_points == null) { return new List<Vector3>(); }
        foreach (Point p in _points) {
            if (p == null) { return outList; }
            outList.Add(p.transform.position);
        }
        return outList;
    }
}
