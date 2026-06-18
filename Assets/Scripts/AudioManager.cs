using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    public AudioSource bgmSource;
    public AudioSource sfxSource;
    public AudioClip[] bgmClips;
    public AudioMixer audioMixer;

    private int currentBGMIndex = 0;
    private const string MusicVolumeParam = "MusicVolume";
    private const string SFXVolumeParam = "SFXVolume";
    private const string MusicVolumePref = "MusicVolume";

    private float currentUserVolume = 0.75f;
    private Coroutine duckingRoutine;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Set mixer group if assigned
        if (audioMixer != null)
        {
            var musicGroups = audioMixer.FindMatchingGroups("Music");
            if (musicGroups.Length > 0)
            {
                bgmSource.outputAudioMixerGroup = musicGroups[0];
            }

            var sfxGroups = audioMixer.FindMatchingGroups("SFX");
            if (sfxGroups.Length > 0)
            {
                sfxSource.outputAudioMixerGroup = sfxGroups[0];
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
        currentUserVolume = volume;
        ApplyVolumes(0f); // Set to default (no ducking)
        
        PlayerPrefs.SetFloat(MusicVolumePref, volume);
        PlayerPrefs.Save();
    }

    private void ApplyVolumes(float musicDuckingDb)
    {
        if (audioMixer == null) return;

        // Base volume in dB
        float baseDb = Mathf.Log10(Mathf.Clamp(currentUserVolume, 0.0001f, 1f)) * 20;
        
        // Music gets the ducking modifier
        audioMixer.SetFloat(MusicVolumeParam, baseDb + musicDuckingDb);
        // SFX stays at base
        audioMixer.SetFloat(SFXVolumeParam, baseDb);
    }

    public float GetVolume()
    {
        return PlayerPrefs.GetFloat(MusicVolumePref, 0.75f);
    }

    private void LoadVolume()
    {
        currentUserVolume = GetVolume();
        ApplyVolumes(0f);
    }

    public void PlaySFX(AudioClip clip, bool duckMusic = false)
    {
        if (sfxSource != null && clip != null)
        {
            sfxSource.PlayOneShot(clip);

            if (duckMusic)
            {
                if (duckingRoutine != null) StopCoroutine(duckingRoutine);
                duckingRoutine = StartCoroutine(DuckMusicRoutine(clip.length));
            }
        }
    }

    private IEnumerator DuckMusicRoutine(float duration)
    {
        float duckDb = -10f; // Lower music by 10 decibels
        float fadeTime = 0.5f;
        float elapsed = 0f;

        // Fade down
        while (elapsed < fadeTime)
        {
            elapsed += Time.deltaTime;
            ApplyVolumes(Mathf.Lerp(0, duckDb, elapsed / fadeTime));
            yield return null;
        }

        ApplyVolumes(duckDb);

        // Wait for the clip to finish (minus fade up time)
        yield return new WaitForSeconds(Mathf.Max(0, duration - fadeTime * 2f));

        // Fade up
        elapsed = 0f;
        while (elapsed < fadeTime)
        {
            elapsed += Time.deltaTime;
            ApplyVolumes(Mathf.Lerp(duckDb, 0, elapsed / fadeTime));
            yield return null;
        }

        ApplyVolumes(0f);
        duckingRoutine = null;
    }

    public void StopSFX()
    {
        if (sfxSource != null)
        {
            sfxSource.Stop();
        }

        if (duckingRoutine != null)
        {
            StopCoroutine(duckingRoutine);
            duckingRoutine = null;
            ApplyVolumes(0f); // Restore music volume immediately
        }
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
