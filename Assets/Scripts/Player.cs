using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField]
    InputActionAsset inputActionAsset;
    [SerializeField]
    float dragStrength = 1.0f;
    const float MAX_RAY_DIST = 1000f;

    Rigidbody selectedBody;
    MousePull mousePull = new MousePull();

    // Start is called before the first frame update
    void Start()
    {
        inputActionAsset.Enable();
        InputAction press = inputActionAsset.FindAction("Press");
        press.started += ctx => {
            mousePull.clickDown = true;
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(inputActionAsset.FindAction("Drag").ReadValue<Vector2>());
            if (Physics.Raycast(ray, out hit, MAX_RAY_DIST))
            {
                selectedBody = hit.rigidbody;
                mousePull.relGrabPos = hit.transform.InverseTransformPoint(hit.point);
            }
        };
        press.canceled += ctx => {
            mousePull.clickDown = false;
            selectedBody = null; // TODO once u add multiple fingers, you'll need to check that all fingers are off the screen
        };
    }

    void FixedUpdate() {
        if (selectedBody != null && mousePull.clickDown) {
            Ray ray = Camera.main.ScreenPointToRay(inputActionAsset.FindAction("Drag").ReadValue<Vector2>());
            Vector3 grabPos = selectedBody.transform.TransformPoint(mousePull.relGrabPos);
            Vector3 proj_mouse = rayCastAtYLevel(ray, grabPos.y);
            Vector3 force = (proj_mouse - grabPos).normalized * Mathf.Sqrt((proj_mouse - grabPos).magnitude) * dragStrength;
            selectedBody.AddForceAtPosition(force, grabPos);
        }
    }

    private Vector3 rayCastAtYLevel(Ray ray, float y) {
        Vector3 dir = ray.direction;
        Vector3 o = ray.origin;
        
        float yx_grad = dir.x / dir.y;
        float x = yx_grad*(y - o.y) + o.x;

        float yz_grad = dir.z / dir.y;
        float z = yz_grad*(y - o.y) + o.z;

        return new Vector3(x, y, z);
    }
}

class MousePull
{
    public bool clickDown { get; set; }
    public Vector3 relGrabPos { get; set; }
    public MousePull() {
        clickDown = false;
        relGrabPos = Vector3.zero;
    }
}