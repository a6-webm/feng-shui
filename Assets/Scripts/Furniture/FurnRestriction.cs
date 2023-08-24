using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FurnRestriction : MonoBehaviour {
    protected Furniture _furniture;

    protected void Start() {
        _furniture = GetComponent<Furniture>();
        _furniture.registerRestriction(this);
    }

    private void FixedUpdate() {
        if (!_furniture.isLocked(this) && lockingCondition()) {
            _furniture.furnLock(this);
        } else if (_furniture.isLocked(this) && !lockingCondition()) {
            _furniture.furnUnlock(this);
        }
    }

    protected abstract bool lockingCondition();
}
