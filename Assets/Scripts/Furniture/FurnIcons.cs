using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Elemental;
using static Elemental.Elem;

public class FurnIcons : MonoBehaviour
{
    [SerializeField] Sprite[] FireSprites = new Sprite[6];
    [SerializeField] Sprite[] EarthSprites = new Sprite[6];
    [SerializeField] Sprite[] MetalSprites = new Sprite[6];
    [SerializeField] Sprite[] WaterSprites = new Sprite[6];
    [SerializeField] Sprite[] WoodSprites = new Sprite[6];
    [SerializeField] public Transform Furn;
    private Transform _lockIcon;
    private GameObject _createsPips;
    private GameObject _destroysPips;
    private GameObject _createsNumText;
    private GameObject _destroysNumText;
    private ReversibleAnim _lockAnim = new(0.1f);
    private ReversibleAnim _hoverAnim = new(0.1f);
    private int _numDestroying;
    private int _numCreating;

    public FurnIcons() {}
    public FurnIcons(Transform furn) {
        Furn = furn;
    }

    void Start() {
        transform.SetParent(GameObject.Find("Canvas").transform);
        _lockIcon = transform.Find("Lock");
        _createsPips = transform.Find("CreatesPips").gameObject;
        _createsPips.SetActive(false);
        _destroysPips = transform.Find("DestroysPips").gameObject;
        _destroysPips.SetActive(false);
        _createsNumText = transform.Find("CreatesNum").gameObject;
        _createsNumText.SetActive(false);
        _destroysNumText = transform.Find("DestroysNum").gameObject;
        _destroysNumText.SetActive(false);
        
    }

    void Update() {
        transform.position = Camera.main.WorldToScreenPoint(Furn.position);
        if (_lockAnim.update()) {
            _lockIcon.localScale = new Vector3(_lockAnim.Progress, _lockAnim.Progress, 1f);
        }
        if (_hoverAnim.update()) {
            var image = _createsPips.GetComponent<Image>();
            image.color = setAlpha(image.color, _hoverAnim.Progress);
            image = _destroysPips.GetComponent<Image>();
            image.color = setAlpha(image.color, _hoverAnim.Progress);
            var txt = _createsNumText.GetComponent<TextMeshProUGUI>();
            txt.color = setAlpha(txt.color, _hoverAnim.Progress);
            txt = _destroysNumText.GetComponent<TextMeshProUGUI>();
            txt.color = setAlpha(txt.color, _hoverAnim.Progress);
        }
    }

    public void showLock() {
        _lockAnim.Play = true;
    }

    public void hideLock() {
        _lockAnim.Play = false;
    }

    public void hovered() {
        _hoverAnim.Play = true;
    }

    public void unhovered() {
        _hoverAnim.Play = false;
    }

    private Sprite sprite(Elem elem, int ind) {
        switch (elem) {
            case Fire:
                return FireSprites[ind];
            case Earth:
                return EarthSprites[ind];
            case Metal:
                return MetalSprites[ind];
            case Water:
                return WaterSprites[ind];
            default: // Wood
                return WoodSprites[ind];    
        }
    }

    public void displayElement(int num, GameObject text, GameObject pips, Elem elem) {
        if (num > 6) {
            text.SetActive(true);
            pips.SetActive(true);
            pips.GetComponent<Image>().sprite = sprite(elem, 0);
            text.GetComponent<TextMeshProUGUI>().text = num.ToString();
        } else if (num >= 1) {
            text.SetActive(false);
            pips.SetActive(true);
            pips.GetComponent<Image>().sprite = sprite(elem, num-1);
        } else {
            text.SetActive(false);
            pips.SetActive(false);
        }
    }

    public void SetSurroundingElementals(int numDestroying, int numCreating, Elem elem) {
        if (_numDestroying != numDestroying || _numCreating != numCreating) {
            displayElement(numDestroying, _destroysNumText, _destroysPips, destroyedByElem(elem));
            displayElement(numCreating, _createsNumText, _createsPips, createdByElem(elem));
        }
        _numDestroying = numDestroying;
        _numCreating = numCreating;
    }

    private class ReversibleAnim {
        public float Progress {private set; get;} = 0f;
        private readonly float _duration;
        public bool Play = false;
        public bool first = true;

        public ReversibleAnim(float duration) {
            _duration = duration;
        }

        public bool update() {
            if (first) {
                first = false;
                return true;
            }
            if (Progress != 1f && Play) {
                Progress = Mathf.Min(Progress + Time.deltaTime / _duration, 1f);
            } else if (Progress != 0f && !Play) {
                Progress = Mathf.Max(Progress - Time.deltaTime / _duration, 0f);
            } else if (!first) {
                return false;
            }
            first = false;
            return true;
        }
    }

    private Color setAlpha(Color color, float alpha) {
        color.a = alpha;
        return color;
    }
}