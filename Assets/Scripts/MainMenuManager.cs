using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public static int selectedDay = 1;

    public void StartNewGame()
    {
        selectedDay = 1;
        SceneManager.LoadScene("GameScene1"); // Make sure the game scene is added in Build Settings
    }

    public void LoadLevel(int day)
    {
        selectedDay = day;
        SceneManager.LoadScene("GameScene1");
    }

    public void ResetProgress()
    {
        PlayerPrefs.DeleteKey("HighestUnlockedDay");
        PlayerPrefs.Save();
        Debug.Log("Progress reset. Reloading scene...");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
