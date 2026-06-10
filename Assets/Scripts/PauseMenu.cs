using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using Unity.VisualScripting;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuPanel;

    private bool isPaused = false;

    private void Start()
    {
        pauseMenuPanel.SetActive(false); // Hide panel di awal
    }

    private void Update()
    {
        // Toggle pause if Esc is pressed
        if (Keyboard.current != null && Keyboard.current.escapeKey.isPressed)
        {
            TooglePause();
        }
    }

    public void TooglePause()
    {
        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    private void PauseGame()
    {
        pauseMenuPanel?.SetActive(true);
        Time.timeScale = 0f; // freeze waktu ingame
        isPaused = true;
    }

    public void ResumeGame()
    {
        pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f; // waktu lanjut
        isPaused = false;
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f; // mastiin waktu lanjut
        SceneManager.LoadScene("MainMenu"); // ganti scene ke main menu
    }
}
