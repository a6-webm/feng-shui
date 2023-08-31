using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using UnityEngine.UI;
using UnityEngine.InputSystem;
using static Player.PlayerState;
using static Player.PlayerFurnState;

public class Player : MonoBehaviour
{
    public enum PlayerFurnState{None, Hovered, Selected};
    public enum PlayerState{None, Panning, Dragging};
    [SerializeField] InputActionAsset InputActionAsset;
    [SerializeField] float DragStrength = 200f;
    [SerializeField] float ScrollSensitivity = 20f;
    [SerializeField] GameObject LinePrefab;
    public event Action<GameObject> DeselectEvent;
    private const float MAX_RAY_DIST = 1000f;
    private const float LINE_THICKNESS = 5f;
    private PlayerState _state;
    private Vector2 _prevMouse;
    private GameObject _focusedFurn;
    private Vector3 _relGrabPos;
    private RectTransform _dragLine;
    private LevelData _levelData;
    private GameObject _canvas;

    void Start()
    {
        _levelData = GameObject.Find("LevelManager").GetComponent<LevelManager>().LevelData;
        _canvas = GameObject.Find("Canvas");
        transform.position = _levelData.playerStartPos;

        InputActionAsset.Enable();
        InputAction pressAction = InputActionAsset.FindAction("Press");
        InputAction zoomInAction = InputActionAsset.FindAction("ZoomIn");
        InputAction zoomOutAction = InputActionAsset.FindAction("ZoomOut");
        pressAction.started += pressStarted;
        pressAction.canceled += pressCancelled;
        zoomInAction.performed += zoomIn;
        zoomOutAction.performed += zoomOut;
    }

    void OnDestroy() {
        InputAction pressAction = InputActionAsset.FindAction("Press");
        InputAction zoomInAction = InputActionAsset.FindAction("ZoomIn");
        InputAction zoomOutAction = InputActionAsset.FindAction("ZoomOut");
        pressAction.started -= pressStarted;
        pressAction.canceled -= pressCancelled;
        zoomInAction.performed -= zoomIn;
        zoomOutAction.performed -= zoomOut;
    }

    private void zoomIn(InputAction.CallbackContext ctx) {
        float newY = Mathf.Clamp(transform.position.y - ScrollSensitivity, _levelData.playerMinPos.y, _levelData.playerMaxPos.y);
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    private void zoomOut(InputAction.CallbackContext ctx) {
        float newY = Mathf.Clamp(transform.position.y + ScrollSensitivity, _levelData.playerMinPos.y, _levelData.playerMaxPos.y);
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    private void pressStarted(InputAction.CallbackContext ctx) {
        if (_focusedFurn != null) {
            _state = Dragging;
            _focusedFurn.GetComponent<Furniture>().setState(Selected);
            var line = Instantiate(LinePrefab);
            line.transform.SetParent(_canvas.transform);
            _dragLine = line.GetComponent<RectTransform>();
        } else {
            _state = Panning;
            _prevMouse = InputActionAsset.FindAction("Drag").ReadValue<Vector2>();
        }
    }

    private void pressCancelled(InputAction.CallbackContext ctx) {
        if (_state == Dragging) Destroy(_dragLine.gameObject);
        _state = PlayerState.None;
    }

    void Update() {
        Vector2 mouse = InputActionAsset.FindAction("Drag").ReadValue<Vector2>();
        if (_state == Dragging) {
            resizeLine(mouse);
        } else if (_state == Panning) {
            panPlayer(mouse);
        }
        handleHovering(mouse);
    }

    private void panPlayer(Vector2 mouse) {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(mouse.x, mouse.y, transform.position.y));
        Vector3 prevMousePos = Camera.main.ScreenToWorldPoint(new Vector3(_prevMouse.x, _prevMouse.y, transform.position.y));
        Vector3 delta = mousePos - prevMousePos;
        float newX = Mathf.Clamp(transform.position.x - delta.x, _levelData.playerMinPos.x, _levelData.playerMaxPos.x);
        float newZ = Mathf.Clamp(transform.position.z - delta.z, _levelData.playerMinPos.z, _levelData.playerMaxPos.z);
        transform.position = new Vector3(newX, transform.position.y, newZ);
        _prevMouse = mouse;
    }

    private void handleHovering(Vector2 mouse) {
        if (_state == Dragging) return;
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(mouse);
        bool didHit = Physics.Raycast(ray, out hit, MAX_RAY_DIST, LayerMask.GetMask("Default", "Turtleable"));
        Furniture furniture = hit.transform?.GetComponent<Furniture>();
        if (furniture != null) {
            _relGrabPos = hit.transform.InverseTransformPoint(hit.point);
            if (_focusedFurn != null) _focusedFurn.GetComponent<Furniture>().setState(PlayerFurnState.None);
            _focusedFurn = hit.transform.gameObject;
            _focusedFurn.GetComponent<Furniture>().setState(Hovered);
        } else {
            if (_focusedFurn != null) {
                _focusedFurn.GetComponent<Furniture>().setState(PlayerFurnState.None);
                DeselectEvent?.Invoke(_focusedFurn);
            }
            _focusedFurn = null;
        }
    }

    void FixedUpdate() {
        if (_state == Dragging) applyForceToFurn();
    }

    private void applyForceToFurn() {
        Ray ray = Camera.main.ScreenPointToRay(InputActionAsset.FindAction("Drag").ReadValue<Vector2>());
        Vector3 grabPos = _focusedFurn.transform.TransformPoint(_relGrabPos);
        Vector3 proj_mouse = rayCastAtYLevel(ray, grabPos.y);
        Vector3 diffVec = proj_mouse - grabPos;
        Vector3 force = diffVec.normalized * dragStrengthCurve(diffVec.magnitude) * DragStrength;
        _focusedFurn.GetComponent<Rigidbody>().AddForceAtPosition(force, grabPos);
    }

    private float dragStrengthCurve(float x) {
        if (x < 3) {
            return Mathf.Sqrt(x);
        }
        return Mathf.Min(0.289f * x + 0.865f, 20);
    }

    private void resizeLine(Vector2 mouse) {
        Vector3 grabPos = _focusedFurn.transform.TransformPoint(_relGrabPos);
        Vector2 grabScreen = Camera.main.WorldToScreenPoint(grabPos);
        grabScreen -= new Vector2(Screen.width/2, Screen.height/2);
        mouse -= new Vector2(Screen.width/2, Screen.height/2);
        RectTransformFromAToB(_dragLine, grabScreen, mouse);
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

    private void RectTransformFromAToB(RectTransform rt, Vector2 a, Vector2 b) {
        rt.anchoredPosition = a;
        rt.sizeDelta = new Vector2(LINE_THICKNESS, Vector2.Distance(a, b));
        rt.rotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.up, b-a));
    }
}