using UnityEngine;

public class RectractableUI : MonoBehaviour
{
    [SerializeField] private GameObject panelContent;

    private bool isVisible = true;

    public void Toogle()
    {
        isVisible = !isVisible;
        panelContent.SetActive(isVisible);
    }
}
