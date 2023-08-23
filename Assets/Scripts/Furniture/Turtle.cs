using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Furniture))]
public class Turtle : FurnRestriction
{
    [SerializeField] Vector3 BackPos;
    [SerializeField] Vector3 BackScale = new Vector3(1, 1, 1);

    private Furniture _furniture;
    private int _turtlingColliders = 0;

    void Start() {
        _furniture = GetComponent<Furniture>();
        var collliderObj = new GameObject{ name = "turtleCollider" };
        collliderObj.transform.SetParent(transform);
        collliderObj.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        collliderObj.AddComponent<TurtleCollider>();
        
        var turtleRB = collliderObj.AddComponent<Rigidbody>(); // To prevent OnTrigger jitter when toggling isKinematic on the parent
        turtleRB.isKinematic = true;
        turtleRB.useGravity = false;

        var col = collliderObj.AddComponent<BoxCollider>();
        col.isTrigger = true;
        col.center = BackPos;
        col.size = BackScale;
        int turtleLayer = LayerMask.GetMask("Turtleable", "Walls");
        col.excludeLayers = ~turtleLayer;
        col.includeLayers = turtleLayer;
    }

    public void onTurtleTriggerEnter(Collider other) {
        if (_turtlingColliders == 0) {
            _furniture.furnUnlock(this);
        }
        _turtlingColliders++;
    }

    public void onTurtleTriggerExit(Collider other) {
        _turtlingColliders--;
        if (_turtlingColliders == 0) {
            _furniture.furnLock(this);
        }
    }
}
