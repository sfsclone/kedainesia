# Kedainesia - Technical Scripts Documentation

This document provides a deep-dive technical explanation of all C# scripts in the **Kedainesia** project. It details the system architecture, component relationships, data flow, and how each script contributes to the game loop.

---

## 1. Architectural Overview & Design Patterns

Kedainesia utilizes a **Manager-Centric Architecture** with a hybrid of **Data-Driven Design** and **Direct Reference Mapping**.

### Key Architectural Patterns:
1. **Dynamic Service Location (`FindAnyObjectByType`)**:
   Instead of strict singletons or heavy dependency injection frameworks, managers and controllers find each other dynamically at runtime using Unity's `FindAnyObjectByType<T>()`. This decouples prefab instantiation from hardcoded scene hierarchy structures.
2. **Data-Driven Content (ScriptableObjects)**:
   All recipes (`RecipeData`) and customer profiles (`CustomerData`) are separated from the logic. Adding new recipes, ingredients, or customers requires *zero* code changes; it is done purely by creating assets in the Inspector.
3. **Event-Driven Pointer Interactions**:
   The drag-and-drop systems implement Unity's `EventSystems` interfaces (`IBeginDragHandler`, `IDragHandler`, `IEndDragHandler`, `IDropHandler`), leveraging Canvas Raycasting to trigger complex gameplay state changes (like cooking validation and food serving).

---

## 2. The Core Gameplay Data Flow

To understand the scripts, it is easiest to follow the lifecycle of a single gameplay loop:

```
[Main Menu] ---> [GameManager] ---> Starts Day ---> [GameClock] (Open Restaurant)
                                                           |
                                                           v
[WarningSystem] <--- (Incorrect Order / Impatient) <--- [CustomerManager] (Spawns Prefabs)
      |                                                    |
  Trigger GameOver                                         v
                                                    [CustomerController] (Tracks Patience Timer)
                                                           |
                                                           v
[DragCookedFood] (Dragged onto Customer) <--- [CraftingManager] (Cooks Recipe) <--- [DragIngredient]
```

---

## 3. Script Reference Index by System

### Category A: Core Game Management
*   **`GameManager.cs`**: The central state machine of the scene. Tracks days, handles day advancement, manages persistent saving via `PlayerPrefs`, and triggers game-over resets.
*   **`GameClock.cs`**: Controls the level timer (09:00 to 17:00), handles UI clock hands/sliders, and triggers early-close conditions if all customers are served.
*   **`WarningSystem.cs`**: Tracks player mistakes. Manages the 3-strike policy before triggering the Game Over UI.

### Category B: Cooking & Recipe System
*   **`CraftingManager.cs`**: The brain of the kitchen. Manages stove UI panels, loads food selection buttons, spawns ingredient items, runs the asynchronous cooking timer, and spawns the physical cooked dish.
*   **`IngredientSlot.cs`**: Attached to the 3 cooking pot input slots. Receives incoming dragged ingredients and registers them in the `CraftingManager`.
*   **`RecipeData.cs`**: ScriptableObject defining the recipe name, icon, and the specific `requiredIngredients` string list.

### Category C: Customer & Order System
*   **`CustomerManager.cs`**: Dynamically generates unique customer schedules based on the current day, instantiates customer prefabs, tracks served count, and validates incoming food deliveries.
*   **`CustomerController.cs`**: Handles individual customer state machine: waiting to be accepted, waiting to be served, and departing (unhappy on timeout or happy on service).
*   **`CustomerData.cs`**: ScriptableObject defining a customer's visual sprite, name, and the pool of potential recipes they might order.
*   **`CustomerDropZone.cs`**: Handles the physical drop interface on the customer. Detects incoming cooked food, validates correctness, and relays the outcome to the managers.

### Category D: Drag-And-Drop Interaction Layer
*   **`DragIngredient.cs`**: Placed on raw ingredients. Allows them to be dragged from the shelf to the stove slots. Includes logic to automatically move items to a high-priority `DragLayer` to prevent visual clipping behind UI panels.
*   **`DragCookedFood.cs`**: Placed on finished dishes on the plate. Allows them to be dragged to the customer. Cleans up and destroys itself on a successful drop.

### Category E: UI & Game Polish
*   **`RectractableUI.cs` / `RetractableUISlide.cs`**: Controls UI sliding/animation effects (e.g., Recipe Book, Stove panel).
*   **`AudioManager.cs`**: Central audio hub managing background music, sound effects, and music-ducking during key events.
*   **`MainMenuManager.cs` / `LevelLockManager.cs` / `PauseMenu.cs`**: Standard navigation and platform flow utilities.

---

## 4. Deep Dive: Core Script Breakdown

### 4.1 `GameManager.cs` (Global Orchestrator)
*   **Purpose**: Controls day-by-day progression and coordinates scene-wide resets.
*   **Key Functions**:
    *   `AdvanceDay()`: Increments `currentDay`, saves the unlock progress to `PlayerPrefs`, resets the `GameClock`, clears any warning strikes, and instructs `CustomerManager` to build the new day's layout.
    *   `RestartDay()`: Invoked by the `WarningSystem` on loss. Clears the board and regenerates the current day's level setup exactly.
*   **Testing Utilities**: Includes an editor-only shortcut (`Update()` with keyboard `N` key) to instantly skip days for fast testing.

### 4.2 `CustomerManager.cs` & `CustomerController.cs` (Customer Life Cycle)
*   **The Randomization Logic (`GenerateTodaysCustomers`)**:
    *   Takes the base `allCustomerPool`.
    *   Determines customer count using difficulty scaling: `Mathf.Min(2 + day, 10)`.
    *   Shuffles the pool and dynamically clones customer data using `ScriptableObject.Instantiate()`.
    *   Forces *unique orders* per customer by tracking assigned dishes in a `HashSet<string> usedRecipes`.
*   **Patience Mechanics**:
    *   Divided into **Pre-Accept** (15s) and **Post-Accept** (20s) timers.
    *   A slider UI updates in `Update()`. If the timer hits the warning threshold, a warning SFX ducks the music.
    *   On expiration (`LeaveUnhappy`), it triggers `WarningSystem.AddWarning()` and flashes the customer sprite red before spawning the next customer.

### 4.3 `CraftingManager.cs` (Ingredient Processing & Validation)
*   **Ingredient Matching**:
    *   Rather than rigid positional matching, ingredients are validated using a `HashSet<string>`.
    *   ```csharp
        List<string> required = new List<string>(recipe.requiredIngredients);
        List<string> input = new List<string>(currentIngredients);
        input.RemoveAll(i => string.IsNullOrEmpty(i));

        cookButton.interactable = input.Count == 3 && 
                                  new HashSet<string>(input).SetEquals(required) && 
                                  !isFoodOnPlate;
        ```
    *   This elegant set-comparison allows the player to drag the 3 correct ingredients into *any slot index in any order* and still have it validate successfully.
*   **Cooking Process**:
    *   Upon clicking "Cook", a coroutine (`CookingProcess`) runs a 3-second timer, updates a cooking progress slider, resets the raw ingredient slots, and instances the completed `cookedFoodPrefab`.

### 4.4 Drag and Drop Mechanics (`DragIngredient.cs` & `DragCookedFood.cs`)
*   **The `DragLayer` System**:
    *   When a drag begins, the object is detached from its layout group/parent panel and reparented to a top-level hierarchy object named `DragLayer` or the canvas root.
    *   This resolves the classic uGUI issue where elements being dragged render *underneath* other adjacent UI panels.
    *   If dropped on an invalid target, the script handles returning the object back to its original parent.
*   **Input Blocking**:
    *   Both scripts toggle a `CanvasGroup.blocksRaycasts` variable. It is set to `false` during dragging so that underlying drop zones (like slots or the customer) can detect pointer hover and drop events.

---

## 5. Implementation Gotchas & Development Rules

1.  **String-Based Matching**:
    *   The `CraftingManager` uses exact string comparison to validate ingredients. The string `ingredientName` on a raw `DragIngredient` script must **exactly match** the spelling and casing of the entries in the `requiredIngredients` list inside the `RecipeData` ScriptableObject.
2.  **Plate State Dependency**:
    *   `isFoodOnPlate` is a critical state flag. If a player cooks food, it occupies the plate. The stove **will lock cooking inputs** until that cooked food is either successfully served to a customer or manually thrown away using the `ClearCookedFood()` method (linked to the "Clear Plate" UI or a trash zone).
3.  **FindAnyObjectByType Performance**:
    *   Because references like `CustomerManager` and `CraftingManager` are resolved dynamically via `FindAnyObjectByType`, ensure there is **only one instance** of these components active in the hierarchy at any time.

---

## 6. How to Extend the Game

### Adding a New Ingredient & Recipe
1.  **Register the Ingredient**:
    *   Open the `CraftingManager` in the Inspector.
    *   Add your new ingredient string to the `allIngredients` list.
    *   Add an entry inside `ingredientSpriteList` mapping the name of your ingredient to its sprite.
2.  **Create the Recipe Asset**:
    *   Right-click in Project Window -> `Create -> Kedainesia -> RecipeData` (or duplicate an existing asset in `Assets/Data/Recipes/`).
    *   Set the `recipeName`, assign a `foodIcon`.
    *   Add exactly **3** ingredient name strings to the `requiredIngredients` list.
3.  **Add to the GameManager**:
    *   Select `CraftingManager` in the scene. Drag the newly created `RecipeData` asset into the `allRecipes` list. It will now automatically populate the food buttons on startup!
