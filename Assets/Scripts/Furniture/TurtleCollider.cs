using System;
using UnityEngine;

public class TurtleCollider : MonoBehaviour
{
    public Action<Collider> triggerEnterAction;
    public Action<Collider> triggerExitAction;

    private void OnTriggerEnter(Collider other) {
        triggerEnterAction?.Invoke(other);
    }

    private void OnTriggerExit(Collider other) {
        triggerExitAction?.Invoke(other);
    }
}
