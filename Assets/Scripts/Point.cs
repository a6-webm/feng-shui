using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Point : MonoBehaviour
{
    public Vector3 p() {
        return transform.position;
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.white;
        Gizmos.DrawSphere(transform.position, 1);
    }
}
