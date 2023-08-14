using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ConvexShapePosGizmo))]
public class ConvexShape : MonoBehaviour
{
    [SerializeField] List<Point> Points;

    void Start()
    {
        Vector3 sumPos = new Vector3(0,0,0);
        points().ForEach(p => sumPos += p);
        transform.position = (sumPos) / Points.Count;
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, 0.5f);
        if (Points != null && Points.Count > 0) {
            points().ForEach(p => Gizmos.DrawLine(p, transform.position));
        }
    }

    public List<Vector3> points() {
        List<Vector3> outList = new List<Vector3>();
        if (Points == null) { return new List<Vector3>(); }
        foreach (Point p in Points) {
            if (p == null) { return outList; }
            outList.Add(p.transform.position);
        }
        return outList;
    }
}
