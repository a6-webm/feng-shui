using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    [SerializeField] List<GameObject> ValidFurniture;
    [SerializeField] bool CareAboutOrientation = true;
    [SerializeField] float PositionMargin = 1f;
    [SerializeField] float RotationMargin = 5f;
    private GameObject _metFurn = null;
    private LevelManager _levelManager;
    private MeshRenderer _meshRenderer;
    private Material _metMaterial;
    private Material _unmetMaterial;

    void Start()
    {
        _levelManager = GameObject.Find("LevelManager").GetComponent<LevelManager>();
        _meshRenderer = GetComponent<MeshRenderer>();
        _metMaterial = Resources.Load<Material>("metGoal");
        _unmetMaterial = Resources.Load<Material>("unmetGoal");
        _meshRenderer.material = _unmetMaterial;
    }

    void FixedUpdate() {
        if (_metFurn == null) {
            foreach (GameObject furn in ValidFurniture) {
                if (isMeetingGoal(furn.transform)) {
                    meetGoal(furn);
                    break;
                }
            }
        } else {
            if (!isMeetingGoal(_metFurn.transform)) {
                unmeetGoal();
            }
        }
    }

    bool isMeetingGoal(Transform f) {
        Vector3 fPos = f.position;
        Vector3 pos = transform.position;
        fPos.y = 0;
        pos.y = 0;
        return Vector3.Distance(fPos, pos) <= PositionMargin
                && (!CareAboutOrientation || Quaternion.Angle(f.rotation, transform.rotation) <= RotationMargin);
    }

    void meetGoal(GameObject furn) {
        _metFurn = furn;
        _levelManager.goalMet(this);
        _meshRenderer.material = _metMaterial;
    }

    void unmeetGoal() {
        _metFurn = null;
        _levelManager.goalUnmet(this);
        _meshRenderer.material = _unmetMaterial;
    }
}
