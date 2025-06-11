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

    public GameObject gameFinishedPanel; // Assign in Inspector

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
            ShowGameFinishedPanel();
        }
    }

    void ShowGameFinishedPanel()
    {
        if (gameFinishedPanel != null)
            gameFinishedPanel.SetActive(true);
    }


    void UpdateDayUI()
    {
        if (dayText != null)
            dayText.text = $"Hari - {currentDay}";

        FindAnyObjectByType<GuideBookManager>()?.UpdateGuideBookVisibility(currentDay);
    }


    public void RestartDay()
    {
        Debug.Log("Restarting current day due to 3 warnings.");
        FindAnyObjectByType<WarningSystem>()?.ResetWarnings();

        // Reset the clock
        FindAnyObjectByType<GameClock>()?.ResetClock();

        // Clear and respawn customers
        customerManager.ClearCustomers();
        customerManager.GenerateTodaysCustomers(currentDay);
    }


    //testing hari
#if UNITY_EDITOR
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            SkipToNextDayForTesting();
        }
    }
#endif

    public void SkipToNextDayForTesting()
    {
        Debug.Log("Skipping to next day for testing...");

        if (currentDay < maxDays)
        {
            currentDay++;
            UpdateDayUI();

            // Reset warnings
            FindAnyObjectByType<WarningSystem>()?.ResetWarnings();

            // Stop and reset the clock
            var clock = FindAnyObjectByType<GameClock>();
            if (clock != null)
            {
                clock.clockRunning = false; // Make sure it's stopped
                clock.ResetClock();
            }

            // Clear all customers
            customerManager.ClearCustomers();
        }
        else
        {
            Debug.Log("Cannot skip: final day reached.");
        }
    }




}
