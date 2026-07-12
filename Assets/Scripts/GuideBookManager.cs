using UnityEngine;
using UnityEngine.UI;

public class GuideBookManager : MonoBehaviour
{
    public Button openGuideBookButton;
    public Button closeButton;
    public GameObject guideBookPanel;

    public GameObject customerBookPanel;
    public GameObject recipeBookPanel;

    public Button customerBookButton;
    public Button recipeBookButton;

    public GameObject[] customerPages;
    public GameObject[] recipePages;

    

    private int currentPage = 0;
    private enum BookType { Customer, Recipe }
    private BookType activeBook;

    private GameManager gameManager;

    private void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>(); // Find the GameManager in the scene

        if (gameManager != null && gameManager.currentDay == 7)
        {
            openGuideBookButton.gameObject.SetActive(false); // Hide button on Day 7
        }

        customerBookButton.onClick.AddListener(ShowCustomerBook);
        recipeBookButton.onClick.AddListener(ShowRecipeBook);
        
        closeButton.onClick.AddListener(CloseGuideBook);
        openGuideBookButton.onClick.AddListener(OpenGuideBook);

        ShowCustomerBook();
    }
    public void UpdateGuideBookVisibility(int day)
    {
        if (day == 7)
        {
            openGuideBookButton.gameObject.SetActive(false);
            guideBookPanel.SetActive(false); // Optional: close it if it's open
        }
        else
        {
            openGuideBookButton.gameObject.SetActive(true);
        }
    }

    private void OpenGuideBook()
    {
        guideBookPanel.SetActive(true);
        ShowCustomerBook();
    }

    private void CloseGuideBook()
    {
        guideBookPanel.SetActive(false);
    }

    private void ShowCustomerBook()
    {
        activeBook = BookType.Customer;
        recipeBookPanel.SetActive(false);
        customerBookPanel.SetActive(true);
        currentPage = 0;
        UpdatePages();
    }

    private void ShowRecipeBook()
    {
        activeBook = BookType.Recipe;
        customerBookPanel.SetActive(false);
        recipeBookPanel.SetActive(true);
        currentPage = 0;
        UpdatePages();
    }

    public void NextPage()
    {
        int pageCount = activeBook == BookType.Customer ? customerPages.Length : recipePages.Length;
        if (currentPage < pageCount - 1)
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
        if (activeBook == BookType.Customer)
        {
            for (int i = 0; i < customerPages.Length; i++)
                customerPages[i].SetActive(i == currentPage);
        }
        else
        {
            for (int i = 0; i < recipePages.Length; i++)
                recipePages[i].SetActive(i == currentPage);
        }
    }
}
