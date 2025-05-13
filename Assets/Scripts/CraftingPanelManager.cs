using UnityEngine;

public class CraftingPanelManager : MonoBehaviour
{
    public GameObject craftingPanel;

    void Update()
    {
        if (craftingPanel.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseCraftingPanel();
        }
    }

    public void OpenCraftingPanel()
    {
        craftingPanel.SetActive(true);
    }

    public void CloseCraftingPanel()
    {
        craftingPanel.SetActive(false);
    }
}
