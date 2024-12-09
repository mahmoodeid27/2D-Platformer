using UnityEngine;
using System.Collections;

public class BossController : MonoBehaviour
{
    public int maxHealthPhase1 = 8;
    public int maxHealthPhase2 = 7;
    public int maxHealthPhase3 = 7;
    public float attackDelay = 2f;
    public GameObject maggotPrefab;
    public int damageOnPlayerCollision = 1; // Damage dealt to player on collision
    public int downedHitCount = 7; // Number of hits required to exit the downed state

    private int currentPhase = 1;
    private int currentHealth;
    private int downedHitsReceived; // Track hits while downed
    private bool isDowned = false;
    public bool inPersistentDownedState = false; // Track if in persistent downed state
    private bool canAttack = true; // Controls whether boss is allowed to attack

    private Animator animator;
    private AttackManager attackManager;
    private Transform player;
    private SpriteRenderer spriteRenderer;
    private Color originalColor; // Store the original color

    private void Start()
    {
        animator = GetComponent<Animator>();
        attackManager = GetComponent<AttackManager>(); // Reference to AttackManager
        player = GameObject.FindGameObjectWithTag("Player").transform;
        spriteRenderer = GetComponent<SpriteRenderer>(); // Get the SpriteRenderer component

        originalColor = spriteRenderer.color; // Store the original color at the start

        SetPhase(1); // Start with phase 1
    }

    private void SetPhase(int phase)
    {
        currentPhase = phase;
        downedHitsReceived = 0; // Reset downed hit counter for each phase

        switch (phase)
        {
            case 1:
                currentHealth = maxHealthPhase1;
                break;
            case 2:
                currentHealth = maxHealthPhase2;
                break;
            case 3:
                currentHealth = maxHealthPhase3;
                break;
        }

        // Update AttackManager's phase
        attackManager.SetCurrentPhase(phase);
    }

    public void TakeDamage(int damage)
    {
        StartCoroutine(FlashRed()); // Trigger the red flashing effect

        if (inPersistentDownedState)
        {
            downedHitsReceived++;
            animator.SetTrigger("isHurt"); // Trigger the hurt animation
            Debug.Log("Boss downed hit received. Total hits: " + downedHitsReceived);

            if (downedHitsReceived >= downedHitCount)
            {
                ExitDownedState(); // Transition to the next phase after 7 hits
            }
            else
            {
                // Return to the downed animation after the hurt animation completes
                StartCoroutine(ReturnToDownedAfterHurt());
            }
        }
        else if (isDowned)
        {
            // The boss just entered the downed state, waiting for the animation event to lock him in.
        }
        else
        {
            currentHealth -= damage;
            Debug.Log("Boss took damage! Current health: " + currentHealth);

            if (currentHealth <= 0 && !isDowned)
            {
                StartCoroutine(EnterDownedState()); // Enter downed state when health reaches 0
            }
        }
    }

    private IEnumerator FlashRed()
    {
        spriteRenderer.color = Color.red; // Set color to red
        yield return new WaitForSeconds(0.1f); // Flash duration
        spriteRenderer.color = originalColor; // Return to original color
    }

    private IEnumerator EnterDownedState()
    {
        isDowned = true;
        canAttack = false; // Disable attacking while downed
        downedHitsReceived = 0; // Reset hit counter when entering downed state
        animator.SetBool("isDowned", true);

        yield return null; // Ensure we stay in downed state until 7 hits are received
    }

    // This function will be triggered by an Animation Event in the final frame of the down animation
    public void EnterPersistentDownedState()
    {
        inPersistentDownedState = true; // Lock the boss in the downed state
        animator.SetBool("isDowned", true); // Keep the down animation
    }

    private IEnumerator ReturnToDownedAfterHurt()
    {
        // Wait for the "hurt" animation to finish before returning to the downed animation
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        if (inPersistentDownedState) // Ensure the boss is still in the downed state
        {
            animator.SetBool("isDowned", true); // Return to downed animation
        }
    }

    private void ExitDownedState()
    {
        inPersistentDownedState = false;
        isDowned = false;
        canAttack = true; // Re-enable attacking in the next phase
        currentPhase++;
        animator.SetBool("isDowned", false);

        if (currentPhase > 3)
        {
            Die();
        }
        else
        {
            SetPhase(currentPhase); // Update phase on recovery
        }
    }

    private void Die()
    {
        animator.Play("die");
        AudioManager.instance.StopCurrentMusic();



        Invoke("SpawnMaggot", 2f);
    }

    private void SpawnMaggot()
    {
        Instantiate(maggotPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Health playerHealth = collision.gameObject.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damageOnPlayerCollision);
            }
        }
    }
}
