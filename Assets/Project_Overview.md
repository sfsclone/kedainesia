# Kedainesia Project Technical Overview

Kedainesia is a 2D management and cooking simulation game developed in Unity for Android. Players run a traditional Indonesian food stall (Kedai), managing customer orders, preparing recipes using specific ingredients, and ensuring timely service to progress through multiple game days.

## 1. Project Description
- **Target Audience:** Casual mobile gamers interested in Indonesian culinary culture and management sims.
- **Core Pillars:**
    - **Authentic Culinary Experience:** Features traditional Indonesian recipes and ingredients (e.g., *Lorjuk*, *Beras Ketan*).
    - **Time Management:** Players must balance preparation time against customer patience.
    - **Progression:** A day-based system with increasing difficulty and customer variety.

## 2. Gameplay Flow / User Loop
1.  **Preparation (Main Menu):** Player selects a day (Level) to start.
2.  **Opening (Game Scene):** The restaurant starts at 09:00. The player clicks "Open" to start the `GameClock`.
3.  **Customer Intake:** Customers arrive with specific food orders. The player must "Accept" the order to see the recipe details.
4.  **Cooking:** 
    - Open the stove UI.
    - Select the requested food.
    - Drag 3 correct ingredients into slots.
    - Wait for the cooking timer to complete.
5.  **Service:** Drag the finished food from the plate to the customer.
6.  **Resolution:** 
    - **Success:** Customer leaves happy; progress increments.
    - **Failure:** Customer leaves angry if the timer runs out or the wrong food is served, adding a warning.
    - **End of Day:** At 17:00, or when all customers are served, the day ends. 3 warnings result in a "Game Over" for that day.

## 3. Architecture
The project follows a **Manager-Pattern** architecture where centralized managers coordinate specific domain logic. Communication is primarily handled via direct references or `FindAnyObjectByType`.

- **`GameManager`**: The central authority for day progression, win/loss states, and global game data.
- **`CustomerManager`**: Handles spawning logic, customer pool shuffling, and order validation.
- **`CraftingManager`**: Manages the cooking UI, ingredient validation, and the cooking process.
- **`GameClock`**: Drives the session time and triggers end-of-day transitions.

`Location: Assets/Scripts`

## 4. Game Systems & Domain Concepts

### Cooking & Recipe System
A data-driven system using `ScriptableObject` to define recipes. 
- `RecipeData`: Defines `recipeName`, `foodIcon`, and `requiredIngredients`.
- `CraftingManager`: Validates if the 3 dragged ingredients match the `RecipeData`.
- **Extension:** To add a new food, create a new `RecipeData` asset and add it to the `allRecipes` list in the `CraftingManager` prefab.

`Location: Assets/Scripts (RecipeData.cs, CraftingManager.cs)`

### Customer & Patience System
Manages customer behavior and time constraints.
- `CustomerData`: Stores customer visuals and a list of `possibleRecipes`.
- `CustomerController`: Handles the two-stage patience timer (Pre-acceptance and Post-acceptance) and emoji feedback.
- **Extension:** Create new `CustomerData` assets to vary customer visuals or their preferred menu items.

`Location: Assets/Scripts (CustomerData.cs, CustomerController.cs, CustomerManager.cs)`

### Warning & Health System
The "Health" of a session is represented by warnings.
- `WarningSystem`: Tracks incorrect orders or timed-out customers. 
- Reaching `maxWarnings` (default 3) triggers the `gameOverPanel`.

`Location: Assets/Scripts (WarningSystem.cs)`

## 5. Scene Overview
- **`MainMenu`**: Initial entry point. Handles day selection and `PlayerPrefs` for unlocking levels.
- **`GameScene1`**: The primary gameplay scene where the restaurant simulation occurs.
- **`testing`**: A sandbox scene for verifying mechanics without progression constraints.

`Location: Assets/Scenes`

## 6. UI System
The game uses **Unity UI (UGUI)** with a heavy emphasis on **Drag and Drop** interfaces.
- **Interactions:** Implemented via `IBeginDragHandler`, `IDragHandler`, and `IEndDragHandler` in `DragIngredient` and `DragCookedFood`.
- **Feedback:** `WarningSystem` and `GameClock` use `TextMeshPro` for real-time status updates.
- **Navigation:** `PauseMenu` and `SettingsPanel` handle standard game state interruptions.

`Location: Assets/Scripts (DragIngredient.cs, DragCookedFood.cs, RectractableUI.cs)`

## 7. Asset & Data Model
- **Data Storage:** Uses `ScriptableObject` for static data (Recipes, Customers).
- **Persistence:** `PlayerPrefs` is used to store `HighestUnlockedDay`.
- **Organization:**
    - `Assets/Data/Recipes`: `RecipeData` assets.
    - `Assets/Data/Customer`: `CustomerData` assets.
    - `Assets/Prefabs`: Reusable UI elements and game objects (e.g., `CustomerPref`, `FoodButtonPref`).
    - `Assets/Settings`: Contains **Sprite Atlases** (`FoodAtlas`, `IngredientsAtlas`) to optimize draw calls for mobile.

## 8. Notes, Caveats & Gotchas
- **UI Drag Layer:** Ingredients and food are moved to a specific "DragLayer" in the hierarchy during dragging to ensure they appear above all other UI elements.
- **Customer Shuffling:** `CustomerManager` shuffles the pool every morning and filters recipes to ensure unique orders per session.
- **Mobile Optimization:** The project uses the **Universal Render Pipeline (URP)** and Sprite Atlases, critical for maintaining performance on Android devices.
- **Ingredient Matching:** The `ValidateIngredients` method in `CraftingManager` uses a `HashSet.SetEquals` check, meaning the order of ingredients in the 3 slots does not matter, only the content.