using UnityEngine;

[CreateAssetMenu(menuName = "Tower of Hanoi/Level Configuration")]
public class LevelConfigurationSO : ScriptableObject
{
    [Header("Level Identity")]
    public int levelID;
    public string levelName;
    
    [Header("Gameplay")]
    public int ringCount = 3;
    public int maxMoves = 7;
    public int timeLimit = 60;
    
    [Header("Visual (Optional)")]
    public Color[] ringColors;
    public Color backgroundColor;
}