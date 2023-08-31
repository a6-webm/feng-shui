using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Furniture))]
public class Elemental : FurnRestriction
{
    [Serializable] public enum Elem { Fire, Earth, Metal, Water, Wood }
    [SerializeField] Elem Element;
    [SerializeField] float Range = 10f;
    [SerializeField] Vector3 CentreOffset;
    private FurnIcons _furnIcons;
    private const float HEIGHT = 20f;
    private int _unlockingElems = 0;
    private int _lockingElems = 0;
    new void Start()
    {
        base.Start();
        _furnIcons = GetComponent<Furniture>().FurnIcons;
    
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

    void Update() {
        _furnIcons.SetSurroundingElementals(_lockingElems, _unlockingElems, Element);
    }

    override protected bool lockingCondition() {
        return _lockingElems > _unlockingElems;
    }

    void OnTriggerEnter(Collider other) {
        if (other.tag == "Element") {
            var elemental = other.GetComponentInParent<Elemental>();
            if (createsElem(elemental.Element) == Element) {
                _unlockingElems++;
            } else if (destroysElem(elemental.Element) == Element) {
                _lockingElems++;
            }
        }
    }

    void OnTriggerExit(Collider other) {
        if (other.tag == "Element") {
            var elemental = other.GetComponentInParent<Elemental>();
            if (createsElem(elemental.Element) == Element) {
                _unlockingElems--;
            } else if (destroysElem(elemental.Element) == Element) {
                _lockingElems--;
            }
        }
    }

    public static Elem createdByElem(Elem elem) {
        return (Elem)(((int)elem + 4) % 5);
    }

    public static Elem createsElem(Elem elem) {
        return (Elem)(((int)elem + 1) % 5);
    }

    public static Elem destroyedByElem(Elem elem) {
        return (Elem)(((int)elem + 3) % 5);
    }

    public static Elem destroysElem(Elem elem) {
        return (Elem)(((int)elem + 2) % 5);
    }
}
