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

    public void LoadTutorialScene()
    {
        SceneManager.LoadScene("TutorialScene");
    }

    public void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
