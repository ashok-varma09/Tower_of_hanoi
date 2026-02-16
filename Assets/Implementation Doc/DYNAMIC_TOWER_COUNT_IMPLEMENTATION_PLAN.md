# Dynamic Tower Count Implementation Plan
### Tower of Hanoi - Supporting Variable Number of Towers

## Overview
This plan extends the Tower of Hanoi game to support **different numbers of towers** (3, 4, 5, or more) instead of being locked to the classic 3-tower configuration. This will make the game more challenging and varied.

---

## 🎯 Goals
1. **Support 3+ towers** - Allow levels with 3, 4, 5, or more towers
2. **Dynamic tower spawning** - Automatically create/destroy towers based on level configuration
3. **Flexible win conditions** - Support multiple valid win configurations
4. **Backward compatibility** - Ensure existing 3-tower levels still work
5. **UI adaptation** - Automatically adjust camera & layout for different tower counts

---

## 📋 Implementation Steps

### **Phase 1: Update Data Layer (ScriptableObject)**

#### 1.1 Update `LevelConfigurationSO.cs`
Add tower count configuration:
```csharp
[Header("Tower Configuration")]
public int towerCount = 3;  // Number of towers (3, 4, 5, etc.)
public TowerRole[] towerRoles;  // Define each tower's starting role

[System.Serializable]
public class TowerRole
{
    public string towerName;  // e.g., "Source", "Auxiliary", "Destination"
    public bool isStartTower;  // Should rings start here?
    public bool isWinTower;    // Is this a valid win tower?
}
```

**Why?** Each level can now specify exactly how many towers it needs and which tower should contain rings at the start and end.

---

### **Phase 2: Update Tower Management**

#### 2.1 Create `TowerSpawner.cs` (NEW SCRIPT)
This script dynamically spawns towers based on level configuration:

```csharp
public class TowerSpawner : MonoBehaviour
{
    [Header("Tower Prefab")]
    public GameObject towerPrefab;  // Prefab containing Tower script + visuals
    
    [Header("Layout Settings")]
    public float towerSpacing = 3f;  // Distance between towers
    public Transform towerParent;    // Parent transform for organization
    
    private List<Tower> spawnedTowers = new List<Tower>();
    
    public List<Tower> SpawnTowers(LevelConfigurationSO levelConfig)
    {
        // Clear existing towers
        ClearTowers();
        
        int towerCount = levelConfig.towerCount;
        
        // Calculate starting X position to center towers
        float startX = -(towerCount - 1) * towerSpacing / 2f;
        
        // Spawn each tower
        for (int i = 0; i < towerCount; i++)
        {
            Vector3 position = new Vector3(startX + i * towerSpacing, 0, 0);
            GameObject towerObj = Instantiate(towerPrefab, position, Quaternion.identity, towerParent);
            
            Tower tower = towerObj.GetComponent<Tower>();
            tower.towerIndex = i;
            tower.towerName = levelConfig.towerRoles[i].towerName;
            
            spawnedTowers.Add(tower);
        }
        
        return spawnedTowers;
    }
    
    public void ClearTowers()
    {
        foreach (var tower in spawnedTowers)
        {
            Destroy(tower.gameObject);
        }
        spawnedTowers.Clear();
    }
}
```

**Why?** Instead of hardcoding 3 tower references in GameManager, we dynamically create the exact number needed.

---

#### 2.2 Update `Tower.cs`
Add identification properties:
```csharp
public class Tower : MonoBehaviour
{
    public Stack<Ring> Ringstack = new Stack<Ring>();
    public GameManager gm;
    public Transform[] position;
    
    // NEW PROPERTIES
    public int towerIndex;      // 0, 1, 2, 3, etc.
    public string towerName;    // "Source", "Auxiliary", "Target", etc.
    public bool isStartTower;   // Does this tower hold rings at start?
    public bool isWinTower;     // Is this a valid destination?
    
    private void OnMouseDown()
    {
        gm.Towerclick(this);   
    }
}
```

---

### **Phase 3: Refactor GameManager**

#### 3.1 Replace Fixed Tower References
**BEFORE (Current Code):**
```csharp
public Tower t1, from, to, WinTower;  // Hardcoded 3 towers
```

**AFTER (Dynamic):**
```csharp
public TowerSpawner towerSpawner;
private List<Tower> towers = new List<Tower>();
private Tower startTower;      // Tower with initial rings
private Tower winTower;        // Primary win tower
private Tower from, to;        // Selected towers during gameplay
```

#### 3.2 Update `Start()` Method
```csharp
private void Start()
{
    CurrentLevel = LevelManager.instance.GetCurrentLevelConfig();
    Application.targetFrameRate = 60;
    
    // SPAWN TOWERS DYNAMICALLY
    towers = towerSpawner.SpawnTowers(CurrentLevel);
    
    // IDENTIFY SPECIAL TOWERS
    for (int i = 0; i < towers.Count; i++)
    {
        towers[i].isStartTower = CurrentLevel.towerRoles[i].isStartTower;
        towers[i].isWinTower = CurrentLevel.towerRoles[i].isWinTower;
        
        if (towers[i].isStartTower)
            startTower = towers[i];
            
        if (towers[i].isWinTower)
            winTower = towers[i];
    }
    
    // PLACE RINGS ON START TOWER
    for (int i = 0; i < rings.Length; i++)
    {
        startTower.Ringstack.Push(rings[i]);
    }
    
    IsGameStart = true;
    StartCoroutine(Timeing());
    MoveText.text = Moves.ToString();
}
```

#### 3.3 Update Win Condition
**BEFORE:**
```csharp
if (WinTower.Ringstack.Count == rings.Length)
```

**AFTER (Support Multiple Win Towers):**
```csharp
// Check if ANY valid win tower has all rings
foreach (var tower in towers)
{
    if (tower.isWinTower && tower.Ringstack.Count == rings.Length)
    {
        IsGameStart = false;
        Gamewin.SetActive(true);
        Debug.Log("Win on tower: " + tower.towerName);
        SoundManager.instance.PlaySfx("Win");
        break;
    }
}
```

---

### **Phase 4: Camera & Visual Adjustments**

#### 4.1 Create `CameraAdjuster.cs` (NEW SCRIPT)
Auto-adjust camera based on tower count:

```csharp
public class CameraAdjuster : MonoBehaviour
{
    public Camera gameCamera;
    
    public void AdjustCameraForTowers(int towerCount)
    {
        // Adjust orthographic size or FOV based on tower count
        if (gameCamera.orthographic)
        {
            // For orthographic cameras
            gameCamera.orthographicSize = 5 + (towerCount - 3) * 0.8f;
        }
        else
        {
            // For perspective cameras - adjust Z position
            float baseZ = -10f;
            float zOffset = -(towerCount - 3) * 1.5f;
            gameCamera.transform.position = new Vector3(0, 5, baseZ + zOffset);
        }
    }
}
```

---

### **Phase 5: Unity Setup**

#### 5.1 Create Tower Prefab
1. Create a new **Tower Prefab** with:
   - Tower script component
   - Visual mesh (pole + base)
   - Collider for mouse/touch input
   - Position markers for rings (Transform array)

#### 5.2 Update Scene Hierarchy
```
GameManager (GameObject)
├── TowerSpawner (Component)
├── CameraAdjuster (Component)  
└── TowerParent (Empty Transform)
    └── [Towers spawn here at runtime]
```

#### 5.3 Create Level Configurations
Create multiple ScriptableObject assets:

**Example: 3-Tower Level**
- Tower Count: 3
- Tower Roles:
  - Tower 0: Start=true, Win=false
  - Tower 1: Start=false, Win=false
  - Tower 2: Start=false, Win=true

**Example: 4-Tower Level**
- Tower Count: 4
- Tower Roles:
  - Tower 0: Start=true, Win=false
  - Tower 1: Start=false, Win=false
  - Tower 2: Start=false, Win=false
  - Tower 3: Start=false, Win=true

**Example: 5-Tower Challenge**
- Tower Count: 5
- Tower Roles:
  - Tower 0: Start=true, Win=false
  - Towers 1-3: Auxiliary
  - Tower 4: Start=false, Win=true

---

## 🎮 Gameplay Variations

### With Different Tower Counts:

**3 Towers (Classic)**
- Classic Tower of Hanoi
- Minimum moves: 2^n - 1

**4 Towers (Reve's Puzzle)**
- Easier than 3 towers
- More strategic options
- Fewer minimum moves required

**5+ Towers**
- Very easy strategically
- Good for beginners
- Focus on speed rather than efficiency

---

## 📦 Files to Create/Modify

### NEW FILES:
1. ✅ `/Scripts/TowerSpawner.cs`
2. ✅ `/Scripts/CameraAdjuster.cs`
3. ✅ `/Prefabs/Tower.prefab`
4. ✅ Multiple Level Configuration assets

### MODIFIED FILES:
1. ✅ `/Levels/LevelConfigurationSO..cs` - Add tower count & roles
2. ✅ `/Scripts/GameManager.cs` - Remove hardcoded towers, add dynamic spawning
3. ✅ `/Scripts/Tower.cs` - Add identification properties
4. ✅ `/Scripts/MainScript.cs` - (No changes needed)

---

## 🧪 Testing Checklist

- [ ] 3-tower levels work correctly
- [ ] 4-tower levels spawn properly
- [ ] 5-tower levels spawn and play correctly
- [ ] Camera adjusts for all tower counts
- [ ] Win condition works for any tower count
- [ ] Towers are evenly spaced
- [ ] Ring movement animation works across all towers
- [ ] Error messages display correctly
- [ ] Level progression works with mixed tower counts

---

## 🚀 Migration Strategy

1. **Backup** current project
2. **Create** new scripts (TowerSpawner, CameraAdjuster)
3. **Update** LevelConfigurationSO with tower configuration
4. **Refactor** GameManager to use dynamic towers
5. **Create** Tower prefab
6. **Test** with existing 3-tower levels
7. **Create** new 4+ tower levels
8. **Polish** UI and camera adjustments

---

## 💡 Future Enhancements

1. **Custom Tower Arrangement** - Not just linear, but circular or grid patterns
2. **Mixed Win Conditions** - Distribute rings across multiple towers
3. **Tower-Specific Rules** - Some towers can only hold certain ring colors
4. **Dynamic Difficulty** - Adjust tower count based on player performance
5. **Visual Themes** - Different tower styles for different counts

---

## ⚠️ Important Notes

- **Ring Position Arrays**: Each tower's `position` array must have enough entries for all rings
- **Performance**: Don't spawn more than 10 towers (rendering/performance)
- **UI Layout**: May need to adjust UI for wider layouts with many towers
- **Touch Controls**: Ensure towers don't overlap on mobile devices
- **Save System**: Update PlayerPrefs to save tower count with level data

---

## 📊 Complexity Estimation

| Task | Time | Difficulty |
|------|------|------------|
| Update LevelConfigurationSO | 15 min | ⭐ Easy |
| Create TowerSpawner | 30 min | ⭐⭐ Medium |
| Create CameraAdjuster | 20 min | ⭐ Easy |
| Refactor GameManager | 1 hour | ⭐⭐⭐ Hard |
| Update Tower.cs | 10 min | ⭐ Easy |
| Create Tower Prefab | 30 min | ⭐⭐ Medium |
| Create Level Assets | 20 min | ⭐ Easy |
| Testing & Polish | 1 hour | ⭐⭐ Medium |
| **TOTAL** | **~4 hours** | ⭐⭐⭐ Medium-Hard |

---

**Ready to implement? Let me know which phase you'd like to start with!** 🚀
