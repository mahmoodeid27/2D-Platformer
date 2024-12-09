using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthCollectible : MonoBehaviour
{
    [SerializeField] private float healthValue;
    private bool increasingSize;
    private float biggestSize;
    private float smallestSize;
    [SerializeField] private float scaleSpeed;
    [SerializeField] private float scaleAmount;

    private void Awake()
    {
        biggestSize = transform.localScale.x * scaleAmount;
        smallestSize = transform.localScale.x;
    }

    private void Update()
    {
        if (increasingSize)
        {
            // Increase size
            transform.localScale = new Vector3(
                Mathf.Min(transform.localScale.x + scaleSpeed * Time.deltaTime, biggestSize),
                Mathf.Min(transform.localScale.y + scaleSpeed * Time.deltaTime, biggestSize),
                Mathf.Min(transform.localScale.z + scaleSpeed * Time.deltaTime, biggestSize)
            );

            // Check if we've reached the biggest size
            if (transform.localScale.x >= biggestSize)
            {
                increasingSize = false; // Stop increasing size
            }
        }
        else
        {
            // Decrease size
            transform.localScale = new Vector3(
                Mathf.Max(transform.localScale.x - scaleSpeed * Time.deltaTime, smallestSize),
                Mathf.Max(transform.localScale.y - scaleSpeed * Time.deltaTime, smallestSize),
                Mathf.Max(transform.localScale.z - scaleSpeed * Time.deltaTime, smallestSize)
            );

            // Check if we've reached the smallest size
            if (transform.localScale.x <= smallestSize)
            {
                increasingSize = true; // Start increasing size again
            }
        }

    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            collision.GetComponent<Health>().AddHealth(healthValue);
            gameObject.SetActive(false);
        }
    }
}
