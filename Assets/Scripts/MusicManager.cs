using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    [Header("Music Clips")]
    public AudioClip menuMusic;
    public AudioClip levelMusic;

    [Header("Volumes")]
    [Range(0f, 1f)] public float menuVolume = 0.7f;
    [Range(0f, 1f)] public float levelVolume = 0.7f;

    private AudioSource musicSource;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        musicSource = GetComponent<AudioSource>();
        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
        }

        musicSource.loop = true;
        musicSource.playOnAwake = false;
    }

    public void PlayMenuMusic(bool restart = false)
    {
        PlayMusic(menuMusic, menuVolume, restart);
    }

    public void PlayLevelMusic(bool restart = false)
    {
        PlayMusic(levelMusic, levelVolume, restart);
    }

    public void RestartCurrent()
    {
        if (musicSource == null) return;
        musicSource.time = 0f;
        musicSource.Play();
    }

    void PlayMusic(AudioClip clip, float volume, bool restart)
    {
        if (clip == null || musicSource == null) return;

        if (musicSource.clip == clip && musicSource.isPlaying && !restart)
            return;

        musicSource.clip = clip;
        musicSource.volume = volume;
        musicSource.time = 0f;
        musicSource.Play();
    }
}
