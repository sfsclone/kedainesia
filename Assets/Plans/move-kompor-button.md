# Project Overview
- Game Title: Kedainesia
- High-Level Concept: 2D restaurant management and cooking simulation focused on traditional Indonesian cuisine.
- Players: Single player
- Inspiration / Reference Games: Diner Dash, Cooking Mama
- Tone / Art Direction: 2D, vibrant Indonesian culture
- Target Platform: Android
- Screen Orientation / Resolution: Landscape
- Render Pipeline: URP

# Game Mechanics
## Core Gameplay Loop
Players receive orders from customers, prepare them using a crafting system (stove), and serve them within a time limit.
## Controls and Input Methods
Touch/Mouse interactions: Drag and drop ingredients, click buttons to open panels and accept orders.

# UI
The "KomporButton" (Stove Button) currently exists as a static button on the Canvas. It will be moved to the `CustomerPref` and only become visible after an order is accepted.

# Key Asset & Context
- `Assets/Scripts/CustomerController.cs`: Manages customer state and order acceptance.
- `Assets/Prefabs/CustomerPref.prefab`: The prefab for spawned customers.
- `Assets/Scripts/CraftingManager.cs`: Manages the crafting/stove logic.
- `Assets/Scenes/GameScene1.unity`: The main gameplay scene.
- `Assets/ASSET FANTEAM KEDAINESIA/ui/kompor.png`: The sprite used for the stove button.

# Implementation Steps
## 1. Update CustomerController Script
- **Description**: Add logic to handle the `komporButton` visibility and interaction.
  - Add `public GameObject komporButton;` field.
  - Add `private CraftingManager craftingManager;` field.
  - In `Start()`, initialize `craftingManager` using `FindAnyObjectByType<CraftingManager>()` and ensure `komporButton` is disabled.
  - In `OnAcceptOrder()`, set `komporButton.SetActive(true)`.
  - Add a public method `OpenStove()` that calls `craftingManager.OpenCraftingPanel()`.
- **Assigned role**: developer
- **Dependencies**: None
- **Parallelizable**: Yes

## 2. Update CraftingManager Script
- **Description**: Add a null check for `stoveButton` in `Start()` to prevent errors when the global button is removed.
- **Assigned role**: developer
- **Dependencies**: None
- **Parallelizable**: Yes

## 3. Configure CustomerPref Prefab
- **Description**: Add the `KomporButton` to the customer prefab.
  - Open `Assets/Prefabs/CustomerPref.prefab`.
  - Create a new `Button` (GameObject -> UI -> Button) named `KomporButton`.
  - Set its `RectTransform` position to match the existing interaction area (e.g., near `acceptOrderButton` or inside `bubble`).
  - Set the `Image` sprite to `kompor_0` (`Assets/ASSET FANTEAM KEDAINESIA/ui/kompor.png`).
  - Set the `Button`'s `On Click()` event to call `CustomerController.OpenStove`.
  - Assign the `KomporButton` GameObject to the `komporButton` field on the `CustomerController` component.
- **Assigned role**: developer
- **Dependencies**: Step 1
- **Parallelizable**: No

## 4. Clean up GameScene1
- **Description**: Remove the now-obsolete global `KomporButton`.
  - Open `Assets/Scenes/GameScene1.unity`.
  - Delete `Canvas/KomporButton`.
- **Assigned role**: developer
- **Dependencies**: Step 3
- **Parallelizable**: No

# Verification & Testing
- **Manual Check**: Start the game and observe that no stove button is visible initially.
- **Manual Check**: Wait for a customer to arrive and accept their order. Verify that the `KomporButton` appears on the customer.
- **Manual Check**: Click the `KomporButton` and verify it opens the crafting panel correctly.
- **Manual Check**: Serve the customer and verify that the button disappears (along with the customer).
