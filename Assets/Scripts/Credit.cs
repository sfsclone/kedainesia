using UnityEngine;

public class OpenPanelButton : MonoBehaviour
{
    [SerializeField] private GameObject panelToOpen;

    public void OpenPanel()
    {
        if (panelToOpen != null)
        {
            panelToOpen.SetActive(true);
        }
    }

    public void ClosePanel()
    {
        if (panelToOpen != null)
        {
            panelToOpen.SetActive(false);
        }
    }
}
