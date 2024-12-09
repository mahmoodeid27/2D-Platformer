using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScreenFade : MonoBehaviour
{
    public Image fadeImage; // The black UI Panel Image
    public float fadeDuration = 3f; // Duration of the fade-out
    public float delayAfterFade = 1f; // Delay before showing Game Over UI
    public GameObject gameOverUI; // Reference to the Game Over UI

    // Reference to GameManager to trigger game over logic
    public GameManager gameManager;

    private bool isFadingOut = false; // Flag to prevent multiple fade-out triggers

    void Start()
    {
        // Reset fade image to fully transparent on scene load
        if (fadeImage != null)
        {
            fadeImage.canvasRenderer.SetAlpha(0); // Start fully transparent
            fadeImage.enabled = true; // Ensure the fade image is active
        }
        FadeIn();
    }

    // Fade in from black to transparent
    public void FadeIn()
    {
        StartCoroutine(FadeInRoutine());
    }

    private IEnumerator FadeInRoutine()
    {
        // Ensure the image starts fully black
        if (fadeImage != null)
        {
            fadeImage.canvasRenderer.SetAlpha(1.0f); // Start fully black
        }

        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = 1f - Mathf.Clamp01(elapsed / fadeDuration); // Decrease alpha
            fadeImage.canvasRenderer.SetAlpha(alpha); // Fade to transparent
            yield return null; // Wait for the next frame
        }

        // Ensure the fadeImage is fully transparent
        fadeImage.canvasRenderer.SetAlpha(0);
    }

    // Fade the screen to black (triggered on player death)

    public void TestFadeOut()
    {
        FadeOut();
    }

    public void FadeOut()
    {
        // Ensure that fade-out only happens once
        if (!isFadingOut)
        {
            isFadingOut = true; // Set the flag so fade-out doesn't trigger multiple times
            StartCoroutine(FadeOutRoutine());
        }
    }

    private IEnumerator FadeOutRoutine()
    {
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsed / fadeDuration);
            fadeImage.canvasRenderer.SetAlpha(alpha); // Increase alpha to black
            yield return null; // Wait for the next frame
        }

        // Ensure the fadeImage is fully black
        fadeImage.canvasRenderer.SetAlpha(1);

        // Add a delay before showing the Game Over UI
        yield return new WaitForSeconds(delayAfterFade);

        ShowGameOverUI();
    }

    private void ShowGameOverUI()
    {
    // Only call GameManager's GameOver method
        if (gameManager != null)
        {
            gameManager.GameOver();
        

    // Disable the fade image to ensure no further fading occurs
        fadeImage.enabled = false;
        }
    }
}
