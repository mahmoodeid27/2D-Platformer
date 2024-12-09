using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class FinishFlag : MonoBehaviour
{
    public string playerTag = "Player";
    private GameManager gameManager;
    private Animator animator;
    public GameObject player;

    public GameObject levelCompleteText;
    public GameObject continueMenuPanel;
    public Image fadeImage;
    public float fadeDuration = 2f;
    private PlayerMovement playerMovement;

    void Start()
    {
        animator = GetComponent<Animator>();
        playerMovement = FindObjectOfType<PlayerMovement>();
        if (playerMovement == null)
        {
            Debug.LogError("PlayerMovement component not found!");
        }
        if (animator == null)
        {
            Debug.LogError("Animator component not found on " + gameObject.name);
        }

        levelCompleteText.SetActive(false);
        continueMenuPanel.SetActive(false);
        if (fadeImage != null)
        {
            fadeImage.canvasRenderer.SetAlpha(0);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            animator.SetTrigger("Finish");
            StartCoroutine(StopPlayerMovementAfterDelay());
        }
    }

    private IEnumerator ShowLevelComplete()
    {
        gameManager = FindObjectOfType<GameManager>();
        gameManager.StopTimer();
        levelCompleteText.SetActive(true);
        TextMeshProUGUI textComponent = levelCompleteText.GetComponent<TextMeshProUGUI>();
        Color textColor = textComponent.color;

        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsed / fadeDuration);
            textColor.a = alpha;
            textComponent.color = textColor;
            yield return null;
        }

        textColor.a = 1f;
        textComponent.color = textColor;

        yield return new WaitForSeconds(1f);

        ShowContinueMenu();
    }

    private void ShowContinueMenu()
    {
        levelCompleteText.SetActive(false);
        continueMenuPanel.SetActive(true);
    }

    private IEnumerator StopPlayerMovementAfterDelay()
{
    yield return new WaitForSeconds(0.5f); // Wait for 1 second

    if (playerMovement != null)
    {
        playerMovement.enabled = false; // Disable the PlayerMovement script
        playerMovement.StopMovement();
    }
    gameManager = FindObjectOfType<GameManager>();
    // Stop the timer
    gameManager.StopTimer();

    // Continue with the rest of your finish sequence
    StartCoroutine(ShowLevelComplete());
}



}