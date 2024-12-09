using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(BoxCollider2D))]
public class FlipColliderWithSprite : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollider;
    private Vector2 originalOffset;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();

        // Store the original offset
        originalOffset = boxCollider.offset;
    }

    void Update()
    {
        // Check if the character is flipped and adjust the collider offset accordingly
        if (spriteRenderer.flipX)
        {
            boxCollider.offset = new Vector2(-originalOffset.x, originalOffset.y); // Flip the offset horizontally
        }
        else
        {
            boxCollider.offset = originalOffset; // Set to original offset when not flipped
        }
    }
}
