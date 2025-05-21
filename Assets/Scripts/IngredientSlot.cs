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

            // Show label when dropped into slot
            if (dragged.label != null)
                dragged.label.gameObject.SetActive(true);

            // Update crafting manager
            CraftingManager crafting = FindAnyObjectByType<CraftingManager>();
            if (crafting != null)
            {
                crafting.SetIngredient(slotIndex, dragged.ingredientName);
            }
        }
    }



    public void ClearSlot()
    {
        CraftingManager crafting = FindAnyObjectByType<CraftingManager>();
        if (crafting != null)
        {
            crafting.ClearIngredient(slotIndex);
        }
    }


    public bool HasIngredient()
    {
        return transform.childCount > 0;
    }

    public string GetIngredientName()
    {
        if (transform.childCount > 0)
        {
            DragIngredient ingredient = transform.GetChild(0).GetComponent<DragIngredient>();
            return ingredient != null ? ingredient.ingredientName : null;
        }
        return null;
    }
}

