using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CraftingManager : MonoBehaviour
{
    [Header("Recipe Setup")]
    public List<RecipeData> allRecipes;
    public TMP_Text selectedFoodText;

    [Header("UI Panels")]
    public GameObject craftingPanel;
    public GameObject foodSelectionPanel;
    public GameObject ingredientInputPanel;
    public Slider cookingSlider;

    [Header("Food Selection UI")]
    public Transform foodButtonParent;
    public GameObject foodButtonPrefab;

    [Header("Buttons")]
    public Button stoveButton;
    public Button cookButton;

    [Header("Ingredient Input")]
    public string[] currentIngredients = new string[3];

    [Header("Ingredient Icons")]
    public List<string> allIngredients;
    public GameObject ingredientIconPrefab;
    public Transform ingredientIconContainer;

    [Header("Ingredient Sprites")]
    public List<IngredientSprite> ingredientSpriteList = new List<IngredientSprite>();
    public Dictionary<string, Sprite> ingredientSprites = new Dictionary<string, Sprite>();

    [Header("Cooked Food UI")]
    public Transform cookedFoodParent; // This should be your plate's transform
    public GameObject cookedFoodPrefab;
    public Canvas canvas; // Reference to your main canvas

    private Coroutine cookingRoutine;
    private string selectedFood;
    public CustomerManager customerManager;

    private void Start()
    {
        stoveButton.onClick.AddListener(OpenCraftingPanel);

        // Initialize ingredient sprites dictionary
        foreach (var entry in ingredientSpriteList)
        {
            if (!ingredientSprites.ContainsKey(entry.ingredientName))
                ingredientSprites[entry.ingredientName] = entry.icon;
        }

        // Set up UI state
        craftingPanel.SetActive(false);
        foodSelectionPanel.SetActive(true);
        ingredientInputPanel.SetActive(false);
        cookButton.interactable = false;

        SpawnAllIngredientIcons();

        // Find canvas if not assigned
        if (canvas == null)
        {
            canvas = GetComponentInParent<Canvas>();
            if (canvas == null)
            {
                canvas = FindAnyObjectByType<Canvas>();
                Debug.LogWarning("Canvas reference not set in CraftingManager. Found one automatically.");
            }
        }
    }

    public void OpenCraftingPanel()
    {
        craftingPanel.SetActive(true);
        foodSelectionPanel.SetActive(true);
        ingredientInputPanel.SetActive(false);
        selectedFood = "";
        LoadFoodButtons();
    }

    public void SelectFood(string foodName)
    {
        selectedFood = foodName;
        selectedFoodText.text = foodName;

        // Reset ingredients
        for (int i = 0; i < currentIngredients.Length; i++)
            currentIngredients[i] = "";

        cookButton.interactable = false;

        // Switch panels
        foodSelectionPanel.SetActive(false);
        ingredientInputPanel.SetActive(true);
    }

    public void SetIngredient(int index, string ingredient)
    {
        currentIngredients[index] = ingredient;
        ValidateIngredients();
    }

    private void ValidateIngredients()
    {
        RecipeData recipe = allRecipes.Find(r => r.recipeName == selectedFood);
        if (recipe == null) return;

        List<string> required = new List<string>(recipe.requiredIngredients);
        List<string> input = new List<string>(currentIngredients);
        input.RemoveAll(i => string.IsNullOrEmpty(i));

        if (input.Count == 3 && new HashSet<string>(input).SetEquals(required))
            cookButton.interactable = true;
        else
            cookButton.interactable = false;
    }

    private void LoadFoodButtons()
    {
        // Clear old buttons
        foreach (Transform child in foodButtonParent)
            Destroy(child.gameObject);

        // Create new buttons
        foreach (RecipeData recipe in allRecipes)
        {
            GameObject button = Instantiate(foodButtonPrefab, foodButtonParent);
            TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();

            if (buttonText != null)
                buttonText.text = recipe.recipeName;

            button.GetComponent<Button>().onClick.AddListener(() => SelectFood(recipe.recipeName));
        }
    }

    public void SpawnAllIngredientIcons()
    {
        foreach (Transform child in ingredientIconContainer)
            Destroy(child.gameObject);

        foreach (string ingredient in allIngredients)
        {
            GameObject icon = Instantiate(ingredientIconPrefab, ingredientIconContainer);
            DragIngredient dragScript = icon.GetComponent<DragIngredient>();
            dragScript.ingredientName = ingredient;

            // Set icon sprite
            Image iconImage = icon.transform.Find("Icon")?.GetComponent<Image>();
            if (iconImage != null && ingredientSprites.ContainsKey(ingredient))
                iconImage.sprite = ingredientSprites[ingredient];

            // Set text label
            TMP_Text tmpText = icon.transform.Find("Label")?.GetComponent<TMP_Text>();
            if (tmpText != null)
                tmpText.text = ingredient;
        }
    }
    public void ClearIngredient(int index)
    {
        if (index >= 0 && index < currentIngredients.Length)
        {
            currentIngredients[index] = "";
            ValidateIngredients();
        }
    }


    public void CookSelectedFood()
    {
        if (cookingRoutine != null)
            StopCoroutine(cookingRoutine);
        cookingRoutine = StartCoroutine(CookingProcess());
    }

    private IEnumerator CookingProcess()
    {
        cookingSlider.gameObject.SetActive(true);
        cookingSlider.value = 0f;

        float duration = 3f; // Cooking duration
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            cookingSlider.value = elapsed / duration;
            yield return null;
        }

        cookingSlider.gameObject.SetActive(false);
        craftingPanel.SetActive(false);

        // Spawn the cooked food before resetting selectedFood
        SpawnCookedFood();

        // Reset state after spawning
        selectedFood = "";
        currentIngredients = new string[3];
        cookButton.interactable = false;
    }


    private void SpawnCookedFood()
    {
        RecipeData cookedRecipe = allRecipes.Find(r => r.recipeName == selectedFood);
        if (cookedRecipe == null || cookedFoodPrefab == null || cookedFoodParent == null)
        {
            Debug.LogError("Missing references for spawning cooked food");
            return;
        }

        // Clear previous food if needed
        foreach (Transform child in cookedFoodParent)
            Destroy(child.gameObject);

        // Create new food instance
        GameObject cookedFood = Instantiate(cookedFoodPrefab, cookedFoodParent);
        cookedFood.transform.localPosition = Vector3.zero;

        // Set up food visuals
        Image foodImage = cookedFood.GetComponent<Image>();
        if (foodImage != null)
        {
            foodImage.sprite = cookedRecipe.foodIcon;
            foodImage.preserveAspect = true;
        }

        // Set up drag functionality
        DragCookedFood dragScript = cookedFood.GetComponent<DragCookedFood>();
        if (dragScript == null)
            dragScript = cookedFood.AddComponent<DragCookedFood>();

        dragScript.foodName = cookedRecipe.recipeName;

        // Set canvas reference - this is the corrected version
        if (dragScript.canvas == null)
        {
            dragScript.canvas = this.canvas; // Use the CraftingManager's canvas reference
        }
    }



    [System.Serializable]
    public class IngredientSprite
    {
        public string ingredientName;
        public Sprite icon;
    }
}