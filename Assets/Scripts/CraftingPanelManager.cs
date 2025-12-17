using UnityEngine;
using UnityEngine.InputSystem;

public class CraftingPanelManager : MonoBehaviour
{
    public GameObject craftingPanel;

    void Update()
    {
        if (craftingPanel.activeSelf && Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
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
