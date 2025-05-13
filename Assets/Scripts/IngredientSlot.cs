using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class IngredientSlot : MonoBehaviour, IDropHandler
{
    public int slotIndex; // 0, 1, 2 for 3 input slots

    public void OnDrop(PointerEventData eventData)
    {
        // Prevent dropping if the slot already has an ingredient
        if (transform.childCount > 0)
            return;

        DragIngredient dragged = eventData.pointerDrag?.GetComponent<DragIngredient>();
        if (dragged != null)
        {
            // Move to this slot
            dragged.transform.SetParent(transform);
            dragged.transform.localPosition = Vector3.zero;

            // Optional: Hide the label (you can comment this out if you want to show it)
            if (dragged.label != null)
                dragged.label.gameObject.SetActive(false);

            // Notify CraftingManager
            CraftingManager crafting = FindAnyObjectByType<CraftingManager>();
            if (crafting != null)
            {
                crafting.SetIngredient(slotIndex, dragged.ingredientName);
            }
        }
    }
}
