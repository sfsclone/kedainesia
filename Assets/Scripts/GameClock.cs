using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameClock : MonoBehaviour
{
    public TMP_Text clockText;
    public Button openButton; // assign in Inspector
    public float timeMultiplier = 60f; // 1 real second = 1 in-game minute
    private float currentTime = 9 * 60; // Start at 09:00
    public bool clockRunning = false;
    public CustomerManager customerManager; // Assign in Inspector
    public GameManager gameManager; // Assign in Inspector

    void Start()
    {
        openButton.onClick.AddListener(StartClock);
        UpdateClockText(); // show 09:00 at the start
    }

    public void StartClock()
    {
        clockRunning = true;
        openButton.gameObject.SetActive(false); // Hide the button
        Debug.Log("Restaurant opened. Clock started.");

        //Generate customers based on current day only when restaurant opens
        customerManager.GenerateTodaysCustomers(gameManager.currentDay);
        customerManager.StartCustomerFlow();
    }

    void Update()
    {
        if (!clockRunning) return;

        currentTime += Time.deltaTime * timeMultiplier / 60f;
        UpdateClockText();

        if (currentTime >= 17 * 60)
        {
            clockRunning = false;
            Debug.Log("Restaurant closed. Day ended.");

            if (gameManager != null)
            {
                gameManager.ShowNextDayButton();
            }
        }
    }

    void UpdateClockText()
    {
        int hours = Mathf.FloorToInt(currentTime / 60);
        int minutes = Mathf.FloorToInt(currentTime % 60);
        clockText.text = $"{hours:00}:{minutes:00}";
    }

    public void ResetClock()
    {
        currentTime = 9 * 60;
        UpdateClockText();
        openButton.gameObject.SetActive(true);
    }

    public void CloseRestaurantEarly()
    {
        if (!clockRunning) return;

        clockRunning = false;
        Debug.Log("All customers served. Restaurant closed early.");

        if (gameManager != null)
        {
            gameManager.ShowNextDayButton();
        }
    }

}
