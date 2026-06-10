using UnityEngine;
using UnityEngine.UI;

public class SettingsPanelController : MonoBehaviour
{
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private Button backButton;

    private AudioManager _audioManager;

    void Awake()
    {
        _audioManager = Object.FindAnyObjectByType<AudioManager>();
        
        if (volumeSlider != null)
        {
            volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        }

        if (backButton != null)
        {
            backButton.onClick.AddListener(ClosePanel);
        }
    }

    void OnEnable()
    {
        if (_audioManager == null)
        {
            _audioManager = Object.FindAnyObjectByType<AudioManager>();
        }

        if (_audioManager != null && volumeSlider != null)
        {
            volumeSlider.value = _audioManager.GetVolume();
        }
    }

    private void OnVolumeChanged(float value)
    {
        if (_audioManager != null)
        {
            _audioManager.SetVolume(value);
        }
    }

    private void ClosePanel()
    {
        gameObject.SetActive(false);
    }
}
