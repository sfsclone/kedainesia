using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WarningSystem : MonoBehaviour
{
    [SerializeField] private GameObject restartDayButton;
    [SerializeField] private GameClock gameClock;
    [SerializeField] private CustomerManager customerManager;
    [SerializeField] private Button openButton;

    public int maxWarnings = 3;
    private int currentWarnings = 0;

    public TMP_Text warningText; // Assign in Inspector: "Peringatan: X / 3"
    public GameManager gameManager;

    public bool HasReachedMaxWarnings => currentWarnings >= maxWarnings;

    private void Start()
    {
        UpdateWarningUI();
    }

    public void AddWarning()
    {
        currentWarnings++;
        UpdateWarningUI();

        if (currentWarnings >= maxWarnings)
        {
            Debug.Log("Max warnings reached. Show restart button.");
            gameClock.clockRunning = false;
            gameClock.ResetClock();
            customerManager.ClearCustomers();
            restartDayButton.SetActive(true);
        }
    }

    public void OnRestartDayClicked()
    {
        currentWarnings = 0;
        restartDayButton.SetActive(false);
        customerManager.GenerateTodaysCustomers(gameManager.currentDay);
        gameClock.ResetClock();
        UpdateWarningUI();
    }

    public void ResetWarnings()
    {
        currentWarnings = 0;
        UpdateWarningUI();
    }

    private void UpdateWarningUI()
    {
        if (warningText != null)
        {
            warningText.text = $"Peringatan : {currentWarnings} / {maxWarnings}";
        }
    }
}
