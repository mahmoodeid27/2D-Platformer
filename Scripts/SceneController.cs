using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    private AudioManager audioManager;

    void Awake()
    {
        audioManager = FindObjectOfType<AudioManager>();
        if (audioManager == null)
        {
            Debug.LogError("AudioManager not found in the scene!");
        }
    }

    public void LoadMainMenu()
    {
        // Change to main menu music only if it�s not already playing
        if (audioManager != null && audioManager.audioSource.clip != audioManager.mainMenuMusic)
        {
            audioManager.ChangeBackgroundMusic(audioManager.mainMenuMusic);
        }
        SceneManager.LoadScene(0); // Assuming Main Menu is index 0
    }

    public void StartGame()
    {
        if (audioManager != null)
        {
            audioManager.ChangeBackgroundMusic(audioManager.level1Music);
        }
        SceneManager.LoadScene(1);
        

    }

    public void LoadSettings()
    {
        // Do not change music, as Settings should continue Main Menu music
        SceneManager.LoadScene(2); 
    }

    public void LoadControls()
    {
        // Do not change music, as Controls should continue Main Menu music
        SceneManager.LoadScene(3); // Assuming Controls is index 3
    }

    public void LoadLevel2()
    {
        if (audioManager != null)
        {
            
            audioManager.ChangeBackgroundMusic(audioManager.level2Music);
        }
        // Do not change music, as Settings should continue Main Menu music
        SceneManager.LoadScene(4); 
    }

    public void LoadBossLevel()
    {
        if (audioManager != null)
        {

            audioManager.ChangeBackgroundMusic(audioManager.bossLevelMusic);
        }
        // Do not change music, as Settings should continue Main Menu music
        SceneManager.LoadScene(5);
    }
}
