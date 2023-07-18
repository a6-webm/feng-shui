using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Furniture : MonoBehaviour
{
    public bool locked { get; private set; } = false;
    Rigidbody rigidbody;
    bool isSelected;

    [Serializable]
    public struct Restrictions {
        public bool dummyRestriction;
    }

    [SerializeField]
    Restrictions restrictions;
    
    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.isKinematic = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (restrictions.dummyRestriction) {
            if (transform.position.x < -1) {
                locked = true;
            } else {
                locked = false;
            }
        } else {
            locked = false;
        }
    }

    public void selected() {
        if (!locked) {
            isSelected = true;
            rigidbody.isKinematic = false;
        }
    }

    public void unselected() {
        isSelected = false;
        rigidbody.isKinematic = true;
    }
}
