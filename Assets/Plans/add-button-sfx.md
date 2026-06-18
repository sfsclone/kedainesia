# Project Overview
- Game Title: Kedainesia
- High-Level Concept: 2D restaurant management game.
- Task: Add button click sound effects to specific buttons efficiently.

# Game Mechanics
## Controls and Input Methods
- UI Buttons (Unity uGUI).

# Key Asset & Context
- `Assets/Audio/sfx/button_sfx.wav`: The sound clip to be used.
- `AudioManager.cs`: The existing singleton that handles SFX playback.

# Implementation Steps
## 1. Create a Reusable Script
- **Description**: Create `Assets/Scripts/ButtonSFXTrigger.cs`. This script will:
    - Require a `UnityEngine.UI.Button` component.
    - Have a field for an `AudioClip`.
    - Automatically add a listener to the button's `onClick` event during `Start` to play the clip through `AudioManager.Instance.PlaySFX`.
- **Assigned role**: developer
- **Dependencies**: None
- **Parallelizable**: Yes

## 2. Apply to Specific Buttons
- **Description**: Attach the `ButtonSFXTrigger` script to the desired buttons in the scene (e.g., Stove button, Open button, Restart button).
- **Assigned role**: developer
- **Dependencies**: Step 1
- **Parallelizable**: No

# Verification & Testing
- Enter Play Mode.
- Click the buttons with the script attached.
- Verify that `button_sfx` plays.
- Verify that buttons *without* the script remain silent.
