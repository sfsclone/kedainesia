using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public CustomerManager customerManager; // Assign in Inspector

    public int currentDay = 1;
    public int maxDays = 7;
    public TMP_Text dayText;

    public GameObject nextDayButton; // Assign in Inspector

    private void Start()
    {
        UpdateDayUI();
        FindAnyObjectByType<GameClock>().ResetClock();
    }

    public void ShowNextDayButton()
    {
        nextDayButton.SetActive(true);
    }

    public void OnNextDayButtonClicked()
    {
        nextDayButton.SetActive(false);
        AdvanceDay();
    }

    public void AdvanceDay()
    {
        if (currentDay < maxDays)
        {
            currentDay++;
            UpdateDayUI();

            FindAnyObjectByType<GameClock>().ResetClock();
        }
        else
        {
            Debug.Log("Game finished. Final day complete.");
            // Trigger end screen here
        }
    }

    void UpdateDayUI()
    {
        if (dayText != null)
            dayText.text = $"Hari - {currentDay}";
    }
}
