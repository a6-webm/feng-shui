using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    private HashSet<Goal> _goals = new HashSet<Goal>();
    private HashSet<Goal> _metGoals = new HashSet<Goal>();

    [SerializeField] public LevelData levelData;

    // Start is called before the first frame update
    void Start()
    {
        foreach (Goal g in FindObjectsOfType<Goal>()) { _goals.Add(g); }
    }

    public void goalMet(Goal goal) {
        _metGoals.Add(goal);
        if (_goals.SetEquals(_metGoals)) {
            Debug.Log("YOU WON!!!"); // TODO send level win event or something
        }
    }

    public void goalUnmet(Goal goal) {
        _metGoals.Remove(goal);
    }
}
