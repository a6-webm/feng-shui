using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Furniture : MonoBehaviour
{
    [Serializable]
    public struct Restrictions {
        public bool dummyRestriction;
        public float dummyRestrictionXValue;
    }
    [SerializeField] public Restrictions restrictions;
    public bool locked { get; private set; } = false;
    private static event Action lockedEvent;
    private Rigidbody rigidbody;
    
    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.solverIterations = 24;
        rigidbody.isKinematic = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (restrictions.dummyRestriction) {
            if (transform.position.x < restrictions.dummyRestrictionXValue) {
                lockSelf();
            } else {
                unlockSelf();
            }
        } else {
            unlockSelf();
        }
    }

    private void lockSelf() {
        locked = true;
        lockedEvent?.Invoke();
    }

    private void unlockSelf() {
        locked = false;
    }

    public bool selected(Action f) {
        if (locked) { return false; }
        lockedEvent += f;
        rigidbody.isKinematic = false;
        return true;
    }

    public void unselected() {
        rigidbody.isKinematic = true;
        lockedEvent = null;
    }
}
