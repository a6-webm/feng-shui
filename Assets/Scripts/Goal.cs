using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    [SerializeField] public List<GameObject> validFurniture;
    [SerializeField] public bool CareAboutOrientation = true;
    [SerializeField] public float positionMargin = 1f;
    [SerializeField] public float rotationMargin = 5f;
    private GameObject metFurn = null;
    private LevelManager levelManager;
    MeshRenderer meshRenderer;
    private Material metMaterial;
    private Material unmetMaterial;

    void Start()
    {
        levelManager = GameObject.Find("LevelManager").GetComponent<LevelManager>();
        meshRenderer = GetComponent<MeshRenderer>();
        metMaterial = Resources.Load<Material>("metGoal");
        unmetMaterial = Resources.Load<Material>("unmetGoal");
        meshRenderer.material = unmetMaterial;
    }

    void FixedUpdate() {
        if (metFurn == null) {
            foreach (GameObject furn in validFurniture) {
                if (isMeetingGoal(furn.transform)) {
                    meetGoal(furn);
                    break;
                }
            }
        } else {
            if (!isMeetingGoal(metFurn.transform)) {
                unmeetGoal();
            }
        }
    }

    bool isMeetingGoal(Transform f) {
        Vector3 fPos = f.position;
        Vector3 pos = transform.position;
        fPos.y = 0;
        pos.y = 0;
        return Vector3.Distance(fPos, pos) <= positionMargin
                && (!CareAboutOrientation || Quaternion.Angle(f.rotation, transform.rotation) <= rotationMargin);
    }

    void meetGoal(GameObject furn) {
        metFurn = furn;
        levelManager.goalMet(this);
        meshRenderer.material = metMaterial;
    }

    void unmeetGoal() {
        metFurn = null;
        levelManager.goalUnmet(this);
        meshRenderer.material = unmetMaterial;
    }
}
