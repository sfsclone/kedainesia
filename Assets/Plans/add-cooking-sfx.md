# Project Overview
- Game Title: Kedainesia
- High-Level Concept: 2D restaurant management and cooking simulation focused on Indonesian cuisine.
- Players: Single player
- Inspiration / Reference Games: Diner Dash, Cooking Mama
- Tone / Art Direction: 2D Stylized
- Target Platform: Android
- Render Pipeline: URP

# Game Mechanics
## Core Gameplay Loop
Players take orders, select recipes, match ingredients, and cook food to serve customers before they lose patience.
## Controls and Input Methods
Touch/Mouse interactions for drag-and-drop ingredients and clicking buttons.

# UI
- CraftingPanel: Contains the ingredient input and cooking progress.
- CookButton: Triggers the cooking process.
- CookingSlider: Shows the progress of cooking.

# Key Asset & Context
- `Assets/Audio/sfx/craftingcooking.wav`: The sound effect to play.
- `Assets/Scripts/CraftingManager.cs`: Controls the cooking logic.
- `Assets/Scripts/AudioManager.cs`: Manages global audio.

# Implementation Steps
## 1. Enhance AudioManager for SFX
- **Description**: Update `AudioManager.cs` to handle SFX playback. This involves:
    - Adding a `public AudioSource sfxSource` field.
    - Adding a `public void PlaySFX(AudioClip clip)` method.
    - Implementing a Singleton pattern for easier access.
    - Adding an "SFX" group to the `MainMixer` and routing `sfxSource` to it.
- **Assigned role**: developer
- **Dependencies**: None
- **Parallelizable**: Yes

## 2. Update CraftingManager to Trigger SFX
- **Description**: Update `CraftingManager.cs` to play the cooking SFX when cooking starts.
    - Add a `public AudioClip cookSFX` field.
    - In `CookingProcess()` coroutine, call `AudioManager.Instance.PlaySFX(cookSFX)` right after activating the `cookingSlider`.
- **Assigned role**: developer
- **Dependencies**: Step 1
- **Parallelizable**: No

## 3. Scene Setup
- **Description**: Configure the components in the scene.
    - Open `Assets/Scenes/GameScene1.unity`.
    - Select the `AudioManager` GameObject.
    - Add a new `AudioSource` component for SFX (uncheck "Play on Awake").
    - Assign the new `AudioSource` to the `sfxSource` field in the `AudioManager` component.
    - Select the `CraftingManager` GameObject.
    - Assign the `Assets/Audio/sfx/craftingcooking.wav` clip to the `cookSFX` field.
- **Assigned role**: developer
- **Dependencies**: Step 1, Step 2
- **Parallelizable**: No

# Verification & Testing
- Start the game in the editor.
- Open the crafting panel.
- Select a recipe and add the correct ingredients.
- Press the **Cook** button.
- Verify that the `craftingcooking` SFX plays exactly when the `CookingSlider` appears.
- Verify that the sound volume is appropriate and doesn't loop (unless intended).
