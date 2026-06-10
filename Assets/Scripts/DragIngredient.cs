using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class DragIngredient : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public string ingredientName;
    public TextMeshProUGUI label;

    private Transform originalParent;
    private CanvasGroup canvasGroup;

    private Transform ingredientListParent; // Reference to scroll list

    private Transform dragLayerTransform;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    private void EnsureReferences()
    {
        if (ingredientListParent == null)
        {
            GameObject container = GameObject.Find("IngredientIconContainer");
            if (container != null) ingredientListParent = container.transform;
        }

        if (dragLayerTransform == null)
        {
            GameObject dragLayerObj = GameObject.Find("DragLayer");
            if (dragLayerObj != null) dragLayerTransform = dragLayerObj.transform;
            else dragLayerTransform = transform.root;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        EnsureReferences();
        originalParent = transform.parent;

        IngredientSlot slot = originalParent.GetComponent<IngredientSlot>();
        if (slot != null)
        {
            slot.ClearSlot();
        }

        // Move to DragLayer
        transform.SetParent(dragLayerTransform);

        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;

        // If the parent is still the DragLayer, it means it wasn't dropped into a valid slot
        if (transform.parent == dragLayerTransform)
        {
            // Return to scroll list
            transform.SetParent(ingredientListParent);
            transform.localPosition = Vector3.zero;
        }
    }
}
