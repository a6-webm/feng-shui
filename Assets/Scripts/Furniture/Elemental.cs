using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Furniture))]
public class Elemental : FurnRestriction
{
    [Serializable] enum Elem { Fire, Earth, Metal, Water, Wood }
    [SerializeField] Elem Element;
    [SerializeField] float Range = 10f;
    [SerializeField] Vector3 CentreOffset;
    private const float HEIGHT = 20f;
    private int _unlockingElems = 0;
    private int _lockingElems = 0;
    new void Start()
    {
        base.Start();
        var collliderObj = new GameObject{ name = "elementCollider" };
        collliderObj.transform.SetParent(transform);
        collliderObj.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        collliderObj.layer = LayerMask.NameToLayer("Element");

        var col = collliderObj.AddComponent<CapsuleCollider>();
        col.tag = "Element";
        col.isTrigger = true;
        col.center = CentreOffset;
        var scaleX = 2*Range / transform.localScale.x;
        var scaleY = (2*Range + HEIGHT) / transform.localScale.y;
        var scaleZ = 2*Range / transform.localScale.z;
        collliderObj.transform.localScale = new Vector3(scaleX, scaleY, scaleZ);
        int colLayer = LayerMask.GetMask("Default", "Turtleable");
        col.excludeLayers = ~colLayer;
        col.includeLayers = colLayer;
    }

    override protected bool lockingCondition() {
        return _lockingElems > _unlockingElems;
    }

    void OnTriggerEnter(Collider other) {
        if (other.tag == "Element") {
            var elemental = other.GetComponentInParent<Elemental>();
            if (creates(elemental.Element) == Element) {
                _unlockingElems++;
            } else if (destroys(elemental.Element) == Element) {
                _lockingElems++;
            }
        }
    }

    void OnTriggerExit(Collider other) {
        if (other.tag == "Element") {
            var elemental = other.GetComponentInParent<Elemental>();
            if (creates(elemental.Element) == Element) {
                _unlockingElems--;
            } else if (destroys(elemental.Element) == Element) {
                _lockingElems--;
            }
        }
    }

    private Elem creates(Elem elem) {
        return (Elem)(((int)elem + 1) % 5);
    }

    private Elem destroys(Elem elem) {
        return (Elem)(((int)elem + 2) % 5);
    }
}
