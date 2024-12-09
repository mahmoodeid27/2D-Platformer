using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Serialize field makes speed changeable inside Unity 
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float speed;
    [SerializeField] private float jumpForce = 10f; // New variable for jump force
    private Rigidbody2D rb;
    private bool isGrounded;  // Check if the player is grounded
    private int jumpCount = 0; // Track number of jumps
    private int maxJumps = 2;  // Limit the number of jumps
    private float raycastDist = 1.3f;
    private Animator anim;
    private Health healthScript;

    private void Awake()
    {
        // Gets references for rigidbody and animator from object
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        healthScript = GetComponent<Health>();
        anim.ResetTrigger("Attack");  // Reset the Attack trigger at game start
        anim.ResetTrigger("hurt");
    }

    private void Update()
    {
        // Horizontal input with no smoothing
        float horizontalInput = Input.GetAxisRaw("Horizontal");

        // Jump logic with double jump limit
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log(isGrounded + " " + jumpCount);

            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, raycastDist, groundLayer);
            isGrounded = hit.collider != null;

            if (isGrounded)
            {
                jumpCount = 1; // The first jump is already done!
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);

                // Trigger the jump animation
                anim.SetTrigger("jump");  // Assuming the Animator has a "jump" trigger parameter
                Debug.Log("jump!");
            }
            else if (!isGrounded && jumpCount > 0 && jumpCount < maxJumps)
            {
                jumpCount++; // The air jump 
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);

                // Optionally trigger the jump animation here as well if you want to handle double jump
                anim.SetTrigger("jump");
                Debug.Log("jump!");
            }

            if (jumpCount > maxJumps)
            {
                jumpCount = 0;
            }
        }

        // Horizontal movement
        rb.velocity = new Vector2(horizontalInput * speed, rb.velocity.y);

        // Flipping player facing direction based on movement
        if (horizontalInput < 0)
        {
            // Moving left, flip the player
            transform.localScale = new Vector2(-1f, 1f);  // Flip the X scale to -1 to face left
        }
        else if (horizontalInput > 0)
        {
            // Moving right, face right
            transform.localScale = new Vector2(1f, 1f);  // Reset the X scale to 1 to face right
        }

        // Set animator parameters (run when horizontalInput is not zero)
        anim.SetBool("run", horizontalInput != 0);

        // Check if the player has landed (to transition back to idle)
        RaycastHit2D hitLanding = Physics2D.Raycast(transform.position, Vector2.down, raycastDist, groundLayer);
        isGrounded = hitLanding.collider != null;

        // If the player is grounded, reset jump trigger and go to idle
        if (isGrounded)
        {
            anim.ResetTrigger("jump");  // Reset the jump trigger to stop jump animation
            anim.SetBool("isGrounded", true);  // Optionally, you can set a grounded parameter if you have one
        }
        else
        {
            anim.SetBool("isGrounded", false);  // Set grounded to false while in the air
        }

    }

    public void StopMovement()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.velocity = Vector2.zero;
        anim.SetBool("run", false);
    }
}