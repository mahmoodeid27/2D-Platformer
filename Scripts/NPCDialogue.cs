using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPCDialogue : MonoBehaviour
{
    public GameObject dialogueBox;
    public Text dialogueText;
    [TextArea(3, 10)]
    public string[] dialogueLines;
    public PlayerMovement playerMovement;
    public Animator playerAnimator; // Reference to the player's Animator
    public GameObject interactPrompt;
    public GameObject skipText;
    public Transform playerTransform; // Reference to the player's Transform
    public float interactionDistance = 1.5f; // Distance at which interaction is allowed

    public GameObject powerUp; // Reference to the power-up object
    private bool hasCollectedPowerUp = false; // Tracks if power-up is collected

    private int currentLine = 0;
    private bool isInteracting = false;
    private bool facingRight = false; // Assume NPC starts facing left; set to true if otherwise
    private GameManager gameManager;

    void Update()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        // Show interaction prompt if within range and not interacting
        if (!hasCollectedPowerUp && distanceToPlayer <= interactionDistance && !isInteracting)
        {
            interactPrompt.SetActive(true);

            // Start dialogue if player presses E
            if (Input.GetKeyDown(KeyCode.E))
            {
                gameManager = FindObjectOfType<GameManager>();
                gameManager.StopTimer();
                ShowDialogue();
            }
        }
        else if (!hasCollectedPowerUp)
        {
            interactPrompt.SetActive(false);
        }

        // Handle dialogue skipping
        if (isInteracting && dialogueBox.activeSelf && Input.GetKeyDown(KeyCode.Return))
        {
            NextLine();
        }
    }

    void ShowDialogue()
    {
        dialogueBox.SetActive(true);
        currentLine = 0;
        dialogueText.text = dialogueLines[currentLine];

        playerMovement.StopMovement(); // Stop player movement instantly
        playerMovement.enabled = false; // Disable movement script

        playerAnimator.SetBool("run", false); // Set run to false to stop running animation

        interactPrompt.SetActive(false);
        skipText.SetActive(true); // Show "Enter to skip" text
        isInteracting = true;

        // Determine if NPC should flip to face the player (reversed logic)
        bool playerIsToLeft = playerTransform.position.x < transform.position.x;

        if (playerIsToLeft && !facingRight)
        {
            FlipNPC(true);
        }
        else if (!playerIsToLeft && facingRight)
        {
            FlipNPC(false);
        }
    }

    void NextLine()
    {
        currentLine++;
        if (currentLine < dialogueLines.Length)
        {
            dialogueText.text = dialogueLines[currentLine];
        }
        else
        {
            dialogueBox.SetActive(false);
            playerMovement.enabled = true;
            skipText.SetActive(false); // Hide "Enter to skip" text
            isInteracting = false;

            gameManager = FindObjectOfType<GameManager>();
            gameManager.StartTimer();
        }
    }

    private void FlipNPC(bool faceRight)
    {
        facingRight = faceRight;
        transform.localScale = new Vector3(faceRight ? Mathf.Abs(transform.localScale.x) : -Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }

    public void SetDialogue(string[] newDialogueLines)
    {
        dialogueLines = newDialogueLines;
    }

    // Trigger forced dialogue when the power-up is collected
    public void ActivateDialogueFromPowerUp()
    {
        if (!isInteracting)
        {
            ShowDialogue();
        }
    }
}
