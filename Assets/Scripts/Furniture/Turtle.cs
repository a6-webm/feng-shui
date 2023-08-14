using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Furniture))]
public class Turtle : FurnRestriction
{
    public Vector3 BackPos;
    public Vector3 BackScale = new Vector3(1, 1, 1);

    private Furniture _furniture;
    private GameObject _collliderObj;
    private List<Collider> _turtlingColliders = new();
    private int _turtleLayer;
    private Rigidbody _rigidbody;

    void Start() {
        _rigidbody = GetComponent<Rigidbody>();
        _furniture = GetComponent<Furniture>();
        _collliderObj = new GameObject{ name = "turtleCollider" };
        _collliderObj.transform.SetParent(transform);
        _collliderObj.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        var turtCol = _collliderObj.AddComponent<TurtleCollider>();
        var turtleRB = _collliderObj.AddComponent<Rigidbody>();
        turtleRB.isKinematic = true;
        turtleRB.useGravity = false;
        var col = _collliderObj.AddComponent<BoxCollider>();
        col.isTrigger = true;
        col.center = BackPos;
        col.size = BackScale;
        _turtleLayer = LayerMask.GetMask("Turtleable", "Walls");
        col.excludeLayers = ~_turtleLayer;
        col.includeLayers = _turtleLayer;
        turtCol.triggerEnterAction = onTurtleTriggerEnter;
        turtCol.triggerExitAction = onTurtleTriggerExit;
    }

    private void onTurtleTriggerEnter(Collider other) {
        Debug.Log("turtle entered: " + other.gameObject.name);
        Debug.Log("isKinematic: " + _rigidbody.isKinematic);
        if (((1 << other.gameObject.layer) & _turtleLayer) != 0) {
            if (_turtlingColliders.Count == 0) {
                _furniture.furnUnlock(this);
            }
            _turtlingColliders.Add(other);
        }
    }

    void onTurtleTriggerExit(Collider other) {
        Debug.Log("turtle exited: " + other.gameObject.name);
        Debug.Log("isKinematic: " + _rigidbody.isKinematic);
        if (((1 << other.gameObject.layer) & _turtleLayer) != 0) {
            _turtlingColliders.Remove(other);
            if (_turtlingColliders.Count == 0) {
                _furniture.furnLock(this);
            }
        }
    }
}
