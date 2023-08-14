using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RigidbodyConstraints;

[RequireComponent(typeof(Edge), typeof(LineRenderer))]
public class Door : MonoBehaviour
{
    [SerializeField] float DoorWidth = 0.1f;
    [SerializeField] float MaxAngle = 90f;
    [SerializeField] bool Flip = false;

    private const float DOOR_HEIGHT = 10f;
    private const float DOOR_RESOLUTION = 1f;
    private Edge _edge;
    private LineRenderer _lineRenderer;
    private GameObject _door;
    private HingeJoint _joint;

    void Start() {
        _edge = GetComponent<Edge>();
        _lineRenderer = GetComponent<LineRenderer>();
        var baseRb = gameObject.AddComponent<Rigidbody>();
        _door = new GameObject{ name = "doorCollider" };
        var rb = _door.AddComponent<Rigidbody>();
        _joint = _door.AddComponent<HingeJoint>();
        var collider = _door.AddComponent<BoxCollider>();
        
        baseRb.isKinematic = true;

        transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        _lineRenderer.numCapVertices = 5;
        _lineRenderer.positionCount = 2;
        _lineRenderer.alignment = LineAlignment.TransformZ;
        
        _door.layer = 7; // Doors layer
        _door.transform.SetLocalPositionAndRotation(
            transform.position,
            Quaternion.LookRotation(_edge.pointA() - _edge.pointB())
        );
        collider.size = new Vector3(DoorWidth, DOOR_HEIGHT, _edge.length());
        rb.angularDrag = 20;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        rb.useGravity = false;
        _joint.autoConfigureConnectedAnchor = false;
        _joint.axis = Vector3.up;
        _joint.connectedAnchor = Flip ? _edge.pointB() : _edge.pointA();
        _joint.anchor = new Vector3(0, 0, (Flip?-1:1) * _edge.length()/2);
        _joint.limits = new JointLimits() {
            min = Flip ? 0 : -MaxAngle,
            max = Flip ? MaxAngle : 0,
            bounciness = 0,
            bounceMinVelocity = 0
        };
        _joint.useLimits = true;
    }

    void FixedUpdate() {
        var line = genDoorLine(
            Quaternion.LookRotation(_edge.pointA() - _edge.pointB()).eulerAngles.y,
            _joint.angle, _edge, Flip
        );
        _lineRenderer.positionCount = line.Count;
        _lineRenderer.SetPositions(line.ToArray());
    }

    private List<Vector3> genDoorLine(float restAng, float jointAng, Edge edge, bool flip) {
        Vector3 hingePt = Flip ? edge.pointB() : edge.pointA();
        List<Vector3> line = drawArc(
            restAng + (flip?-1:1) * 90f,
            -jointAng,
            edge.length(),
            DOOR_RESOLUTION
        );
        line = line.ConvertAll(p => p + hingePt);
        line.Add(hingePt);
        return line;
    }

    private List<Vector3> drawArc(float startAng, float deltaAng, float r, float resolution) {
        List<Vector3> line = new();
        int subDivisions = (int)(resolution * 2*Mathf.PI * r * (Mathf.Abs(deltaAng) / 360));
        line.Add(pointOnFlatCircle(startAng, r));
        for (int sub = 1; sub < subDivisions; sub++) {
            float a = deltaAng * sub*1f/subDivisions + startAng;
            line.Add(pointOnFlatCircle(a, r));
        }
        line.Add(pointOnFlatCircle(startAng + deltaAng, r));
        return line;
    }

    private Vector3 pointOnFlatCircle(float ang, float r) {
        ang *= Mathf.Deg2Rad;
        return new Vector3(Mathf.Cos(ang), 0, Mathf.Sin(ang)) * r;
    }

    void OnDrawGizmos() {
        Edge edge = GetComponent<Edge>();
        if (edge.points() != null) {
            Gizmos.color = Color.black;
            var line = genDoorLine(
                Quaternion.LookRotation(edge.pointA() - edge.pointB()).eulerAngles.y,
                (Flip?1:-1) * MaxAngle,
                edge,
                Flip
            );
            Gizmos.DrawLineStrip(line.ToArray(), false);
        }
        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(transform.position, 0.5f);
    }
}
