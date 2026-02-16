using System.Linq;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;
    
    [Header("All Level Configurations")]
    public LevelConfigurationSO[] levels;
    
    private int currentLevelID = 1;
    
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void SetCurrentLevel(int levelID)
    {
        currentLevelID = levelID;
    }
    
    public LevelConfigurationSO GetCurrentLevelConfig()
    {
        // return levels.FirstOrDefault(l => l.levelID == currentLevelID);
        return levels.FirstOrDefault(x => x.levelID == currentLevelID);
    }
}