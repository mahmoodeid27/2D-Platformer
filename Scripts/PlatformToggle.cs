using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformToggle : MonoBehaviour
{
    public bool isSolid = true; // Set initial state in the Inspector

    private BoxCollider2D platformCollider;
    private SpriteRenderer platformRenderer;

    private void Awake()
    {
        platformCollider = GetComponent<BoxCollider2D>();
        platformRenderer = GetComponent<SpriteRenderer>();

        UpdatePlatformState();
    }

    public void ToggleState()
    {
        isSolid = !isSolid;
        UpdatePlatformState();
    }

    private void UpdatePlatformState()
    {
        // Toggle collider
        platformCollider.enabled = isSolid;

        // Update color: solid = white, non-solid = red
        if (isSolid)
        {
            platformRenderer.color = new Color(1f, 1f, 1f, 1f); // White (solid)
        }
        else
        {
            platformRenderer.color = new Color(1f, 0f, 0f, 0.5f); // Red with transparency (non-solid)
        }
    }
}
