using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    [Header("Pages")]
    public GameObject[] pages;
    private int currentPage = 0;

    [Header("Navigation Buttons")]
    public Button nextButton;
    public Button backButton;

    private void OnEnable()
    {
        currentPage = 0;
        UpdatePages();
    }

    public void NextPage()
    {
        if (currentPage < pages.Length - 1)
        {
            currentPage++;
            UpdatePages();
        }
    }

    public void PreviousPage()
    {
        if (currentPage > 0)
        {
            currentPage--;
            UpdatePages();
        }
    }

    private void UpdatePages()
    {
        if (pages == null || pages.Length == 0) return;

        for (int i = 0; i < pages.Length; i++)
        {
            if (pages[i] != null)
            {
                pages[i].SetActive(i == currentPage);
            }
        }

        // Update button interactivity
        if (nextButton != null)
            nextButton.interactable = (currentPage < pages.Length - 1);
        
        if (backButton != null)
            backButton.interactable = (currentPage > 0);
    }
}
