using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource bgmSource;
    public AudioClip[] bgmClips;

    private int currentBGMIndex = 0;

    void Start()
    {
        if (bgmClips.Length > 0)
        {
            PlayBGM(currentBGMIndex);
        }
    }

    void Update()
    {
        if (!bgmSource.isPlaying)
        {
            PlayNextBGM();
        }
    }

    void PlayBGM(int index)
    {
        if (bgmClips.Length == 0) return;

        bgmSource.clip = bgmClips[index];
        bgmSource.Play();
    }

    void PlayNextBGM()
    {
        currentBGMIndex = (currentBGMIndex + 1) % bgmClips.Length;
        PlayBGM(currentBGMIndex);
    }
}
