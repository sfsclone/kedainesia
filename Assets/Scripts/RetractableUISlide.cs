using UnityEngine;
using System.Collections;

public class RetractableUISlide : MonoBehaviour
{
    [SerializeField] private RectTransform panelContent; // Panel yang digeser
    [SerializeField] private Vector2 hiddenPosition = new Vector2(-300, 0); // Pos sembunyi
    [SerializeField] private Vector2 shownPosition = new Vector2(0, 0);     // Pos muncul
    [SerializeField] private float duration = 0.3f;                         // Durasi animasi

    private bool isVisible = true;

    public void Toggle()
    {
        StopAllCoroutines();
        if (isVisible)
        {
            StartCoroutine(Slide(panelContent.anchoredPosition, hiddenPosition));
        }
        else
        {
            StartCoroutine(Slide(panelContent.anchoredPosition, shownPosition));
        }
        isVisible = !isVisible;
    }

    private IEnumerator Slide(Vector2 from, Vector2 to)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            panelContent.anchoredPosition = Vector2.Lerp(from, to, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        panelContent.anchoredPosition = to;
    }
}
