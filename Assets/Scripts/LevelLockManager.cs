using UnityEngine;
using UnityEngine.UI;

public class LevelLockManager : MonoBehaviour
{
    public UnityEngine.UI.Button[] levelButtons;
    public Color unlockedColor = new Color(0.953f, 0.851f, 0.353f, 1f); // #F3D95A
    public Color lockedColor = new Color(0.5f, 0.5f, 0.5f, 1f);

    void Start()
    {
        int highestDay = PlayerPrefs.GetInt("HighestUnlockedDay", 1);

        for (int i = 0; i < levelButtons.Length; i++)
        {
            if (levelButtons[i] == null) continue;

            if ((i + 1) <= highestDay)
            {
                // Unlocked: interactable = true, color = unlockedColor
                levelButtons[i].interactable = true;
                if (levelButtons[i].image != null)
                {
                    levelButtons[i].image.color = unlockedColor;
                }
            }
            else
            {
                // Locked: interactable = false, color = lockedColor
                levelButtons[i].interactable = false;
                if (levelButtons[i].image != null)
                {
                    levelButtons[i].image.color = lockedColor;
                }
            }
        }
    }
}
