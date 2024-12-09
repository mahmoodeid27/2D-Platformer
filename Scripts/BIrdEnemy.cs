using System.Collections;
using UnityEngine;

public class BirdEnemy : MonoBehaviour
{
    [Header("Bird Settings")]
    [SerializeField] private bool isBird = false; // Toggle to activate bird behavior
    [SerializeField] private float fallSpeed = 5f; // Adjustable fall speed

    [Header("Animations")]
    private Animator anim;

    [Header("Detection Settings")]
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private float detectionRange = 2f;

    [Header("Ground Settings")]
    [SerializeField] private LayerMask groundLayer; // Layer to detect the ground
    private bool hasTouchedGround = false;

    private Rigidbody2D rb;
    private bool hasDropped = false; // To ensure the bird drops only once

    private void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (!isBird || hasDropped) return;

        if (PlayerBelow())
        {
            Debug.Log("Player detected below. Falling now.");
            anim.SetTrigger("Fall");
            rb.bodyType = RigidbodyType2D.Dynamic; // Enable dynamic behavior
            rb.gravityScale = fallSpeed; // Adjust gravity to control fall speed
            hasDropped = true; // Prevent multiple drops
        }
    }

    private bool PlayerBelow()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, detectionRange, playerLayer);
        return hit.collider != null;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (hasTouchedGround) return; // Ensure this triggers only once

        // Check if the bird hit the ground
        if (groundLayer == (groundLayer | (1 << collision.gameObject.layer)))
        {
            hasTouchedGround = true;
            anim.SetTrigger("Ground"); // Trigger ground animation
        }
    }

    public void TriggerDeath()
    {
        anim.SetTrigger("Die");
        rb.velocity = Vector2.zero; // Stop movement
        rb.isKinematic = true; // Disable physics
        GetComponent<Collider2D>().enabled = false; // Disable collider
        Destroy(gameObject, 1f); // Destroy after animation
    }
}
