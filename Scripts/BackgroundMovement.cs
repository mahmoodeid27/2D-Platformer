using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMovement : MonoBehaviour
{
    [SerializeField] private float backgroundSpeed = 1f;
    [SerializeField] private Transform playerTransform;

    private Vector3 lastPlayerPosition;

    void Start()
    {
        if (playerTransform == null)
        {
            Debug.LogError("Player Transform is not assigned to BackgroundMovement script!");
            enabled = false; // Disable this script if playerTransform is not set
            return;
        }

        lastPlayerPosition = playerTransform.position;
    }

    void LateUpdate()
    {
        if (playerTransform != null)
        {
            Vector3 playerMovement = playerTransform.position - lastPlayerPosition;
            
            // Only move background if player has moved horizontally
            if (Mathf.Abs(playerMovement.x) > 0.001f)
            {
                float moveAmount = playerMovement.x * backgroundSpeed;
                transform.position += new Vector3(moveAmount, 0, 0);
            }

            lastPlayerPosition = playerTransform.position;
        }
    }
}
