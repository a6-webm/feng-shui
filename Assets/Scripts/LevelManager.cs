using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

[RequireComponent(typeof(UIDocument))]
public class LevelManager : MonoBehaviour
{

    private HashSet<Goal> _goals = new HashSet<Goal>();
    private HashSet<Goal> _metGoals = new HashSet<Goal>();

    private VisualElement _winDiv;
    private VisualElement _duringDiv;

    [SerializeField] public LevelData LevelData;

    // Start is called before the first frame update
    void Start()
    {
        foreach (Goal g in FindObjectsOfType<Goal>()) { _goals.Add(g); }
        initUI();
        initNavMesh();
    }

    private void initNavMesh() {
        var settings = NavMesh.CreateSettings();
        settings.agentClimb = 0f;
        settings.agentHeight = 17f;
        settings.agentRadius = 2.5f;
        settings.agentSlope = 0f;
        var buildSources = new List<NavMeshBuildSource>();
        var floor = new NavMeshBuildSource {
            transform = Matrix4x4.TRS(new Vector3(0, 0, 0), Quaternion.identity, Vector3.one),
            shape = NavMeshBuildSourceShape.Box,
            size = new Vector3(500, 1, 500)
        };
        buildSources.Add(floor);
        const int OBSTACLE = 1 << 0;
        foreach (ConvexCollider col in FindObjectsOfType<ConvexCollider>()) {
            var obstacle = new NavMeshBuildSource
            {
                transform = col.gameObject.transform.localToWorldMatrix,
                shape = NavMeshBuildSourceShape.Mesh,
                size = new Vector3(1, 1, 1),
                area = OBSTACLE,
                sourceObject = col.ColMesh
            };
            buildSources.Add(obstacle);
        }
        Vector3 centre = new Vector3(transform.position.x, 0, transform.position.z);
        Bounds bounds = new Bounds();
        bounds.SetMinMax(
            new Vector3(LevelData.navMinPos.x, -1, LevelData.navMinPos.y) - centre,
            new Vector3(LevelData.navMaxPos.x, 1, LevelData.navMaxPos.y) - centre
        );
        NavMeshData built = NavMeshBuilder.BuildNavMeshData(
            settings,
            buildSources,
            bounds,
            centre,
            Quaternion.identity
        );
        NavMesh.AddNavMeshData(built);
    }

    private void initUI() {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;
        _duringDiv = root.Q<VisualElement>("during-div");
        _duringDiv.Q<Button>("back").clicked += () => { SceneManager.LoadScene("Assets/Scenes/LevelSelect.unity"); };
        _duringDiv.Q<Button>("restart").clicked += () => { SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); };
        _winDiv = root.Q<VisualElement>("win-div");
        _winDiv.Q<Button>("back").clicked += () => { SceneManager.LoadScene("Assets/Scenes/LevelSelect.unity"); };
        _winDiv.Q<Button>("restart").clicked += () => { SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); };
        _winDiv.Q<Button>("next").clicked += () => { SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1); };
        _winDiv.visible = false;
    }

    public void goalMet(Goal goal) {
        _metGoals.Add(goal);
        if (_goals.SetEquals(_metGoals)) {
            Debug.Log("YOU WON!!!"); // TODO send level win event or something
            _duringDiv.visible = false;
            _winDiv.visible = true;
        }
    }

    public void goalUnmet(Goal goal) {
        _metGoals.Remove(goal);
    }
}
