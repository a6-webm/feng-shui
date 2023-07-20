using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "ScriptableObjects/LevelData", order = 1)]
public class LevelData : ScriptableObject
{
    public bool playerCanChangeHeight;
    public bool playerCanPan;
    public Vector3 playerMaxPos;
    public Vector3 playerMinPos;
    public Vector3 playerStartPos;
}