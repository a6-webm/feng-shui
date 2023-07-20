using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using UnityEngine.UI;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] InputActionAsset inputActionAsset;
    [SerializeField] float dragStrength = 100f;
    [SerializeField] float scrollSensitivity = 5f;
    const float MAX_RAY_DIST = 1000f;
    const float LINE_THICKNESS = 5f;

    [SerializeField] GameObject linePrefab;
    GameObject selected;
    MousePull mousePull = new MousePull();
    bool dragging;
    Vector2 prevMouse;
    LevelData levelData;

    GameObject canvas;

    void Start()
    {
        levelData = GameObject.Find("LevelManager").GetComponent<LevelManager>().levelData;
        transform.position = levelData.playerStartPos;
        inputActionAsset.Enable();
        InputAction pressAction = inputActionAsset.FindAction("Press");
        InputAction zoomInAction = inputActionAsset.FindAction("ZoomIn");
        InputAction zoomOutAction = inputActionAsset.FindAction("ZoomOut");
        canvas = GameObject.Find("Canvas");
        
        pressAction.started += ctx => {
            mousePull.clickDown = true;
            RaycastHit hit;
            Vector2 mouse = inputActionAsset.FindAction("Drag").ReadValue<Vector2>();
            Ray ray = Camera.main.ScreenPointToRay(mouse);
            bool didHit = Physics.Raycast(ray, out hit, MAX_RAY_DIST);
            Furniture furniture = hit.transform?.GetComponent<Furniture>();
            if (furniture != null) {
                selectFurn(furniture, hit);
            } else {
                dragging = true;
                prevMouse = mouse;
            }
        };
        pressAction.canceled += ctx => {
            mousePull.clickDown = false; // TODO do we need clickDown?
            deselectFurn();
            dragging = false;
        };
        zoomInAction.performed += ctx => {
            float newY = Mathf.Clamp(transform.position.y - scrollSensitivity, levelData.playerMinPos.y, levelData.playerMaxPos.y);
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        };
        zoomOutAction.performed += ctx => {
            float newY = Mathf.Clamp(transform.position.y + scrollSensitivity, levelData.playerMinPos.y, levelData.playerMaxPos.y);
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        };
    }

    void Update() {
        Vector2 mouse = inputActionAsset.FindAction("Drag").ReadValue<Vector2>();
        if (selected != null) {
            resizeLine(mouse, mousePull.relGrabPos, mousePull.line);
        } else if (dragging) {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(mouse.x, mouse.y, transform.position.y));
            Vector3 prevMousePos = Camera.main.ScreenToWorldPoint(new Vector3(prevMouse.x, prevMouse.y, transform.position.y));
            Vector3 delta = mousePos - prevMousePos;
            float newX = Mathf.Clamp(transform.position.x - delta.x, levelData.playerMinPos.x, levelData.playerMaxPos.x);
            float newZ = Mathf.Clamp(transform.position.z - delta.z, levelData.playerMinPos.z, levelData.playerMaxPos.z);
            transform.position = new Vector3(newX, transform.position.y, newZ);
            prevMouse = mouse;
        }
    }

    void FixedUpdate() {
        if (selected != null) {
            Ray ray = Camera.main.ScreenPointToRay(inputActionAsset.FindAction("Drag").ReadValue<Vector2>());
            Vector3 grabPos = selected.transform.TransformPoint(mousePull.relGrabPos);
            Vector3 proj_mouse = rayCastAtYLevel(ray, grabPos.y);
            Vector3 force = (proj_mouse - grabPos).normalized * dragStrengthCurve((proj_mouse - grabPos).magnitude) * dragStrength;
            selected.GetComponent<Rigidbody>().AddForceAtPosition(force, grabPos);
        }
    }

    private float dragStrengthCurve(float x) {
        if (x < 3) {
            return Mathf.Sqrt(x);
        }
        return Mathf.Min(0.289f * x + 0.865f, 20);
    }

    void selectFurn(Furniture furniture, RaycastHit hit) {
        if (!furniture.selected(deselectFurn)) {
            return;
        }
        selected = hit.transform.gameObject;
        mousePull.relGrabPos = hit.transform.InverseTransformPoint(hit.point);
        GameObject line = Instantiate(linePrefab);
        line.transform.SetParent(canvas.transform);
        mousePull.line = line.GetComponent<RectTransform>();
    }

    void deselectFurn() {
        if (selected != null) {
            selected.GetComponent<Furniture>().unselected();
            selected = null;
        }
        if (mousePull.line != null) {
                Destroy(mousePull.line.gameObject);
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

    void resizeLine(Vector2 mouse, Vector3 relGrabPos, RectTransform rt) {
        Vector3 grabPos = selected.transform.TransformPoint(relGrabPos);
        Vector2 grabScreen = Camera.main.WorldToScreenPoint(grabPos);
        grabScreen -= new Vector2(Screen.width/2, Screen.height/2);
        mouse -= new Vector2(Screen.width/2, Screen.height/2);
        RectTransformFromAToB(rt, grabScreen, mouse);
    }

    private void RectTransformFromAToB(RectTransform rt, Vector2 a, Vector2 b) {
        rt.anchoredPosition = a;
        rt.sizeDelta = new Vector2(LINE_THICKNESS, Vector2.Distance(a, b));
        rt.rotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.up, b-a));
    }
}

class MousePull
{
    public RectTransform line;
    public bool clickDown;
    public Vector3 relGrabPos;
    public MousePull() {
        clickDown = false;
        relGrabPos = Vector3.zero;
        line = null;
    }
}