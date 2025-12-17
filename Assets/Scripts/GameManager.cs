using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public CustomerManager customerManager;
    public TMP_Text dayText;
    public GameObject nextDayButton;
    public GameObject gameFinishedPanel;

    public int currentDay = 1;
    public int maxDays = 7;

    private void Start()
    {
        nextDayButton.SetActive(false); // Ensure hidden at start
        UpdateDayUI();
        FindAnyObjectByType<GameClock>()?.ResetClock();
    }

    public void ShowNextDayButton()
    {
        // Show only if the day isn't finished yet
        if (currentDay < maxDays)
            nextDayButton.SetActive(true);
        else
            ShowGameFinishedPanel();
    }

    public void OnNextDayButtonClicked()
    {
        nextDayButton.SetActive(false); // Hide the button
        AdvanceDay();
    }

    public void AdvanceDay()
    {
        nextDayButton.SetActive(false); // Double safety: hide again here

        if (currentDay < maxDays)
        {
            currentDay++;
            UpdateDayUI();

            // Reset clock
            var clock = FindAnyObjectByType<GameClock>();
            if (clock != null)
            {
                clock.clockRunning = false;
                clock.ResetClock();
            }

            // Reset warnings and customers
            FindAnyObjectByType<WarningSystem>()?.ResetWarnings();
            customerManager.ClearCustomers();
            customerManager.GenerateTodaysCustomers(currentDay);
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

        // Make sure button is hidden every time day UI updates
        if (nextDayButton != null)
            nextDayButton.SetActive(false);
    }

    public void RestartDay()
    {
        Debug.Log("Restarting current day due to 3 warnings.");
        FindAnyObjectByType<WarningSystem>()?.ResetWarnings();

        var clock = FindAnyObjectByType<GameClock>();
        clock?.ResetClock();

        customerManager.ClearCustomers();
        customerManager.GenerateTodaysCustomers(currentDay);

        nextDayButton.SetActive(false); // Hide again
    }

#if UNITY_EDITOR
    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.nKey.wasPressedThisFrame)
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
            FindAnyObjectByType<WarningSystem>()?.ResetWarnings();

            var clock = FindAnyObjectByType<GameClock>();
            if (clock != null)
            {
                clock.clockRunning = false;
                clock.ResetClock();
            }

            customerManager.ClearCustomers();
            customerManager.GenerateTodaysCustomers(currentDay);

            nextDayButton.SetActive(false); // Hide button again
        }
        else
        {
            Debug.Log("Cannot skip: final day reached.");
        }
    }
}
