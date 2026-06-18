using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WarningSystem : MonoBehaviour
{
    [SerializeField] private GameObject gameOverPanel; 
    [SerializeField] private Button restartButton;     
    public AudioClip gameOverSFX;

    [SerializeField] private GameClock gameClock;
    [SerializeField] private CustomerManager customerManager;
    [SerializeField] private Button openButton;

    public int maxWarnings = 3;
    private int currentWarnings = 0;

    public TMP_Text warningText;
    public GameManager gameManager;

    public bool HasReachedMaxWarnings => currentWarnings >= maxWarnings;

    private void Start()
    {
        UpdateWarningUI();
        restartButton.onClick.AddListener(OnRestartDayClicked); // Hook listener
        gameOverPanel.SetActive(false);                         // Hide at start
    }

    public void AddWarning()
    {
        currentWarnings++;
        UpdateWarningUI();

        if (currentWarnings >= maxWarnings)
        {
            Debug.Log("Max warnings reached. Show Game Over panel.");
            gameClock.clockRunning = false;
            gameClock.ResetClock();
            customerManager.ClearCustomers();

            if (AudioManager.Instance != null && gameOverSFX != null)
            {
                AudioManager.Instance.PlaySFX(gameOverSFX, true); // Added duckMusic: true
            }

            gameOverPanel.SetActive(true); //Show Game Over
}
    }

    public void OnRestartDayClicked()
    {
        currentWarnings = 0;
        gameOverPanel.SetActive(false); //Hide Game Over
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
            warningText.text = $"Peringatan : {currentWarnings}/{maxWarnings}";
    }
}
