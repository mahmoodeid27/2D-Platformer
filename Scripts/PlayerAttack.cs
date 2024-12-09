using UnityEngine;
using System.Collections;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private float attackCooldown;
    [SerializeField] private int attackDamage;
    [SerializeField] private Vector2 attackOffset;
    [SerializeField] private Vector2 attackBoxSize;

    [Header("Attack Sound")]
    [SerializeField] private AudioClip attackSound;

    private Animator anim;
    private float cooldownTimer = Mathf.Infinity;
    private bool isFacingRight = true;
    public Animator playerAnimator;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        // Track whether the player is facing right or left
        if (Input.GetAxis("Horizontal") > 0)
            isFacingRight = true;
        else if (Input.GetAxis("Horizontal") < 0)
            isFacingRight = false;

        // Handle attack input only if cooldown is complete
        if (Input.GetKeyDown(KeyCode.Q) && cooldownTimer >= attackCooldown)
        {
            Attack();
        }

        // Update cooldown timer
        cooldownTimer += Time.deltaTime;
    }

    private void Attack()
    {
        // Trigger attack animation
        anim.SetTrigger("Attack");
        Debug.Log("Player attacked!");

        // Play attack sound
        AudioManager.instance.PlaySound(attackSound);

        // Reset cooldown timer
        cooldownTimer = 0;
    }




    public void ApplyDamage()
    {
        Vector2 adjustedAttackOffset = isFacingRight ? attackOffset : new Vector2(-attackOffset.x, attackOffset.y);
        Vector2 attackPosition = (Vector2)transform.position + adjustedAttackOffset;

        Collider2D[] hitEnemies = Physics2D.OverlapBoxAll(attackPosition, attackBoxSize, 0);

        foreach (Collider2D enemy in hitEnemies)
        {
            if (enemy.CompareTag("Enemy"))
            {
                // Check if the enemy has a Health component (for regular enemies)
                Health enemyHealth = enemy.GetComponent<Health>();
                if (enemyHealth != null)
                {
                    enemyHealth.TakeDamage((float)attackDamage);
                    Debug.Log("Regular enemy hit!");
                }

                // Check if the enemy has a BossController component (for the boss)
                BossController bossController = enemy.GetComponent<BossController>();
                if (bossController != null)
                {
                    bossController.TakeDamage(attackDamage);
                    Debug.Log("Boss hit!");
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector2 adjustedAttackOffset = isFacingRight ? attackOffset : new Vector2(-attackOffset.x, attackOffset.y);
        Vector2 attackPosition = (Vector2)transform.position + adjustedAttackOffset;
        Gizmos.DrawWireCube(attackPosition, attackBoxSize);
    }

    private IEnumerator PlayAttackSoundWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        AudioManager.instance.PlaySound(attackSound);
    }
}
