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

        customerManager.StartCustomerFlow(); // spawn next customer
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
            // TODO: Trigger day-end logic here
        }
    }

    void UpdateClockText()
    {
        int hours = Mathf.FloorToInt(currentTime / 60);
        int minutes = Mathf.FloorToInt(currentTime % 60);
        clockText.text = $"{hours:00}:{minutes:00}";
    }
}
