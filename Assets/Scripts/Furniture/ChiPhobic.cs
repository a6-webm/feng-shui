using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChiPhobic : FurnRestriction
{
    private Furniture _furniture;
    private List<Collider> _chiColliders = new();

    void Start() {
        _furniture = GetComponent<Furniture>();
    }

    void OnTriggerEnter(Collider other) {
        if (other.tag == "Chi") {
            if (_chiColliders.Count == 0) {
                _furniture.furnLock(this);
            }
            _chiColliders.Add(other);
        }
    }

    void OnTriggerExit(Collider other) {
        if (other.tag == "Chi") {
            _chiColliders.Remove(other);
            if (_chiColliders.Count == 0) {
                _furniture.furnUnlock(this);
            }
        }
    }
}
