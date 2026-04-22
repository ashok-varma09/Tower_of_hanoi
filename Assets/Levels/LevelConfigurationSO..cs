using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tower of Hanoi/Level Configuration")]
public class LevelConfigurationSO : ScriptableObject
{
    [Header("Level Identity")]
    public int levelID;
    public string levelName;
    
    [Header("Gameplay")]
    public int ringCount = 3;
    public int towerCount = 3;
    public int maxMoves = 7;
    public int timeLimit = 60;

    public int[] Ringparentindex; // Array to hold the parent index of each ring at the start of the level, where the index represents the ring and the value represents the tower it starts on.
    // public int[] RingCountOnTower;. // Array to hold the count of rings on each tower at the start of the level 
    // in the dictionary int repreasent the tower index and transform is the position of the ring on that tower.
    public int tower1ringcount,tower2ringcount,tower3ringcount;
    [Header("Visual (Optional)")]
    public Color[] ringColors;
    public Color backgroundColor;
}