This technical documentation provides a comprehensive overview of the **Kedainesia** Unity project, a restaurant management game focused on traditional Indonesian cuisine.

## 1. Project Description
**Kedainesia** is a 2D management and cooking simulation game developed for Android. Players take on the role of a restaurant owner serving traditional Indonesian dishes to various customers. The core pillars of the experience are **Time Management**, **Recipe Accuracy**, and **Cultural Representation**. The game features a progression system spanning 7 in-game days, where difficulty increases as more complex recipes and a higher volume of customers are introduced.

## 2. Gameplay Flow / User Loop
The experience follows a structured daily cycle:
1.  **Boot & Menu**: The player starts at the `MainMenu` and enters the `GameScene1`.
2.  **Preparation**: The player starts each day at 09:00. The day begins only when the player clicks the "Open" button on the `GameClock`.
3.  **Core Loop (Service)**: 
    *   **Customer Spawns**: A customer appears with a specific order (e.g., "Nasi Timbel").
    *   **Accepting Orders**: The player accepts the order, starting a patience timer managed by `CustomerController`.
    *   **Crafting**: The player opens the stove UI, selects the correct recipe, and drags three specific ingredients into slots.
    *   **Cooking**: A progress bar simulates cooking time. Once finished, the food appears on a plate.
    *   **Serving**: The player drags the cooked food onto the customer. Correct orders progress the day; incorrect or slow service leads to warnings.
4.  **End of Day**: The restaurant closes at 17:00 or when all customers are served. Players then advance to the next day, which introduces more customers and potentially more recipes.
5.  **Shutdown/Game Over**: If the player accumulates 3 warnings (failed/slow orders), the day must be restarted. Reaching the end of Day 7 completes the game.

## 3. Architecture
The project follows a **Manager-centric architecture** using the Singleton-like pattern (finding managers via `FindAnyObjectByType`) to coordinate between decoupled systems.

*   **Central Authority**: `GameManager` tracks the current day and global game state (Active, Finished, Restarting).
*   **State Persistence**: Data is primarily stored in `ScriptableObject` assets (`RecipeData`, `CustomerData`), making the system highly data-driven.
*   **Event Handling**: Systems interact via direct method calls (e.g., `CustomerManager` calling `WarningSystem.AddWarning()`) and Unity UI events.
*   **UI-Driven Logic**: Much of the game logic is triggered by UI interactions (Drag and Drop interfaces).

## 4. Game Systems & Domain Concepts

### Cooking & Crafting System
The heart of the game, managing recipe validation and ingredient processing.
*   `CraftingManager`: Handles the UI for selecting recipes and the logic for validating if the three input ingredients match the `RecipeData`.
*   `IngredientSlot`: UI containers that receive `DragIngredient` components.
*   `RecipeData`: A ScriptableObject defining a food item's name, icon, and the specific list of `requiredIngredients`.
*   **Pattern**: Data-driven validation. The manager compares a `HashSet` of strings from the input slots against the recipe's requirement list.
`Location: Assets/Scripts/CraftingManager.cs`

### Customer & Order System
Manages the lifecycle of customers from spawning to departure.
*   `CustomerManager`: Handles the spawning pool for the day and tracks progress (e.g., "3/5 customers served").
*   `CustomerController`: Manages individual customer state, specifically "Patience" timers (pre-accept and post-accept).
*   `CustomerData`: A ScriptableObject containing customer visuals and a list of `possibleRecipes` they can order.
*   `CustomerDropZone`: The physical (UI) area where food is dragged to complete an order.
`Location: Assets/Scripts/CustomerManager.cs`

### Time & Progression System
Controls the flow of a "Work Day" and difficulty scaling.
*   `GameClock`: Simulates time from 09:00 to 17:00. It triggers the start of the customer flow.
*   `GameManager`: Increments the `currentDay` and scales the number of customers per day using the formula `Mathf.Min(2 + day, 10)`.
*   `WarningSystem`: Tracks failures. 3 warnings trigger a game-over/restart state for the current day.
`Location: Assets/Scripts/GameClock.cs`

## 5. Scene Overview
*   **MainMenu**: The entry point. Handles basic navigation and potentially credits.
*   **GameScene1**: The primary gameplay scene containing the restaurant counter, the stove (Crafting UI), and the customer arrival area.
*   **SampleScene/testing**: Developer sandboxes for testing drag-and-drop mechanics and UI layouts.

## 6. UI System
The game uses **Unity UI (uGUI)** with a heavy emphasis on the **Pointer Interaction** interfaces for its drag-and-drop mechanics.
*   **Framework**: Standard `Canvas` with `Image` and `TextMeshPro`.
*   **Interaction Logic**: 
    *   `DragIngredient` / `DragCookedFood`: Implement `IBeginDragHandler`, `IDragHandler`, and `IEndDragHandler` to move items between the inventory and slots/customers.
    *   `RectractableUI`: Used for panels that slide in/out (like the recipe book or crafting panel).
*   **Binding**: References are largely assigned via the Inspector, with some runtime discovery using `GameObject.Find`.

## 7. Asset & Data Model
The project is strictly organized to separate visual assets from logic.
*   **Data Models**:
    *   `RecipeData`: Found in `Assets/Data/Recipes/`. Defines the "win condition" for a dish.
    *   `CustomerData`: Found in `Assets/Data/Customer/`. Defines the "order pool" for specific characters.
*   **Prefabs**:
    *   `CustomerPref`: The visual representation of a customer.
    *   `IngredientIconPref`: The draggable ingredient items.
*   **Naming Conventions**: ScriptableObjects are named by their specific dish or character (e.g., `NasiTimbel.asset`, `LelakiBaduy.asset`).

## 8. Notes, Caveats & Gotchas
*   **Drag Layer**: Draggable items are moved to a specific `DragLayer` transform during movement to ensure they appear above all other UI elements. If this object is missing in the scene, dragging may cause items to disappear behind panels.
*   **Ingredient Matching**: The `CraftingManager` uses string-based matching for ingredients. Spelling in the `RecipeData` list must exactly match the `ingredientName` property on the `DragIngredient` scripts.
*   **Customer Scaling**: The number of customers is hard-capped at 10 per day regardless of the day number.
*   **Early Closure**: If the player serves all customers before 17:00, the `GameClock` will trigger an "Early Close" to prevent the player from waiting idly.