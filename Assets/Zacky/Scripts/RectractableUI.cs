using UnityEngine;
using UnityEngine.UI;

public class RectractableUI : MonoBehaviour
{
    [SerializeField] private GameObject panelContent;
    [SerializeField] private Image buttonImage;
    [SerializeField] private Sprite defaultSprite;
    [SerializeField] private Sprite toggledSprite;

    private bool isVisible = false;

    public void Toogle()
    {
        isVisible = !isVisible;
        panelContent.SetActive(isVisible);

        if (isVisible)
        {
            buttonImage.sprite = defaultSprite;
        }
        else
        {
            buttonImage.sprite = toggledSprite;
        }
    }
}
