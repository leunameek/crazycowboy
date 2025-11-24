using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("SOnidossss")]
    public AudioClip jumpClip;
    public AudioClip deathClip;

    [Header("volumen")]
    [Range(0f, 1f)] public float jumpVolume = 1f;
    [Range(0f, 1f)] public float deathVolume = 1f;

    private AudioSource audioSource;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
    }

    public void PlayJump()
    {
        PlayClip(jumpClip, jumpVolume);
    }

    public void PlayDeath()
    {
        PlayClip(deathClip, deathVolume);
    }

    void PlayClip(AudioClip clip, float volume)
    {
        if (clip == null || audioSource == null) return;
        audioSource.PlayOneShot(clip, volume);
    }
}
