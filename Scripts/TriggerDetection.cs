using UnityEngine;

public class TriggerDetection : MonoBehaviour
{
    public bool isWithinRange { get; private set; } = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isWithinRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isWithinRange = false;
        }
    }
}
