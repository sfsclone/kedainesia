using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class TrashDropZone : MonoBehaviour, IDropHandler
{
    private CraftingManager craftingManager;

    private void Awake()
    {
        craftingManager = FindAnyObjectByType<CraftingManager>();
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null) return;

        DragCookedFood draggedFood = eventData.pointerDrag.GetComponent<DragCookedFood>();
        if (draggedFood != null)
        {
            // Destroy the food object immediately since it's on the canvas now
            Destroy(draggedFood.gameObject);

            if (craftingManager != null)
            {
                // Reset CraftingManager state
                craftingManager.ClearCookedFood();
                Debug.Log("Food thrown into trash.");
            }
        }
    }
}
