using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Image))] // Works with UI Image
public class CustomerDropZone : MonoBehaviour, IDropHandler
{
    [Header("References")]
    public CustomerManager customerManager;

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
        {
            originalColor = customerImage.color;
        }
        else
        {
            Debug.LogError("CustomerDropZone requires an Image component!");
        }

        // Null check for critical reference
        if (customerManager == null)
        {
            customerManager = FindAnyObjectByType<CustomerManager>();
            if (customerManager == null)
            {
                Debug.LogError("CustomerManager reference is missing!");
            }
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        // 1. Null check for the dragged object
        if (eventData.pointerDrag == null)
        {
            Debug.LogWarning("Dropped object is null");
            return;
        }

        // 2. Get the DragCookedFood component
        DragCookedFood draggedFood = eventData.pointerDrag.GetComponent<DragCookedFood>();
        if (draggedFood == null)
        {
            Debug.LogWarning("Dropped object has no DragCookedFood component");
            return;
        }

        // 3. Null check for customerManager
        if (customerManager == null)
        {
            Debug.LogError("CustomerManager reference is missing!");
            return;
        }

        // 4. Check if the order is correct
        bool isCorrectOrder = customerManager.CheckOrder(draggedFood.foodName);

        if (isCorrectOrder)
        {
            // Success - serve the food
            draggedFood.ServeToCustomer(transform);
            customerManager.OnFoodServed(draggedFood.foodName);
            StartCoroutine(FlashFeedback(successColor));
        }
        else
        {
            // Wrong order - return food
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