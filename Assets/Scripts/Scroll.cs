using UnityEngine;
using UnityEngine.UI;

public class ScrollToTopOnEnable : MonoBehaviour
{
    private ScrollRect scrollRect;

    private void Awake()
    {
        scrollRect = GetComponent<ScrollRect>();
    }

    private void OnEnable()
    {
        // Snap to top
        Canvas.ForceUpdateCanvases(); // Important to refresh layout before setting
        scrollRect.verticalNormalizedPosition = 1f;
    }
}
