using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class DragCookedFood : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public string foodName;
    public Canvas canvas;
    public bool isOnPlate = true;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector3 originalPosition;
    private Transform originalParent;
    private Image image;
    private bool isBeingDragged = false;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        image = GetComponent<Image>();

        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
        }

        // Try to find canvas if not set
        if (canvas == null)
        {
            canvas = GetComponentInParent<Canvas>();
            if (canvas == null)
            {
                canvas = FindAnyObjectByType<Canvas>();
                Debug.LogWarning("Canvas reference not set, found one automatically.");
            }
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!isOnPlate || isBeingDragged) return;

        isBeingDragged = true;
        originalPosition = rectTransform.position;
        originalParent = transform.parent;

        // Visual feedback
        canvasGroup.alpha = 1f; // Keep it fully visible
        canvasGroup.blocksRaycasts = false;

        // Bring to front while dragging
        transform.SetParent(canvas.transform, true);
        transform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isBeingDragged) return;

        // Convert screen position to canvas position
        Vector2 localPoint;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.GetComponent<RectTransform>(),
            eventData.position,
            eventData.pressEventCamera,
            out localPoint))
        {
            rectTransform.localPosition = localPoint;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isBeingDragged) return;
        isBeingDragged = false;

        // Reset visual properties
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        // If not dropped on a valid target
        if (transform.parent == canvas.transform)
        {
            ReturnToPlate();
        }
    }

    public void ReturnToPlate()
    {
        // Reset all drag-related properties
        transform.SetParent(originalParent);
        rectTransform.position = originalPosition;
        canvasGroup.blocksRaycasts = true;
        image.raycastTarget = true;
    }

    public void ServeToCustomer(Transform customerPlate)
    {
        isOnPlate = false;
        transform.SetParent(customerPlate);
        transform.localPosition = Vector3.zero;
        canvasGroup.blocksRaycasts = false;
        image.raycastTarget = false;

        // Optional: Add any serve effects here
    }
}