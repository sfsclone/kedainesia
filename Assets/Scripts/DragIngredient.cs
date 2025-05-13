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

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        ingredientListParent = GameObject.Find("IngredientIconContainer").transform; // Adjust name if needed
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;

        // Move to DragLayer
        Transform dragLayer = GameObject.Find("DragLayer").transform;
        transform.SetParent(dragLayer);

        if (label != null)
            label.gameObject.SetActive(false);

        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        bool droppedOnSlot = eventData.pointerEnter != null &&
                             eventData.pointerEnter.GetComponent<IngredientSlot>() != null;

        if (droppedOnSlot)
        {
            transform.SetParent(eventData.pointerEnter.transform);
        }
        else
        {
            // Return to scroll list
            transform.SetParent(ingredientListParent);
        }

        transform.localPosition = Vector3.zero;

        if (label != null)
            label.gameObject.SetActive(true);

        canvasGroup.blocksRaycasts = true;
    }
}
