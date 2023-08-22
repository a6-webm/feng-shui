using System;
using UnityEngine;

public class TurtleCollider : MonoBehaviour
{
    private Turtle _turtle;

    void Awake() {
        _turtle = transform.parent.GetComponent<Turtle>();
    }

    private void OnTriggerEnter(Collider other) {
        _turtle.onTurtleTriggerEnter(other);
    }

    private void OnTriggerExit(Collider other) {
        _turtle.onTurtleTriggerExit(other);
    }
}
