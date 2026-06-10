using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public AudioSource bgmSource;
    public AudioClip[] bgmClips;
    public AudioMixer audioMixer;

    private int currentBGMIndex = 0;
    private const string MusicVolumeParam = "MusicVolume";
    private const string MusicVolumePref = "MusicVolume";

    void Awake()
    {
        // Set mixer group if assigned
        if (audioMixer != null)
        {
            var groups = audioMixer.FindMatchingGroups("Music");
            if (groups.Length > 0)
            {
                bgmSource.outputAudioMixerGroup = groups[0];
            }
        }
    }

    void Start()
    {
        LoadVolume();

        if (bgmClips != null && bgmClips.Length > 0)
        {
            PlayBGM(currentBGMIndex);
        }
    }

    void Update()
    {
        if (bgmSource != null && !bgmSource.isPlaying)
        {
            PlayNextBGM();
        }
    }

    public void SetVolume(float volume)
    {
        if (audioMixer == null) return;

        // Map 0..1 to -80..20 dB (or similar)
        // Using -80 to 0 is safer for "silence to max"
        float db = Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20;
        audioMixer.SetFloat(MusicVolumeParam, db);
        
        PlayerPrefs.SetFloat(MusicVolumePref, volume);
        PlayerPrefs.Save();
    }

    public float GetVolume()
    {
        return PlayerPrefs.GetFloat(MusicVolumePref, 0.75f);
    }

    private void LoadVolume()
    {
        float volume = GetVolume();
        SetVolume(volume);
    }

    void PlayBGM(int index)
    {
        if (bgmClips == null || bgmClips.Length == 0) return;

        bgmSource.clip = bgmClips[index];
        bgmSource.Play();
    }

    void PlayNextBGM()
    {
        if (bgmClips == null || bgmClips.Length == 0) return;
        currentBGMIndex = (currentBGMIndex + 1) % bgmClips.Length;
        PlayBGM(currentBGMIndex);
    }
}
