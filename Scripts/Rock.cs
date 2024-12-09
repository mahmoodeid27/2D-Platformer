using UnityEngine;
using System.Collections;

public class Rock : MonoBehaviour
{
    private float fallSpeed;
    public float rotationSpeed = 50f; // Adjust rotation speed as desired

    // Method to set the fall speed from AttackEffects
    public void SetFallSpeed(float speed)
    {
        fallSpeed = speed;
    }

    private void Update()
    {
        // Move the rock downward at the set fall speed
        transform.position += Vector3.down * fallSpeed * Time.deltaTime;

        // Rotate the rock slowly while it falls
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            // Check if the collided object is a different enemy (not another rock)
            Rock rockScript = collision.gameObject.GetComponent<Rock>();
            if (rockScript == null)
            {
                // If it's an enemy (not a rock), destroy this rock
                DestroyRock();
            }
        }
        else if (collision.gameObject.CompareTag("Player"))
        {
            // If the rock hits the player, apply damage but let it continue falling
            Health playerHealth = collision.gameObject.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(1); // Adjust damage value as needed
            }
            // Do not destroy the rock here; let it fall to the ground
        }
        else if (collision.gameObject.CompareTag("Ground"))
        {
            // If the rock hits the ground, stop movement and rotation, then start the delayed destruction
            StartCoroutine(StopAndDestroy());
        }
    }

    private IEnumerator StopAndDestroy()
    {
        // Stop the rock from falling and rotating
        fallSpeed = 0f;
        rotationSpeed = 0f;

        // Wait for 0.2 seconds
        yield return new WaitForSeconds(0.2f);

        // Destroy the rock after the delay
        Destroy(gameObject);
    }

    private void DestroyRock()
    {
        Destroy(gameObject); // Destroy the rock
    }
}
