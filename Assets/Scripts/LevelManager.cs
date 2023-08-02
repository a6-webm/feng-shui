using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

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
