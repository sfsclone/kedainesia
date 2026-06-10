# Project Overview
- Game Title: Kedainesia
- High-Level Concept: A 2D restaurant management and cooking simulation game focused on traditional Indonesian cuisine.
- Players: Single player.
- Inspiration / Reference Games: Cooking Mama, Diner Dash.
- Tone / Art Direction: Cultural, 2D Indonesian aesthetic.
- Target Platform: Android.
- Screen Orientation / Resolution: Landscape (1920x1080).
- Render Pipeline: URP.

# Game Mechanics
## Core Gameplay Loop
The game focuses on serving Indonesian dishes to customers within a time limit. Players must select recipes, drag ingredients, cook, and serve correctly to progress through 7 days of increasing difficulty.

## Controls and Input Methods
- Touch/Drag: Dragging ingredients to slots and cooked food to customers.
- UI Buttons: Menu navigation, recipe selection, and starting the day.

# UI
The main menu features buttons for starting the game (Mulai), viewing credits, and quitting. The new Level Selection panel allows players to choose between the Tutorial and specific game levels.

# Key Asset & Context
- `Assets/Scripts/MainMenuManager.cs`: Controls main menu transitions and scene loading.
- `Assets/Scripts/Credit.cs`: Contains the `OpenPanelButton` helper class for toggling UI panels.
- `Assets/Scenes/MainMenu.unity`: The scene where the menu UI is located.
- `Assets/Scenes/TutorialScene.unity`: The target scene for the tutorial.

# Implementation Steps
## Step 1: Update MainMenuManager Script
**Description**: Add a method to `MainMenuManager.cs` to load the Tutorial scene.
- **File**: `Assets/Scripts/MainMenuManager.cs`
- **Changes**: Add `public void LoadTutorialScene() { SceneManager.LoadScene("TutorialScene"); }`.
- **Assigned role**: developer
- **Dependencies**: None
- **Parallelizable**: Yes

## Step 2: Configure Build Settings
**Description**: Add the `TutorialScene` to the project's Build Settings so it can be loaded at runtime.
- **Action**: Add `Assets/Scenes/TutorialScene.unity` to the "Scenes In Build" list.
- **Assigned role**: developer
- **Dependencies**: None
- **Parallelizable**: Yes

## Step 3: Connect MulaiButton to LevelPanel
**Description**: Change the `MulaiButton` behavior to open the `LevelPanel` instead of starting the game directly.
- **Action**: 
    1. Select `Canvas/Background/MulaiButton` in the `MainMenu` scene.
    2. Add the `OpenPanelButton` component to it.
    3. Assign the `LevelPanel` GameObject to the `panelToOpen` field.
    4. In the `Button` component's `OnClick` event list:
        - Remove the existing call to `MainMenuManager.StartNewGame`.
        - Add a new entry calling `OpenPanelButton.OpenPanel` on itself.
- **Assigned role**: developer
- **Dependencies**: Step 1
- **Parallelizable**: No

## Step 4: Connect TutorialButton to TutorialScene
**Description**: Configure the `TutorialButton` to load the `TutorialScene`.
- **Action**:
    1. Select `Canvas/LevelPanel/TutorialButton` in the `MainMenu` scene.
    2. In the `Button` component's `OnClick` event list:
        - Add a new entry.
        - Assign the `MainMenuManager` GameObject.
        - Select `MainMenuManager.LoadTutorialScene`.
- **Assigned role**: developer
- **Dependencies**: Step 1
- **Parallelizable**: No

## Step 5: Initialize Menu State
**Description**: Ensure the `LevelPanel` is hidden when the game starts.
- **Action**: Set the `Canvas/LevelPanel` GameObject to **Inactive** in the `MainMenu` scene.
- **Assigned role**: developer
- **Dependencies**: None
- **Parallelizable**: Yes

# Verification & Testing
- **Manual Verification**:
    1. Run the `MainMenu` scene.
    2. Click the `MulaiButton` and verify the `LevelPanel` appears.
    3. Click the `TutorialButton` and verify the `TutorialScene` loads correctly.
    4. Verify the `LevelPanel` is not visible immediately upon starting the `MainMenu`.
- **Edge Cases**:
    - Ensure that clicking `MulaiButton` multiple times doesn't cause issues (the panel should already be active).
    - Ensure the `TutorialScene` exists in the build list (otherwise `LoadScene` will fail).
