using UnityEngine;
using System.Collections;

public class AttackManager : MonoBehaviour
{
    private Animator animator;
    private Transform player;
    private Rigidbody2D rb;

    [Header("Attack Parameters")]
    public float horizontalForce = 5f;
    public float jumpForce = 10f;
    public float buildUpTime = 0.5f;
    public float attackCooldown = 2f;
    public int initialIdleDelay = 3;
    public float jumpTime = 1f; // Time for the jump to complete
    public int playerDamage = 1; // Damage dealt to player on each attack

    [Header("Detection Parameters")]
    public float range = 5f;
    public float colliderDistance = 0.5f;
    public LayerMask playerLayer;

    [Header("Detection Box Size")]
    public float detectionBoxWidth = 2.0f;
    public float detectionBoxHeight = 2.0f;

    private bool isOnCooldown = false;
    private bool hasPlayerBeenDetected = false;
    private int attackIndex = 0;
    private int currentPhase = 1;

    private Collider2D hitBox; // Reference to the boss's HitBox collider
    private BossController bossController; // Reference to the BossController script to check downed state

    // Define attack sequences for each phase
    private readonly int[] phase1Sequence = { 1, 2, 2, 1, 2, 2 };
    private readonly int[] phase2Sequence = { 1, 4, 1, 4, 2 };
    private readonly int[] phase3Sequence = { 3, 2, 3, 1, 4 };

    private int[] currentAttackSequence;

    private void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        // Reference to BossController
        bossController = GetComponent<BossController>();

        // Assign the HitBox collider if it exists as a child object
        hitBox = transform.Find("HitBox")?.GetComponent<Collider2D>();

        SetCurrentPhase(1); // Start with phase 1
    }

    private void Update()
    {
        if (!hasPlayerBeenDetected && PlayerInSight())
        {
            Debug.Log("Player detected initially. Starting idle delay.");
            hasPlayerBeenDetected = true;
            StartCoroutine(InitialIdleDelay());
        }
    }

    private IEnumerator InitialIdleDelay()
    {
        Debug.Log("Initial idle delay started.");
        yield return new WaitForSeconds(initialIdleDelay);

        Debug.Log("Initial idle delay ended. Starting attack cycle.");
        StartCoroutine(AttackCycle());
    }

    private IEnumerator AttackCycle()
    {
        while (true)
        {
            if (!isOnCooldown && (bossController == null || !bossController.inPersistentDownedState)) // Check if boss is not downed
            {
                if (PlayerInSight())
                {
                    int currentAttack = currentAttackSequence[attackIndex];
                    Debug.Log($"Executing attack type {currentAttack}");
                    ExecuteAttack(currentAttack);

                    // Move to the next attack in the sequence
                    attackIndex = (attackIndex + 1) % currentAttackSequence.Length;
                }
                else
                {
                    Debug.Log("Player not detected within range. Flipping direction.");
                    FlipDirection();
                }
            }

            yield return new WaitForSeconds(attackCooldown);
        }
    }

    public void ExecuteAttack(int attackType)
    {
        if (isOnCooldown || (bossController != null && bossController.inPersistentDownedState)) return; // Don't execute if boss is on cooldown or downed

        switch (attackType)
        {
            case 1:
                StartCoroutine(PrepareAndExecuteJumpAttack1(attackType));
                break;
            case 2:
                StartCoroutine(PrepareAndExecuteJumpAttack2(attackType));
                break;
            case 3:
                StartCoroutine(PrepareAndExecuteAttack3(attackType));
                break;
            case 4:
                StartCoroutine(PrepareAndExecuteAttack4(attackType));
                break;
            default:
                Debug.LogWarning("Unknown attack type: " + attackType);
                break;
        }

        StartCoroutine(StartCooldown());
    }

    private IEnumerator PrepareAndExecuteJumpAttack1(int attackType)
    {
        Debug.Log("Preparing Jump Attack (Attack1)");
        FacePlayer();
        animator.SetInteger("attackType", attackType); // Set animation parameter for Attack1
        yield return new WaitForSeconds(buildUpTime);

        // Only proceed with the jump if boss is not in the persistent downed state
        if (bossController != null && !bossController.inPersistentDownedState)
        {
            StartJump();
        }

        yield return new WaitForSeconds(jumpTime); // Ensure jump completes
        animator.SetInteger("attackType", 0);
    }

    private IEnumerator PrepareAndExecuteJumpAttack2(int attackType)
    {
        Debug.Log("Preparing Jump Attack (Attack2)");
        FacePlayer();
        animator.SetInteger("attackType", attackType); // Set animation parameter for Attack2
        yield return new WaitForSeconds(buildUpTime);

        if (bossController != null && !bossController.inPersistentDownedState)
        {
            StartJump();
        }

        yield return new WaitForSeconds(jumpTime); // Ensure jump completes
        animator.SetInteger("attackType", 0);
    }

    private IEnumerator PrepareAndExecuteAttack3(int attackType)
    {
        if (bossController != null && bossController.inPersistentDownedState) yield break; // Stop if boss is downed

        Debug.Log("Preparing Attack3");
        animator.SetInteger("attackType", attackType);
        yield return new WaitForSeconds(buildUpTime);

        yield return new WaitForSeconds(0.1f);
        animator.SetInteger("attackType", 0);
    }

    private IEnumerator PrepareAndExecuteAttack4(int attackType)
    {
        if (bossController != null && bossController.inPersistentDownedState) yield break; // Stop if boss is downed

        Debug.Log("Preparing Attack4");
        animator.SetInteger("attackType", attackType);
        yield return new WaitForSeconds(buildUpTime);

        yield return new WaitForSeconds(0.1f);
        animator.SetInteger("attackType", 0);
    }

    private void StartJump()
    {
        int direction = player.position.x < transform.position.x ? -1 : 1;
        rb.velocity = Vector2.zero;  // Reset velocity before jump to avoid residual forces
        rb.AddForce(new Vector2(horizontalForce * direction, jumpForce), ForceMode2D.Impulse);
    }

    private bool PlayerInSight()
    {
        Vector2 boxCenter = new Vector2(transform.position.x, transform.position.y);
        RaycastHit2D hit = Physics2D.BoxCast(
            boxCenter,
            new Vector2(detectionBoxWidth, detectionBoxHeight),
            0, Vector2.zero, 0, playerLayer);

        return hit.collider != null && hit.collider.CompareTag("Player");
    }

    private void FlipDirection()
    {
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }

    public void SetCurrentPhase(int phase)
    {
        currentPhase = phase;
        attackIndex = 0;

        switch (currentPhase)
        {
            case 1:
                currentAttackSequence = phase1Sequence;
                break;
            case 2:
                currentAttackSequence = phase2Sequence;
                break;
            case 3:
                currentAttackSequence = phase3Sequence;
                break;
            default:
                Debug.LogWarning("Unknown phase: " + currentPhase);
                break;
        }
    }

    private IEnumerator StartCooldown()
    {
        isOnCooldown = true;
        yield return new WaitForSeconds(attackCooldown);
        isOnCooldown = false;
    }

    private void FacePlayer()
    {
        if (!IsFacingPlayer())
        {
            FlipDirection();
        }
    }

    private bool IsFacingPlayer()
    {
        float directionToPlayer = player.position.x - transform.position.x;
        return (directionToPlayer > 0 && transform.localScale.x > 0) ||
               (directionToPlayer < 0 && transform.localScale.x < 0);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && collision == hitBox)
        {
            Health playerHealth = collision.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(playerDamage);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector2 boxCenter = new Vector2(transform.position.x, transform.position.y);
        Gizmos.DrawWireCube(
            boxCenter,
            new Vector2(detectionBoxWidth, detectionBoxHeight)
        );
    }
}
