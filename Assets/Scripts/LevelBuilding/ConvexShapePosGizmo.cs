using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ConvexShape))]
[ExecuteAlways]
public class ConvexShapePosGizmo : MonoBehaviour
{
    private ConvexShape _convexShape;

    void Start() {
        if (Application.isPlaying) {
            enabled = false;
        }
    }

    void Update()
    {
        if (_convexShape == null) {
            _convexShape = GetComponent<ConvexShape>();
        } else {
            List<Vector3> points = _convexShape.points();
            if (points.Count < 3) { return; }
            Vector3 sumPos = new Vector3(0,0,0);
            points.ForEach(p => sumPos += p);
            transform.position = (sumPos) / points.Count;
        }
    }
}
