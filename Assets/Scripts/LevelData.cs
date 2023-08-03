using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "ScriptableObjects/LevelData", order = 1)]
public class LevelData : ScriptableObject
{
    public bool playerCanChangeHeight = true;
    public bool playerCanPan = true;
    public Vector3 playerMaxPos;
    public Vector3 playerMinPos;
    public Vector3 playerStartPos;
    public Vector2 navMaxPos;
    public Vector2 navMinPos;
}