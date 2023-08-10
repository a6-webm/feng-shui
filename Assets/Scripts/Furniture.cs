using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody))]
public class Furniture : MonoBehaviour
{
    [Serializable]
    struct Restricters {
        public bool turtle;
        public Vector3 turtleBackPos;
        public bool chiPhobia;
    }
    [SerializeField] Restricters Restrictions;
    public bool Locked { get; private set; } = false;
    private Action _lockedEvent;
    private Rigidbody _rigidbody;
    private NavMeshObstacle _navMeshObs;
    private BoxCollider _turtleCol;
    private List<Collider> _turtlingColliders;
    private const int TURTLEABLE = 6;
    private const int WALLS = 8;
    private const int TURTLE_LAYER = 1 << TURTLEABLE | 1 << WALLS;
    private const int CHI = 9;
    private List<Collider> _chiTriggers;
    
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.solverIterations = 24;
        _rigidbody.isKinematic = true;
        if ((_navMeshObs = GetComponent<NavMeshObstacle>()) != null) {
            _navMeshObs.carvingTimeToStationary = 0f;
            _navMeshObs.carving = true;
        }
        if (Restrictions.turtle) {
            _turtleCol = gameObject.AddComponent<BoxCollider>();
            _turtleCol.center = Restrictions.turtleBackPos;
            _turtleCol.size = Vector3.one * 0.1f;
            _turtleCol.isTrigger = true;
            _turtleCol.excludeLayers = ~TURTLE_LAYER;
            _turtleCol.includeLayers = TURTLE_LAYER;
        }
        unselected();
    }

    void OnTriggerEnter(Collider other) {
        if (other.gameObject.layer == CHI && Restrictions.chiPhobia) {
            if (_chiTriggers.Count == 0) {
                lockSelf();
            }
            _chiTriggers.Add(other);
        } else if (((1 << other.gameObject.layer) & TURTLE_LAYER) != 0 && Restrictions.turtle) {
            if (_turtlingColliders.Count == 0) {
                lockSelf();
            }
            _turtlingColliders.Add(other);
        }
    }

    void OnTriggerExit(Collider other) {
        if (other.gameObject.layer == CHI && Restrictions.chiPhobia) {
            _chiTriggers.Remove(other);
            if (_chiTriggers.Count == 0) {
                unlockSelf();
            }
        } else if (((1 << other.gameObject.layer) & TURTLE_LAYER) != 0 && Restrictions.turtle) {
            _turtlingColliders.Remove(other);
            if (_turtlingColliders.Count == 0) {
                unlockSelf();
            }
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
        if (_navMeshObs != null) { _navMeshObs.carveOnlyStationary = false; }
        return true;
    }

    public void unselected() {
        _rigidbody.isKinematic = true;
        _lockedEvent = null;
        if (_navMeshObs != null) { _navMeshObs.carveOnlyStationary = true; }
    }
}
