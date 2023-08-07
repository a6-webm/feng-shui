using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody), typeof(NavMeshObstacle))]
public class Furniture : MonoBehaviour
{
    [Serializable]
    struct Restricters {
        public bool dummyRestriction;
        public float dummyRestrictionXValue;
    }
    [SerializeField] Restricters Restrictions;
    public bool Locked { get; private set; } = false;
    private static event Action _lockedEvent;
    private Rigidbody _rigidbody;
    private NavMeshObstacle _navMeshObs;
    
    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.solverIterations = 24;
        _rigidbody.isKinematic = true;
        _navMeshObs = GetComponent<NavMeshObstacle>();
        _navMeshObs.carvingTimeToStationary = 0f;
        _navMeshObs.carving = true;
        unselected();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Restrictions.dummyRestriction) {
            if (transform.position.x < Restrictions.dummyRestrictionXValue) {
                lockSelf();
            } else {
                unlockSelf();
            }
        } else {
            unlockSelf();
        }
    }

    private void lockSelf() {
        Locked = true;
        _lockedEvent?.Invoke();
    }

    private void unlockSelf() {
        Locked = false;
    }

    public bool selected(Action f) {
        if (Locked) { return false; }
        _lockedEvent += f;
        _rigidbody.isKinematic = false;
        _navMeshObs.carveOnlyStationary = false;
        return true;
    }

    public void unselected() {
        _rigidbody.isKinematic = true;
        _lockedEvent = null;
        _navMeshObs.carveOnlyStationary = true;
    }
}
