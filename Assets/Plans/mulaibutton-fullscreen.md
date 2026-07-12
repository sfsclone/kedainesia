# Project Overview 
- Game Title: Kedainesia
- High-Level Concept: Kedainesia is a 2D management and cooking simulation game developed for Android. Players take on the role of a restaurant owner serving traditional Indonesian dishes to various customers.
- Players: Single player
- Inspiration / Reference Games: Diner Dash, Cooking Fever
- Tone / Art Direction: 2D Indonesian culinary theme
- Target Platform: Android
- Screen Orientation / Resolution: Landscape (2280x1080)
- Render Pipeline: URP

# Game Mechanics 
## Core Gameplay Loop
The player starts at the `MainMenu` scene, clicks to open the level selection panel, and begins cooking/serving traditional dishes. Correct service progresses through 7 days while failures trigger a restart.
## Controls and Input Methods
Standard touch/mouse pointer interaction. Drag-and-drop mechanics for ingredients and cooked food, and standard UI button clicks.

# UI
The main menu contains a central background canvas with several overlay panels (Settings, Credits, Level Selection). Currently, starting the game requires tapping a specific "MulaiButton" ("Tekan untuk main...") at the bottom center. We will convert this button into a full-screen trigger that detects taps/clicks anywhere on the empty screen area, while keeping other UI buttons (Settings, Credits) functional.

# Key Asset & Context
### New Script: `Assets/Scripts/FlashingText.cs`
A clean script that modulates the text color between black and white using `Mathf.Sin` and `Color.Lerp` to achieve a smooth, slow flashing effect.
```csharp
using UnityEngine;
using TMPro;

public class FlashingText : MonoBehaviour
{
    private TMP_Text textMesh;
    [SerializeField] private float speed = 2.5f;

    private void Awake()
    {
        textMesh = GetComponent<TMP_Text>();
    }

    private void Update()
    {
        if (textMesh != null)
        {
            // Calculate a slow sinusoidal oscillation between 0 and 1
            float t = (Mathf.Sin(Time.time * speed) + 1f) / 2f;
            // Smoothly interpolate between black and white
            textMesh.color = Color.Lerp(Color.black, Color.white, t);
        }
    }
}
```

### Scene Modification: `Assets/Scenes/MainMenu.unity`
- **GameObject**: `Canvas/Background/MulaiButton`
  - Change `RectTransform` anchor preset to "Stretch-Stretch" (stretch to fill parent).
  - Set `offsetMin` and `offsetMax` to `(0, 0)` and `anchoredPosition` to `(0, 0)`.
  - Add an `Image` component. Set its color to transparent (`RGBA(0, 0, 0, 0)`) and ensure `raycastTarget` is set to `true`.
- **GameObject**: `Canvas/Background/MulaiButton/Text (TMP)`
  - Ensure its `RectTransform` `anchoredPosition` is set to `(0, -430.0)` so it remains in its exact visual position relative to the center of the screen.
  - Set its anchors to `(0.5, 0.5)` to anchor it at the screen bottom-center.
  - Attach the new `FlashingText` script.

# Implementation Steps
### Step 1: Create the Flashing Text Script
- **Description**: Create `Assets/Scripts/FlashingText.cs` with the visual interpolation logic.
- **Assigned role**: developer
- **Dependencies**: None
- **Parallelizable**: Yes

### Step 2: Open and Configure MainMenu Scene UI
- **Description**: Open `MainMenu.unity`. Select `MulaiButton` and add a transparent `Image` component. Update its `RectTransform` to stretch across the entire screen.
- **Assigned role**: developer
- **Dependencies**: None
- **Parallelizable**: No

### Step 3: Reposition and Setup Text Component
- **Description**: Adjust `Text (TMP)`'s RectTransform so that it continues to be displayed in its original position at the bottom of the screen (`anchoredPosition = (0, -430)`). Attach the `FlashingText` component to it.
- **Assigned role**: developer
- **Dependencies**: Step 1, Step 2
- **Parallelizable**: No

### Step 4: Save and Commit Scene Changes
- **Description**: Save the modified scene and project assets.
- **Assigned role**: developer
- **Dependencies**: Step 3
- **Parallelizable**: No

# Verification & Testing
### Verification Steps
1. **Visual Test**: Play the `MainMenu` scene and verify that the "Tekan untuk main..." text smoothly and slowly flashes between black and white.
2. **Press Anywhere Test**: Click on empty/blank spaces of the main menu screen. Verify that clicking anywhere triggers the opening of the Level Panel.
3. **Button Hierarchy Verification**: Click on `SettingButton ` or `CreditButton`. Verify that they are still clickable and correctly open their respective panels instead of triggering the Level Panel.
4. **Blocking Test**: With any panel active (e.g., Settings, Level, or Credits Panel), click outside the panel on the background. Verify that clicking does NOT trigger the `MulaiButton` again (clicks are properly blocked by the active panels' raycast targets).
