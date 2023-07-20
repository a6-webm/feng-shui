using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    private HashSet<Goal> goals = new HashSet<Goal>();
    private HashSet<Goal> metGoals = new HashSet<Goal>();

    // Start is called before the first frame update
    void Start()
    {
        foreach (Goal g in FindObjectsOfType<Goal>()) { goals.Add(g); }
    }

    public void goalMet(Goal goal) {
        metGoals.Add(goal);
        if (goals.SetEquals(metGoals)) {
            Debug.Log("YOU WON!!!"); // TODO send level win event or something
        }
    }

    public void goalUnmet(Goal goal) {
        metGoals.Remove(goal);
    }
}
