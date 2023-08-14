using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

[RequireComponent(typeof(Rigidbody))]
public class Furniture : MonoBehaviour
{
    public bool Locked { get; private set; } = false;
    private Action _lockedEvent;
    private Rigidbody _rigidbody;
    private NavMeshObstacle _navMeshObs;
    private List<FurnRestriction> _furnRestrictions = new();
    
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.solverIterations = 24;
        _rigidbody.isKinematic = true;
        if ((_navMeshObs = GetComponent<NavMeshObstacle>()) != null) {
            _navMeshObs.carvingTimeToStationary = 0f;
            _navMeshObs.carving = true;
        }
        unselected();
    }

    public void furnLock(FurnRestriction furnRestriction) {
        if (_furnRestrictions.Count == 0) {
            Locked = true;
            _lockedEvent?.Invoke();
        }
        _furnRestrictions.Add(furnRestriction);
    }

    public void furnUnlock(FurnRestriction furnRestriction) {
        _furnRestrictions.Remove(furnRestriction);
        if (_furnRestrictions.Count == 0) {
            Locked = false;
        }
    }

    public bool selected(Action f) {
        if (Locked) { return false; }
        _lockedEvent += f;
        _rigidbody.isKinematic = false;
        if (_navMeshObs != null) { _navMeshObs.carveOnlyStationary = false; }
        return true;
    }

    public void unselected() {
        _rigidbody.isKinematic = true;
        _lockedEvent = null;
        if (_navMeshObs != null) { _navMeshObs.carveOnlyStationary = true; }
    }
}
