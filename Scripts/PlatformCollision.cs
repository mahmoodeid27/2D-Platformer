using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformCollision : MonoBehaviour
{
    public string playerTag = "Player";

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag(playerTag) && gameObject.activeInHierarchy)
        {
            // Set the player as a child of the platform
            collision.transform.SetParent(transform);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag(playerTag) && gameObject.activeInHierarchy)
        {
            // Remove the player from the platform's hierarchy
            collision.transform.SetParent(null);
        }
    }
}
