using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    public CustomerManager customerManager;
    public TMP_Text dayText;
    public GameObject nextDayButton;
    [FormerlySerializedAs("gameFinishedPanel")]
    public GameObject winPanel;
    public AudioClip winSFX;

    public int currentDay = 1;
    public int maxDays = 4;

    private void Start()
    {
        currentDay = MainMenuManager.selectedDay;
        nextDayButton.SetActive(false); // Ensure hidden at start
        UpdateDayUI();
        FindAnyObjectByType<GameClock>()?.ResetClock();
    }

    public void ShowNextDayButton()
    {
        if (winPanel != null)
        {
            winPanel.SetActive(true);
            if (AudioManager.Instance != null && winSFX != null)
            {
                AudioManager.Instance.PlaySFX(winSFX, true); // Duck music for win sound
            }
        }

        if (currentDay < maxDays)
        {
            if (nextDayButton != null)
                nextDayButton.SetActive(true);
        }
        else
        {
            if (nextDayButton != null)
                nextDayButton.SetActive(false);
            
            // Handle final win logic
            PlayerPrefs.SetInt("HighestUnlockedDay", maxDays);
            PlayerPrefs.Save();
        }
    }

    public void OnNextDayButtonClicked()
    {
        if (nextDayButton != null)
            nextDayButton.SetActive(false);
        if (winPanel != null)
            winPanel.SetActive(false);
        AdvanceDay();
    }

    public void AdvanceDay()
    {
        if (nextDayButton != null)
            nextDayButton.SetActive(false);
        if (winPanel != null)
            winPanel.SetActive(false);

        if (currentDay < maxDays)
        {
            currentDay++;
            PlayerPrefs.SetInt("HighestUnlockedDay", currentDay);
            PlayerPrefs.Save();
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
            ShowWinPanel();
        }
    }

    void ShowWinPanel()
    {
        PlayerPrefs.SetInt("HighestUnlockedDay", maxDays);
        PlayerPrefs.Save();
        if (winPanel != null)
        {
            winPanel.SetActive(true);
            if (AudioManager.Instance != null && winSFX != null)
            {
                AudioManager.Instance.PlaySFX(winSFX, true);
            }
        }
    }

    void UpdateDayUI()
    {
        if (dayText != null)
            dayText.text = $"Hari {currentDay}";

        FindAnyObjectByType<GuideBookManager>()?.UpdateGuideBookVisibility(currentDay);

        // Make sure button and panel are hidden every time day UI updates
        if (nextDayButton != null)
            nextDayButton.SetActive(false);
        if (winPanel != null)
            winPanel.SetActive(false);
    }

    public void RestartDay()
    {
        Debug.Log("Restarting current day due to 3 warnings.");
        FindAnyObjectByType<WarningSystem>()?.ResetWarnings();

        var clock = FindAnyObjectByType<GameClock>();
        clock?.ResetClock();

        customerManager.ClearCustomers();
        customerManager.GenerateTodaysCustomers(currentDay);

        if (nextDayButton != null)
            nextDayButton.SetActive(false);
        if (winPanel != null)
            winPanel.SetActive(false);
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
