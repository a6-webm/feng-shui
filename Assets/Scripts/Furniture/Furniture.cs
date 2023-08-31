using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static Player;
using static Player.PlayerFurnState;

[RequireComponent(typeof(Rigidbody))]
public class Furniture : MonoBehaviour
{
    [SerializeField] GameObject IconsPrefab;
    public bool Locked { get; private set; } = false;
    public FurnIcons FurnIcons;
    private Dictionary<FurnRestriction, bool> _restrictions = new();
    private Rigidbody _rigidbody;
    private NavMeshObstacle _navMeshObs;
    private PlayerFurnState _state;
    
    void Start() {
        FurnIcons = Instantiate(IconsPrefab).GetComponent<FurnIcons>();
        FurnIcons.Furn = transform;
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.solverIterations = 24;
        _rigidbody.isKinematic = true;
        if (GetComponent<ChiPhobic>() == null) {
            _navMeshObs = gameObject.AddComponent<NavMeshObstacle>();
            _navMeshObs.carving = true;
            _navMeshObs.carveOnlyStationary = false;
        }
        setState(None);
    }

    public void registerRestriction(FurnRestriction restriction, bool locked = false) {
        _restrictions[restriction] = locked;
    }

    public bool isLocked(FurnRestriction restriction) {
        return _restrictions[restriction];
    }

    public void furnLock(FurnRestriction restriction) {
        _restrictions[restriction] = true;
        Locked = true;
        FurnIcons.showLock();
        _rigidbody.isKinematic = true;
    }

    public void furnUnlock(FurnRestriction restriction) {
        _restrictions[restriction] = false;
        foreach (bool restrLocked in _restrictions.Values) {
            if (restrLocked) { return;}
        }
        Locked = false;
        FurnIcons.hideLock();
        if (_state == Selected) _rigidbody.isKinematic = false;
    }

    public void setState(PlayerFurnState state) {
        if (state == Selected) {
            if (!Locked) _rigidbody.isKinematic = false;
            FurnIcons.hovered();
        } else if (state == Hovered) {
            FurnIcons.hovered();
        } else {
            _rigidbody.isKinematic = true;
            FurnIcons.unhovered();
        }
        _state = state;
    }
}
