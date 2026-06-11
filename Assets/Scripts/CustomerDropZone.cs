using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Image))]
public class CustomerDropZone : MonoBehaviour, IDropHandler
{
    [Header("References")]
    public CustomerManager customerManager;
    private CraftingManager craftingManager; // cached reference

    [Header("Feedback")]
    public float flashDuration = 0.3f;
    public Color successColor = Color.green;
    public Color failColor = Color.red;

    private Image customerImage;
    private Color originalColor;

    private void Awake()
    {
        customerImage = GetComponent<Image>();
        if (customerImage != null)
            originalColor = customerImage.color;
        else
            Debug.LogError("CustomerDropZone requires an Image component!");

        // Find managers once at start (only if not assigned in Inspector)
        if (customerManager == null)
        {
            customerManager = FindAnyObjectByType<CustomerManager>();
            if (customerManager == null)
                Debug.LogError("CustomerManager reference is missing!");
        }

        if (craftingManager == null)
        {
            craftingManager = FindAnyObjectByType<CraftingManager>();
            if (craftingManager == null)
                Debug.LogError("CraftingManager reference is missing!");
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null)
        {
            Debug.LogWarning("Dropped object is null");
            return;
        }

        DragCookedFood draggedFood = eventData.pointerDrag.GetComponent<DragCookedFood>();
        if (draggedFood == null)
        {
            Debug.LogWarning("Dropped object has no DragCookedFood component");
            return;
        }

        if (customerManager == null)
        {
            Debug.LogError("CustomerManager reference is missing!");
            return;
        }

        bool isCorrectOrder = customerManager.CheckOrder(draggedFood.foodName);

        if (isCorrectOrder)
        {
            // Serve the food
            draggedFood.ServeToCustomer(transform);
            customerManager.OnFoodServed(draggedFood.foodName);

            // Clear plate so player can cook again
            if (craftingManager != null)
                craftingManager.ClearCookedFood();
        }
        else
        {
            draggedFood.ReturnToPlate();
            StartCoroutine(FlashFeedback(failColor));
        }
    }

    private IEnumerator FlashFeedback(Color flashColor)
    {
        if (customerImage == null) yield break;

        customerImage.color = flashColor;
        yield return new WaitForSeconds(flashDuration);
        customerImage.color = originalColor;
    }
}
