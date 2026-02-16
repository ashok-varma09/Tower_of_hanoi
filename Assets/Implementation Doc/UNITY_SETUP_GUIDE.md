# Unity Setup Guide - Single Scene Multi-Level System

This guide will help you set up the single-scene multi-level system in Unity.

---

## Overview

With this system, you'll have:
- **Main scene** (Index 0) - Menu with level selection
- **Level scene** (Index 1) - Single gameplay scene that configures dynamically

No more Level 1, 2, 3 duplicate scenes!

---

## Step 1: Scene Setup

### Rename or Create the Level Scene

**Option A: If you have existing Level scenes**
1. Open your best Level scene (probably "Level 1")
2. File → Save Scene As → Name it **"Level"**
3. Close the scene

**Option B: If starting fresh**
1. Your current Level 1 scene works fine
2. Just make sure it has all necessary components

### Delete Duplicate Scenes (Optional)
If you have Level 2, Level 3, etc., you can delete them:
1. In Project window, find Level 2.unity and Level 3.unity
2. Right-click → Delete
3. They're no longer needed!

---

## Step 2: Build Settings

1. Go to **File → Build Settings**
2. Ensure scenes are in this order:
   - **Scene 0**: Main scene (your menu)
   - **Scene 1**: Level (your gameplay scene)
3. If wrong, drag scenes to reorder
4. Remove any extra level scenes (Level 2, 3, etc.)

---

## Step 3: Prepare Level Scene

Open the **Level** scene and ensure it has enough rings:

### Check Ring Array
1. Select **GameManager** GameObject
2. In Inspector, find the **Rings** array
3. Ensure **Size >= 7** (to support levels with up to 7 rings)

**If you need more Ring GameObjects:**
1. Duplicate existing ring GameObjects
2. Name them sequentially (Ring_0, Ring_1, ... Ring_6)
3. Set their `size` property correctly (0 for smallest, increasing)
4. Add them to the GameManager's Rings array

> **Note:** Unused rings will be automatically hidden when a level loads.

---

## Step 4: Create Level Configuration Assets

### Create Folder Structure
1. In Project window, navigate to `Assets/`
2. Right-click → Create → Folder → Name it **"Resources"**
3. Inside Resources, create another folder → **"Levels"**

Path: `Assets/Resources/Levels/`

### Create Level 1 Configuration
1. Right-click in `Assets/Resources/Levels/`
2. Select **Create → Tower of Hanoi → Level Configuration**
3. Name it: **"Level_01_Beginner"**
4. Click on it to configure in Inspector:

```
Level Identity:
  Level ID: 1
  Level Name: "Beginner"

Gameplay Parameters:
  Ring Count: 3
  Max Moves: 7
  Time Limit: 60

Visual Settings (Optional):
  Ring Colors: (leave empty or set custom colors)
  Background Color: (default or customize)
```

### Create Level 2 Configuration
Repeat the process:
1. Create → Tower of Hanoi → Level Configuration
2. Name: **"Level_02_Intermediate"**
3. Configure:

```
Level Identity:
  Level ID: 2
  Level Name: "Intermediate"

Gameplay Parameters:
  Ring Count: 4
  Max Moves: 15
  Time Limit: 90
```

### Create Level 3 Configuration
1. Create → Tower of Hanoi → Level Configuration
2. Name: **"Level_03_Advanced"**
3. Configure:

```
Level Identity:
  Level ID: 3
  Level Name: "Advanced"

Gameplay Parameters:
  Ring Count: 5
  Max Moves: 31
  Time Limit: 120
```

---

## Step 5: Set Up LevelManager

### Create LevelManager GameObject
1. Open **Main scene** (your menu scene)
2. In Hierarchy, right-click → Create Empty
3. Name it: **"LevelManager"**
4. With it selected, click **Add Component** in Inspector
5. Search for and add: **LevelManager** script

### Assign Level Configurations
1. With LevelManager selected, look at Inspector
2. Find the **Levels** array
3. Set **Size** to **3**
4. Drag your level configuration assets into the array:
   - **Element 0**: Level_01_Beginner
   - **Element 1**: Level_02_Intermediate
   - **Element 2**: Level_03_Advanced

**Visual Reference:**
```
LevelManager GameObject
└── LevelManager (Script)
    └── Levels (Size: 3)
        ├── Element 0: Level_01_Beginner
        ├── Element 1: Level_02_Intermediate
        └── Element 2: Level_03_Advanced
```

> **Important:** LevelManager uses `DontDestroyOnLoad`, so it persists across scenes. Only add it to Main scene!

---

## Step 6: Update Level Buttons

In your Main scene, update your level selection buttons:

### For Each Level Button:
1. Select the button GameObject
2. In Inspector, find the **On Click ()** event
3. The function should call: `MainScript.ClickLevel(int)`
4. The parameter should be the **Level ID** (1, 2, or 3)

**Example Setup:**
- Level 1 Button → `ClickLevel(1)`
- Level 2 Button → `ClickLevel(2)`
- Level 3 Button → `ClickLevel(3)`

---

## Step 7: Testing

### Test Level 1
1. Press **Play** in Unity
2. Click Level 1 button
3. Check Console for:
   - `"LevelManager initialized with 3 levels"`
   - `"Loaded Level 1: Beginner"`
   - `"Config - Rings: 3, Moves: 7, Time: 60s"`
4. Verify: **3 rings** appear in the game

### Test Level 2
1. Return to menu (or restart)
2. Click Level 2 button
3. Check Console for:
   - `"Loaded Level 2: Intermediate"`
   - `"Config - Rings: 4, Moves: 15, Time: 90s"`
4. Verify: **4 rings** appear (same scene, different config!)

### Test Level 3
1. Return to menu
2. Click Level 3 button
3. Check Console for:
   - `"Loaded Level 3: Advanced"`
   - `"Config - Rings: 5, Moves: 31, Time: 120s"`
4. Verify: **5 rings** appear

### Test Win Condition
1. Complete a level
2. Verify win panel appears when correct number of rings are on win tower
3. Click **Next Level** → should load next level configuration

### Test Restart
1. During gameplay, click **Restart**
2. Should reload the same level with same configuration

---

## Troubleshooting

### "LevelManager not found!"
- Ensure LevelManager GameObject exists in Main scene
- Ensure LevelManager script is attached
- Must play from Main scene (not directly from Level scene)

### Wrong number of rings appear
- Check `ringCount` in level configuration
- Ensure Level scene has enough Ring GameObjects
- Check Console for initialization messages

### Same level loads every time
- Verify level configurations have unique `levelID` values (1, 2, 3)
- Check that ClickLevel button events pass correct level ID parameter

### Rings from previous level appear
- This should not happen (stacks are cleared)
- If it does, check that tower stacks are being cleared in InitializeRings()

---

## Adding More Levels

To add Level 4, 5, etc.:

1. **Create new configuration asset:**
   - Right-click in `Assets/Resources/Levels/`
   - Create → Tower of Hanoi → Level Configuration
   - Name it (e.g., "Level_04_Expert")
   - Set Level ID: 4
   - Configure ring count, moves, time

2. **Add to LevelManager:**
   - Select LevelManager in Main scene
   - Increase Levels array size
   - Drag new configuration into new slot

3. **Create button (optional):**
   - Add Level 4 button to menu
   - Set onClick → `ClickLevel(4)`

**That's it! No new scenes needed!**

---

## Optional: Custom Ring Colors

To set unique colors per level:

1. Select a level configuration asset
2. Find **Ring Colors** array
3. Set **Size** to match **Ring Count**
4. Click each color swatch to choose colors

Example for 3 rings:
- Element 0: Red (largest)
- Element 1: Green (middle)
- Element 2: Blue (smallest)

---

## Summary Checklist

- [ ] Level scene created/renamed (scene index 1)
- [ ] Build Settings updated (Main scene + Level scene only)
- [ ] Level scene has 7+ Ring GameObjects
- [ ] Created 3 level configuration assets
- [ ] LevelManager GameObject in Main scene
- [ ] Level configurations assigned to LevelManager
- [ ] Level buttons call ClickLevel with correct IDs
- [ ] Tested all 3 levels load correctly
- [ ] Tested win/restart/next level functionality

---

**Congratulations! Your single-scene multi-level system is ready!**

You can now create unlimited levels by just creating new ScriptableObject assets - no more duplicate scenes!

---

*Setup complete - enjoy your scalable level system!*
