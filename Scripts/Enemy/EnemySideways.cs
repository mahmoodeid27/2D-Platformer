using UnityEngine;

public class EnemySideways : MonoBehaviour
{
    // Enum to specify the movement type (horizontal or vertical)
    public enum MovementType
    {
        Horizontal,
        Vertical
    }

    public MovementType movementType; // Set this in the Inspector for each object

    [SerializeField] private float damage = 10f; // Damage dealt to the player
    [SerializeField] private float movementDistance = 5f; // How far the object moves
    [SerializeField] private float speed = 2f; // Speed of movement
    [SerializeField] private bool startInNegativeDirection = true; // Specify starting direction in the Inspector
    private bool movingInNegativeDirection; // Track movement direction
    private float negativeEdge; // Left or bottom edge of movement
    private float positiveEdge; // Right or top edge of movement

    private void Awake()
    {
        // Initialize movement boundaries based on the chosen movement type
        if (movementType == MovementType.Horizontal)
        {
            negativeEdge = transform.position.x - movementDistance;
            positiveEdge = transform.position.x + movementDistance;
        }
        else if (movementType == MovementType.Vertical)
        {
            negativeEdge = transform.position.y - movementDistance;
            positiveEdge = transform.position.y + movementDistance;
        }

        // Set the initial movement direction
        movingInNegativeDirection = startInNegativeDirection;
    }

    private void Update()
    {
        // Move based on the chosen movement type
        if (movementType == MovementType.Horizontal)
        {
            MoveHorizontally();
        }
        else if (movementType == MovementType.Vertical)
        {
            MoveVertically();
        }
    }

    private void MoveHorizontally()
    {
        if (movingInNegativeDirection)
        {
            if (transform.position.x > negativeEdge)
            {
                transform.position = new Vector3(transform.position.x - speed * Time.deltaTime, transform.position.y, transform.position.z);
            }
            else
            {
                movingInNegativeDirection = false;
            }
        }
        else
        {
            if (transform.position.x < positiveEdge)
            {
                transform.position = new Vector3(transform.position.x + speed * Time.deltaTime, transform.position.y, transform.position.z);
            }
            else
            {
                movingInNegativeDirection = true;
            }
        }
    }

    private void MoveVertically()
    {
        if (movingInNegativeDirection)
        {
            if (transform.position.y > negativeEdge)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y - speed * Time.deltaTime, transform.position.z);
            }
            else
            {
                movingInNegativeDirection = false;
            }
        }
        else
        {
            if (transform.position.y < positiveEdge)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y + speed * Time.deltaTime, transform.position.z);
            }
            else
            {
                movingInNegativeDirection = true;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Health playerHealth = collision.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
        }
    }
}
