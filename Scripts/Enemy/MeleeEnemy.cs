using System.Collections;
using UnityEngine;

public class MeleeEnemy : MonoBehaviour
{
    [Header("Enemy Health")]
    [SerializeField] private int health = 3;  // This can be omitted if the Health component is handling health

    [Header("Attack Parameters")]
    [SerializeField] private float attackCooldown = 2f;  // Adjust the attack cooldown
    [SerializeField] private float range;
    [SerializeField] private int damage = 1;

    [Header("Collider Parameters")]
    [SerializeField] private float colliderDistance;
    [SerializeField] private BoxCollider2D boxCollider;

    [Header("Player Layer")]
    [SerializeField] private LayerMask playerLayer;
    private float cooldownTimer = Mathf.Infinity;

    // References
    private Animator anim;
    private Health playerHealth;
    private EnemyPatrol enemyPatrol;

    private bool isAttacking = false; // Track if the enemy is currently in an attack state

    private void Awake()
    {
        anim = GetComponent<Animator>();
        enemyPatrol = GetComponentInParent<EnemyPatrol>();
    }

    private void Update()
    {
        cooldownTimer += Time.deltaTime;

        // Attack only when player is in sight and cooldown allows
        if (PlayerInSight())
        {
            if (cooldownTimer >= attackCooldown)
            {
                cooldownTimer = 0;
                anim.SetTrigger("Attack");  // Trigger the attack animation
            }
        }

        // Enable/Disable patrol based on player visibility
        if (enemyPatrol != null)
            enemyPatrol.enabled = !PlayerInSight();
    }

    private bool PlayerInSight()
    {
        RaycastHit2D hit = Physics2D.BoxCast(
            boxCollider.bounds.center + transform.right * range * transform.localScale.x * colliderDistance,
            new Vector3(boxCollider.bounds.size.x * range, boxCollider.bounds.size.y, boxCollider.bounds.size.z),
            0, Vector2.left, 0, playerLayer);

        if (hit.collider != null)
        {
            playerHealth = hit.transform.GetComponent<Health>();
            return playerHealth != null; // Return true if Health component exists on player
        }

        playerHealth = null; // Reset if player is not detected
        return false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(boxCollider.bounds.center + transform.right * range * transform.localScale.x * colliderDistance,
            new Vector3(boxCollider.bounds.size.x * range, boxCollider.bounds.size.y, boxCollider.bounds.size.z));
    }

    // Method to start the attack; called by an animation event at the beginning of the attack animation
    public void StartAttack()
    {
        isAttacking = true;  // Begin attack state
    }

    // Method to deal damage to the player; called by an animation event at the moment of impact
    private void DamagePlayer()
    {
        if (isAttacking && playerHealth != null)
        {
            playerHealth.TakeDamage(damage);  // Deal damage to the player
            isAttacking = false;  // Reset isAttacking to prevent multiple hits in one attack
        }
    }

    // Handle player collision with the enemy
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            Health playerHealth = collision.collider.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);  // Deal damage to the player on collision
            }
        }
    }

    // Method to trigger hurt animation
    public void TriggerHurtAnimation()
    {
        if (anim != null)
        {
            anim.SetTrigger("hurt");
        }
    }

    // Method to handle death and destruction
    public void TriggerDeath()
    {
        if (anim != null)
        {
            anim.SetTrigger("die");  // Trigger death animation
        }

        // Disable collider to prevent further interactions
        if (GetComponent<Collider2D>() != null)
        {
            GetComponent<Collider2D>().enabled = false;
        }

        // Disable Rigidbody to prevent falling or movement
        if (GetComponent<Rigidbody2D>() != null)
        {
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            rb.velocity = Vector2.zero;  // Stop any movement
            rb.gravityScale = 0;         // Disable gravity
            rb.isKinematic = true;       // Make the Rigidbody kinematic (non-physical interactions)
        }

        // Disable any additional movement or attack scripts
        if (enemyPatrol != null)
            enemyPatrol.enabled = false;

        this.enabled = false;  // Disable this script

        // Destroy the game object after a short delay to allow death animation to play
        Destroy(gameObject, 1f);  // Delay destruction to allow death animation
    }
}
