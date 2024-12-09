using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Audio Mixers")]
    public AudioMixer audioMixer;
    public AudioMixer sfxMixer;

    [Header("Audio Source and Clips")]
    public AudioSource audioSource; // Drag your MusicManager's AudioSource here in the Inspector
    public AudioClip mainMenuMusic;
    public AudioClip level1Music;
    public AudioClip level2Music;
    public AudioClip bossLevelMusic;  // Track for boss level (assigned to Track 7 in your case)
    public AudioClip attackSound;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            audioSource = GetComponent<AudioSource>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Change the background music to a new clip
    public void ChangeBackgroundMusic(AudioClip newClip)
    {
        if (audioSource.clip != newClip)
        {
            audioSource.clip = newClip;
            audioSource.Play();
        }
    }

    // Method to stop the current background music (in this case, Track 7 for the boss level)
    public void StopCurrentMusic()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    // Set the background music volume
    public void SetMusicVolume(float volume)
    {
        if (audioMixer == null)
        {
            Debug.LogError("Audio Mixer is not assigned!");
            return;
        }

        volume = Mathf.Clamp(volume, 0f, 1f);
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(Mathf.Max(volume, 0.0001f)) * 20);
    }

    // Set the SFX volume
    public void SetSfxVolume(float volume)
    {
        if (sfxMixer == null)
        {
            Debug.LogError("SFX Mixer is not assigned!");
            return;
        }

        volume = Mathf.Clamp(volume, 0f, 1f);
        sfxMixer.SetFloat("SFXVolume", Mathf.Log10(Mathf.Max(volume, 0.0001f)) * 20);
    }

    // Play a single sound effect, such as an attack sound
    public void PlaySound(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogWarning("No audio clip assigned to PlaySound.");
            return;
        }

        audioSource.PlayOneShot(clip);
    }
}
