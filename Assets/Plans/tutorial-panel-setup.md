# Project Overview
- Game Title: Kedainesia
- High-Level Concept: 2D restaurant management simulation.
- Target Platform: Android.
- Render Pipeline: URP.

# Game Mechanics
The game now uses a `TutorialPanel` within the `MainMenu` scene instead of a separate scene for the tutorial.

# UI
- `TutorialPanel`: A UI overlay in the Main Menu that explains game mechanics through multiple pages.
- `LevelPanel`: Contains the button to trigger the tutorial.

# Key Asset & Context
- `Assets/Scripts/TutorialManager.cs`: (New script) Manages page navigation within the Tutorial Panel.
- `Assets/Scripts/Credit.cs`: Contains `OpenPanelButton` for basic panel toggling.
- `Canvas/TutorialPanel`: The container for the tutorial UI.
- `Canvas/LevelPanel/TutorialButton`: The trigger for the tutorial.

# Implementation Steps
## Step 1: Create TutorialManager Script
**Description**: Create a script to handle page switching logic, following the existing pattern in `GuideBookManager`.
- **File**: `Assets/Scripts/TutorialManager.cs`
- **Logic**:
    - Array of `GameObject` for pages.
    - `NextPage()` and `PreviousPage()` methods.
    - `UpdatePageVisibility()` to toggle active state based on an index.
- **Assigned role**: developer
- **Dependencies**: None
- **Parallelizable**: Yes

## Step 2: Configure TutorialButton to open TutorialPanel
**Description**: Update the `TutorialButton` to show the `TutorialPanel` and hide the `LevelPanel`.
- **Action**:
    1. Select `Canvas/LevelPanel/TutorialButton`.
    2. Ensure it has an `OpenPanelButton` component (or add it).
    3. Set `panelToOpen` to `Canvas/TutorialPanel`.
    4. In `Button.OnClick`:
        - Add a listener to `OpenPanelButton.OpenPanel` on itself.
        - Add a listener to the `LevelPanel` GameObject -> `SetActive(false)`. (Optional, but recommended for clean UI).
- **Assigned role**: developer
- **Dependencies**: Step 1
- **Parallelizable**: No

## Step 3: Build TutorialPanel UI Structure
**Description**: Add necessary navigation elements to the `TutorialPanel`.
- **Action**:
    1. Create a `Pages` empty GameObject under `TutorialPanel`.
    2. Create `Page1`, `Page2`, etc. under `Pages`.
    3. Add a `NextButton`, `BackButton`, and `CloseButton` to `TutorialPanel`.
- **Assigned role**: developer
- **Dependencies**: None
- **Parallelizable**: Yes

## Step 4: Wire TutorialPanel Navigation
**Description**: Connect the navigation buttons to the `TutorialManager`.
- **Action**:
    1. Attach `TutorialManager` to `TutorialPanel`.
    2. Assign the pages array in the inspector.
    3. Wire `NextButton.OnClick` -> `TutorialManager.NextPage`.
    4. Wire `BackButton.OnClick` -> `TutorialManager.PreviousPage`.
    5. Wire `CloseButton.OnClick` -> `TutorialPanel.SetActive(false)`.
    6. Wire `CloseButton.OnClick` -> `LevelPanel.SetActive(true)`. (To return to level selection).
- **Assigned role**: developer
- **Dependencies**: Step 1, Step 3
- **Parallelizable**: No

# Verification & Testing
- **Manual Verification**:
    1. Click "Mulai" -> `LevelPanel` opens.
    2. Click "Tutorial" -> `LevelPanel` closes, `TutorialPanel` opens.
    3. Navigate through pages using "Next" and "Back".
    4. Verify buttons become inactive or behave correctly at the start/end of the page list.
    5. Click "Close" -> `TutorialPanel` closes, `LevelPanel` re-opens.
