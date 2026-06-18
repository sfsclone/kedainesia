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
    public Button clearPlateButton;

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
    public AudioClip cookSFX;
    public AudioClip foodAppearSFX;
    public Transform cookedFoodParent;
    public GameObject cookedFoodPrefab;
    public Canvas canvas;

    private Coroutine cookingRoutine;
    private string selectedFood;
    public CustomerManager customerManager;

    private bool isFoodOnPlate = false;

    private void Start()
    {
        if (stoveButton != null) stoveButton.onClick.AddListener(OpenCraftingPanel);
        clearPlateButton.onClick.AddListener(ClearCookedFood);
        clearPlateButton.gameObject.SetActive(false);

        foreach (var entry in ingredientSpriteList)
        {
            if (!ingredientSprites.ContainsKey(entry.ingredientName))
                ingredientSprites[entry.ingredientName] = entry.icon;
        }

        craftingPanel.SetActive(false);
        foodSelectionPanel.SetActive(true);
        ingredientInputPanel.SetActive(false);
        cookButton.interactable = false;

        SpawnAllIngredientIcons();

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
        selectedFoodText.text = "Select Food";
        selectedFood = "";
        LoadFoodButtons();
        SpawnAllIngredientIcons();
    }

    public void CloseCraftingPanel()
    {
        craftingPanel.SetActive(false);
        foodSelectionPanel.SetActive(false);
        ingredientInputPanel.SetActive(false);
    }

    public void SelectFood(string foodName)
    {
        selectedFood = foodName;
        selectedFoodText.text = foodName;

        ClearIngredientSlotsUI();
        ClearIngredientSlotObjects();
        ValidateIngredients();

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

        cookButton.interactable = input.Count == 3 &&
                                  new HashSet<string>(input).SetEquals(required) &&
                                  !isFoodOnPlate;
    }

    private void ClearIngredientSlotsUI()
    {
        for (int i = 0; i < currentIngredients.Length; i++)
        {
            currentIngredients[i] = "";

            Transform slot = ingredientInputPanel.transform.Find($"IngredientSlot{i + 1}");
            if (slot != null)
            {
                Image iconImage = slot.Find("Icon")?.GetComponent<Image>();
                if (iconImage != null)
                {
                    iconImage.sprite = null;
                    iconImage.color = new Color(1, 1, 1, 0);
                }
            }
        }
    }

    private void ClearIngredientSlotObjects()
    {
        for (int i = 0; i < 3; i++)
        {
            Transform slot = ingredientInputPanel.transform.Find($"IngredientSlot{i + 1}");
            if (slot != null)
            {
                foreach (Transform child in slot)
                {
                    if (child.name != "Icon" && child.name != "Label")
                    {
                        Destroy(child.gameObject);
                    }
                }
            }
        }
    }

    private void LoadFoodButtons()
    {
        foreach (Transform child in foodButtonParent)
            Destroy(child.gameObject);

        foreach (RecipeData recipe in allRecipes)
        {
            GameObject button = Instantiate(foodButtonPrefab, foodButtonParent);
            TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();
            if (buttonText != null)
                buttonText.text = recipe.recipeName;

            Transform iconTransform = button.transform.Find("Icon");
            if (iconTransform != null)
            {
                Image iconImage = iconTransform.GetComponent<Image>();
                if (iconImage != null && recipe.foodIcon != null)
                {
                    iconImage.sprite = recipe.foodIcon;
                    iconImage.preserveAspect = true;
                }
            }

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

            Image iconImage = icon.transform.Find("Icon")?.GetComponent<Image>();
            if (iconImage != null && ingredientSprites.ContainsKey(ingredient))
                iconImage.sprite = ingredientSprites[ingredient];

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
        if (isFoodOnPlate)
        {
            Debug.Log("Please clear the plate before cooking new food.");
            return;
        }

        if (AudioManager.Instance != null && cookSFX != null)
        {
            AudioManager.Instance.PlaySFX(cookSFX, true); // Added duckMusic: true
        }

        if (cookingRoutine != null)
            StopCoroutine(cookingRoutine);
        cookingRoutine = StartCoroutine(CookingProcess());
    }

    private IEnumerator CookingProcess()
    {
        cookingSlider.gameObject.SetActive(true);
        cookingSlider.value = 0f;

        float duration = 3f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            cookingSlider.value = elapsed / duration;
            yield return null;
        }

        cookingSlider.gameObject.SetActive(false);
        craftingPanel.SetActive(false);

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopSFX();
        }

        SpawnCookedFood();

        // Reset ingredient slots and UI
        currentIngredients = new string[3];
        ClearIngredientSlotsUI();
        ClearIngredientSlotObjects();
        cookButton.interactable = false;

        // Go back to food selection to allow a new recipe
        ingredientInputPanel.SetActive(false);
        foodSelectionPanel.SetActive(true);
        selectedFoodText.text = "Select Food";
    }

    private void SpawnCookedFood()
    {
        RecipeData cookedRecipe = allRecipes.Find(r => r.recipeName == selectedFood);
        if (cookedRecipe == null || cookedFoodPrefab == null || cookedFoodParent == null)
        {
            Debug.LogError("Missing references for spawning cooked food");
            return;
        }

        if (AudioManager.Instance != null && foodAppearSFX != null)
        {
            AudioManager.Instance.PlaySFX(foodAppearSFX);
        }

        foreach (Transform child in cookedFoodParent)
            Destroy(child.gameObject);

        GameObject cookedFood = Instantiate(cookedFoodPrefab, cookedFoodParent);
        cookedFood.transform.localPosition = Vector3.zero;

        Image foodImage = cookedFood.GetComponent<Image>();
        if (foodImage != null)
        {
            foodImage.sprite = cookedRecipe.foodIcon;
            foodImage.preserveAspect = true;
        }

        DragCookedFood dragScript = cookedFood.GetComponent<DragCookedFood>();
        if (dragScript == null)
            dragScript = cookedFood.AddComponent<DragCookedFood>();

        dragScript.foodName = cookedRecipe.recipeName;
        if (dragScript.canvas == null)
            dragScript.canvas = this.canvas;

        isFoodOnPlate = true;
        clearPlateButton.gameObject.SetActive(true);
        cookButton.interactable = false;
    }

    public void ClearCookedFood()
    {
        foreach (Transform child in cookedFoodParent)
            Destroy(child.gameObject);

        isFoodOnPlate = false;
        clearPlateButton.gameObject.SetActive(false);
        ValidateIngredients();
    }

    [System.Serializable]
    public class IngredientSprite
    {
        public string ingredientName;
        public Sprite icon;
    }
}
