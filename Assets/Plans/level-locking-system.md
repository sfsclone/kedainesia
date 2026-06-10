# Project Overview
- Game Title: Kedainesia
- High-Level Concept: Restaurant management game focusing on traditional Indonesian cuisine.
- Players: Single player
- Target Platform: Android
- Render Pipeline: URP

# Game Mechanics
## Core Gameplay Loop
Players serve customers traditional Indonesian dishes. Progressing through days increases difficulty. Level progress is currently not persistent between sessions or menu visits.

## Level Locking
Players must complete the previous day/level to unlock the next one. Locked levels are visually distinguished by being darker and are non-interactable.

# UI
- **MainMenu / LevelPanel**: Contains buttons for Level 1, Level 2, Level 3, and Level 4.
- **Visuals**: Locked buttons will have their `Image` color set to a darker tint (e.g., grey/dark) and their `Button.interactable` set to false.

# Key Asset & Context
- `Assets/Scripts/GameManager.cs`: Existing manager that handles day progression.
- `Assets/Scripts/LevelLockManager.cs`: New script to be attached to `LevelPanel` to manage button states.
- `PlayerPrefs`: Used to persist the `HighestUnlockedDay` (int).

# Implementation Steps
1. **Modify `GameManager.cs`**:
    - Rename `gameFinishedPanel` to `winPanel` and use `[FormerlySerializedAs("gameFinishedPanel")]` to keep the reference.
    - Rename `ShowGameFinishedPanel()` to `ShowWinPanel()`.
    - Update `AdvanceDay()` to save the new `currentDay` to `PlayerPrefs` under the key `"HighestUnlockedDay"`.
    - In `ShowWinPanel()`, save the final progress (e.g., `PlayerPrefs.SetInt("HighestUnlockedDay", maxDays)`).
    - Assigned role: developer
    - Dependencies: None

2. **Create `LevelLockManager.cs`**:
    - Implement logic to read `"HighestUnlockedDay"` from `PlayerPrefs`.
    - Loop through an array of level buttons.
    - Set `interactable` and `color` based on whether the level is unlocked.
    - Assigned role: developer
    - Dependencies: None

3. **Configure `MainMenu` Scene**:
    - Attach `LevelLockManager` to the `LevelPanel` GameObject.
    - Assign `Level1Button` through `Level4Button` to the `levelButtons` array in the inspector.
    - Assigned role: developer
    - Dependencies: Step 2

4. **Verify Persistence**:
    - Complete Day 1 in the game.
    - Return to Main Menu.
    - Verify that Level 2 is now unlocked and Level 3/4 are still locked.
    - Assigned role: developer
    - Dependencies: Steps 1-3

# Verification & Testing
- **Test Case 1: First Run**: Open the game for the first time. Level 1 should be unlocked, others locked.
- **Test Case 2: Unlock Progress**: Complete Level 1. Return to menu. Level 2 should be unlocked.
- **Test Case 3: Visuals**: Ensure locked levels are noticeably darker.
- **Test Case 4: Interaction**: Ensure locked levels cannot be clicked.
