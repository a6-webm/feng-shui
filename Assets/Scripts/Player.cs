using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using UnityEngine.UI;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] InputActionAsset InputActionAsset;
    [SerializeField] float DragStrength = 200f;
    [SerializeField] float ScrollSensitivity = 20f;
    [SerializeField] GameObject LinePrefab;
    private const float MAX_RAY_DIST = 1000f;
    private const float LINE_THICKNESS = 5f;
    private GameObject _selected;
    private MousePull _mousePull = new MousePull();
    private bool _dragging;
    private Vector2 _prevMouse;
    private LevelData _levelData;
    private GameObject _canvas;

    void Start()
    {
        _levelData = GameObject.Find("LevelManager").GetComponent<LevelManager>().levelData;
        transform.position = _levelData.playerStartPos;
        InputActionAsset.Enable();
        InputAction pressAction = InputActionAsset.FindAction("Press");
        InputAction zoomInAction = InputActionAsset.FindAction("ZoomIn");
        InputAction zoomOutAction = InputActionAsset.FindAction("ZoomOut");
        _canvas = GameObject.Find("Canvas");
        
        pressAction.started += ctx => {
            _mousePull.clickDown = true;
            RaycastHit hit;
            Vector2 mouse = InputActionAsset.FindAction("Drag").ReadValue<Vector2>();
            Ray ray = Camera.main.ScreenPointToRay(mouse);
            bool didHit = Physics.Raycast(ray, out hit, MAX_RAY_DIST);
            Furniture furniture = hit.transform?.GetComponent<Furniture>();
            if (furniture != null) {
                selectFurn(furniture, hit);
            } else {
                _dragging = true;
                _prevMouse = mouse;
            }
        };
        pressAction.canceled += ctx => {
            _mousePull.clickDown = false; // TODO do we need clickDown?
            deselectFurn();
            _dragging = false;
        };
        zoomInAction.performed += ctx => {
            float newY = Mathf.Clamp(transform.position.y - ScrollSensitivity, _levelData.playerMinPos.y, _levelData.playerMaxPos.y);
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        };
        zoomOutAction.performed += ctx => {
            float newY = Mathf.Clamp(transform.position.y + ScrollSensitivity, _levelData.playerMinPos.y, _levelData.playerMaxPos.y);
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        };
    }

    void Update() {
        Vector2 mouse = InputActionAsset.FindAction("Drag").ReadValue<Vector2>();
        if (_selected != null) {
            resizeLine(mouse, _mousePull.relGrabPos, _mousePull.line);
        } else if (_dragging) {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(mouse.x, mouse.y, transform.position.y));
            Vector3 prevMousePos = Camera.main.ScreenToWorldPoint(new Vector3(_prevMouse.x, _prevMouse.y, transform.position.y));
            Vector3 delta = mousePos - prevMousePos;
            float newX = Mathf.Clamp(transform.position.x - delta.x, _levelData.playerMinPos.x, _levelData.playerMaxPos.x);
            float newZ = Mathf.Clamp(transform.position.z - delta.z, _levelData.playerMinPos.z, _levelData.playerMaxPos.z);
            transform.position = new Vector3(newX, transform.position.y, newZ);
            _prevMouse = mouse;
        }
    }

    void FixedUpdate() {
        if (_selected != null) {
            Ray ray = Camera.main.ScreenPointToRay(InputActionAsset.FindAction("Drag").ReadValue<Vector2>());
            Vector3 grabPos = _selected.transform.TransformPoint(_mousePull.relGrabPos);
            Vector3 proj_mouse = rayCastAtYLevel(ray, grabPos.y);
            Vector3 diffVec = proj_mouse - grabPos;
            Vector3 force = diffVec.normalized * dragStrengthCurve(diffVec.magnitude) * DragStrength;
            _selected.GetComponent<Rigidbody>().AddForceAtPosition(force, grabPos);
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
        _selected = hit.transform.gameObject;
        _mousePull.relGrabPos = hit.transform.InverseTransformPoint(hit.point);
        GameObject line = Instantiate(LinePrefab);
        line.transform.SetParent(_canvas.transform);
        _mousePull.line = line.GetComponent<RectTransform>();
    }

    void deselectFurn() {
        if (_selected != null) {
            _selected.GetComponent<Furniture>().unselected();
            _selected = null;
        }
        if (_mousePull.line != null) {
                Destroy(_mousePull.line.gameObject);
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
        Vector3 grabPos = _selected.transform.TransformPoint(relGrabPos);
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