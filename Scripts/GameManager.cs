using System.Collections;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public GameObject gameOverUI;
    public Animator playerAnimator; 
    public float deathAnimationDuration = 2f;

    private static GameManager instance;
    private bool isGameOver = false;

    [SerializeField] private ScreenFade screenFade;
    
    private PlayerMovement playerMovement;
    private TimerUI timer; // Assuming TimerUI manages the timer display

    [SerializeField] private float timeCounter = 0f; // Timer counter
    [SerializeField] private bool isCounting = true; // Control counting state
     private TimerUI timerUI;
     public static GameManager Instance { get; private set; }
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return; // Exit to prevent further execution if this instance is destroyed
        }
    }

       void Start()
    {
        timerUI = FindObjectOfType<TimerUI>();
        StartTimer();
    }
    
    void Update()
    {
        if (isCounting)
        {
            timeCounter -= Time.deltaTime;
            timerUI.UpdateTimerDisplay(timeCounter);
        }
    }


    public void GameOver()
    {
        if (isGameOver) return; // Prevent multiple calls

        isGameOver = true;

        screenFade = FindObjectOfType<ScreenFade>();
        screenFade.FadeOut();

        StartCoroutine(GameOverSequence());

        StopTimer(); // Stop counting when game over occurs
    }

    private IEnumerator GameOverSequence()
    {
        playerMovement = FindObjectOfType<PlayerMovement>();

        if (playerMovement != null)
        {
            playerMovement.enabled = false; // Disable player movement
        }

        // Trigger death animation
        if (playerAnimator != null)
        {
            playerAnimator.SetBool("isGrounded", true);
            playerAnimator.SetTrigger("die");

            // Wait for the death animation to complete
            yield return new WaitForSeconds(deathAnimationDuration);
        }

        ShowGameOverUI();
    }

   private void ShowGameOverUI()
   {
       if (gameOverUI != null)
       {
           gameOverUI.SetActive(true);
       }
   }

   public void RestartLevel()
   {
       int currentSceneIndex = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
       UnityEngine.SceneManagement.SceneManager.LoadScene(currentSceneIndex);
       StartCoroutine(ReinitializeAfterSceneLoad());
   }

   private IEnumerator ReinitializeAfterSceneLoad()
   {
       yield return null; // Wait for the scene to load fully

       gameOverUI = GameObject.Find("GameOverScreen"); 
       ScreenFade screenFade = FindObjectOfType<ScreenFade>();
       
       if (screenFade != null)
       {
           screenFade.fadeImage.enabled = false; 
           screenFade.FadeIn(); 
       }

       ResetTimer(); // Reset timer when restarting level
   }

   public void StartTimer()
    {
        isCounting = true;
    }

   public void StopTimer()
   {
       isCounting = false; // Stop counting
   }

   public void ResetTimer()
   {
       timeCounter = 0f;
       isCounting = false;
       timerUI.UpdateTimerDisplay(timeCounter);

   }

   

   public void ExitGame()
   {
#if UNITY_EDITOR
       UnityEditor.EditorApplication.isPlaying = false; 
#else
       Application.Quit(); 
#endif
   }
}
