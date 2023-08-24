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
    private Dictionary<FurnRestriction, bool> Restrictions = new();
    private Action _lockedEvent;
    private Rigidbody _rigidbody;
    private NavMeshObstacle _navMeshObs;
    
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.solverIterations = 24;
        _rigidbody.isKinematic = true;
        if (GetComponent<ChiPhobic>() == null) {
            _navMeshObs = gameObject.AddComponent<NavMeshObstacle>();
            _navMeshObs.carvingTimeToStationary = 0f;
            _navMeshObs.carving = true;
        }
        unselected();
    }

    public void registerRestriction(FurnRestriction restriction, bool locked = false) {
        Restrictions[restriction] = locked;
    }

    public bool isLocked(FurnRestriction restriction) {
        return Restrictions[restriction];
    }

    public void furnLock(FurnRestriction restriction) {
        Restrictions[restriction] = true;
        if (!Locked) {
            Locked = true;
            _lockedEvent?.Invoke();
        }
    }

    public void furnUnlock(FurnRestriction restriction) {
        Restrictions[restriction] = false;
        foreach (bool restrLocked in Restrictions.Values) {
            if (restrLocked) { return;}
        }
        Locked = false;
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
