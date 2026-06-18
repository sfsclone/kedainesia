using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonSFXTrigger : MonoBehaviour
{
    [SerializeField] private AudioClip clickSFX;

    void Start()
    {
        Button btn = GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.AddListener(PlaySound);
        }
    }

    private void PlaySound()
    {
        if (AudioManager.Instance != null && clickSFX != null)
        {
            AudioManager.Instance.PlaySFX(clickSFX);
        }
    }
}
