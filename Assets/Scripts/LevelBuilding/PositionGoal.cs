using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionGoal : Goal
{
    [SerializeField] Material MetMaterial;
    [SerializeField] Material UnmetMaterial;
    [SerializeField] List<GameObject> ValidFurniture;
    [SerializeField] bool CareAboutOrientation = true;
    [SerializeField] float PositionMargin = 1f;
    [SerializeField] float RotationMargin = 5f;
    private LevelManager _levelManager;
    private GameObject _metFurn = null;
    private MeshRenderer _meshRenderer;

    void Start()
    {
        _levelManager = GameObject.Find("LevelManager").GetComponent<LevelManager>();
        _meshRenderer = GetComponent<MeshRenderer>();
        _meshRenderer.material = UnmetMaterial;
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
        _meshRenderer.material = MetMaterial;
        _levelManager.goalMet(this);
    }

    void unmeetGoal() {
        _metFurn = null;
        _meshRenderer.material = UnmetMaterial;
        _levelManager.goalUnmet(this);
    }
}
