using System.Collections;
using UnityEngine;

public class Health : MonoBehaviour
{
    [Header ("Death Sound")]
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private AudioClip hurtSound;
    [SerializeField] private float startingHealth;
    public float currentHealth { get; private set; }
    private bool dead = false;
    [SerializeField] private ScreenFade screenFade;
    private GameManager gameManager;

    private MeleeEnemy meleeEnemy;  // Reference to MeleeEnemy for animations
    private Animator playerAnimator; // Reference to Animator for player hurt and death animations
    private PlayerMovement playerMovement; // Reference to player movement script
    private Rigidbody2D rb; // Reference to Rigidbody2D for stopping movement on death

    [SerializeField] private bool isPlayer = false;  // Flag to indicate whether this is the player or enemy
    [SerializeField] private float deathYThreshold = -10f;
    [Header("iFrames")]
    [SerializeField] private float iFramesDuration;
    [SerializeField] private int numberOfFlashes;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        gameManager = FindAnyObjectByType<GameManager>();
        currentHealth = startingHealth;

        if (isPlayer)
        {
            playerAnimator = GetComponent<Animator>();
            playerMovement = GetComponent<PlayerMovement>(); // Assuming the movement script is called "PlayerMovement"
            rb = GetComponent<Rigidbody2D>(); // Get the Rigidbody2D component
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
        else
        {
            meleeEnemy = GetComponent<MeleeEnemy>();
        }

    }

    private void Update()
    {
        if (isPlayer && transform.position.y < deathYThreshold && !dead)
        {
            Die();
        }
    }

    public void TakeDamage(float _damage)
    {
        currentHealth = Mathf.Clamp(currentHealth - _damage, 0, startingHealth);

        if (currentHealth > 0)
        {
            if (isPlayer)
            {
                playerAnimator.SetBool("isGrounded", true);
                playerAnimator.SetTrigger("hurt"); // Trigger hurt animation
                AudioManager.instance.PlaySound(hurtSound);
            }
            else
            {
                meleeEnemy.TriggerHurtAnimation();
            }
        }
        else
        {
            Die();
        }
        StartCoroutine(invulnerability());
    }

    private void Die()
    {
        if (!dead)
        {
            dead = true;
            AudioManager.instance.PlaySound(deathSound);

            if (isPlayer)
            {
                // Disable player movement and reset velocity
                if (playerMovement != null)
                {
                    playerMovement.enabled = false;
                }
            
                if (rb != null)
                {
                    rb.velocity = Vector2.zero; // Stop any movement
                    rb.constraints = RigidbodyConstraints2D.FreezePosition; // Freeze the Rigidbody's position
                }

                // Trigger the death animation with the "die" trigger
                if (playerAnimator != null)
                {
                    playerAnimator.SetBool("isGrounded", true);
                    playerAnimator.SetTrigger("die"); // Ensure "die" trigger exists in the Animator
                }
                //screenFade.FadeOut();
                if (gameManager != null)
                {
                    gameManager.GameOver();
                }
                else
                {
                    Debug.LogError("GameManager not found in the scene!");
                }
            }
            else
            {
                meleeEnemy.TriggerDeath();
            }
            
        }
    }

    public void AddHealth(float _value)
    {
        currentHealth = Mathf.Clamp(currentHealth + _value, 0, startingHealth);
    }
    private IEnumerator invulnerability()
    {
        Physics2D.IgnoreLayerCollision(8, 9, true);
        for (int i = 0; i < numberOfFlashes; i++)
        {
            spriteRenderer.color = new Color(1, 0, 0, 0.5f);
            yield return new WaitForSeconds(iFramesDuration / (numberOfFlashes * 2));
            spriteRenderer.color = Color.white;
            yield return new WaitForSeconds(iFramesDuration / (numberOfFlashes * 2));

        }
        Physics2D.IgnoreLayerCollision(8, 9, false);
    }
}
