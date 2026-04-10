using UnityEngine;

public class BallController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 15f;
    public float maxSpeed = 15f;

    [Header("Jump")]
    public float jumpForce = 10f;
    public LayerMask groundLayer;
    public float groundCheckDistance = 0.2f;  // Small ray length from bottom of ball

    private Rigidbody2D rb;
    private float moveInput;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Horizontal input
        moveInput = Input.GetAxisRaw("Horizontal");

        // Jump only if grounded (and no queuing)
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W)) && IsGrounded())
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    void FixedUpdate()
    {
        // Movement (works mid‑air too, but jump is disabled)
        rb.AddForce(new Vector2(moveInput * moveSpeed, 0f), ForceMode2D.Force);

        // Limit horizontal speed
        if (Mathf.Abs(rb.linearVelocity.x) > maxSpeed)
        {
            rb.linearVelocity = new Vector2(Mathf.Sign(rb.linearVelocity.x) * maxSpeed, rb.linearVelocity.y);
        }
    }

    private bool IsGrounded()
    {
        // Cast a ray from the BOTTOM of the ball downward.
        // The ball's radius is 0.5, so bottom is at y - 0.5.
        Vector2 rayOrigin = transform.position + Vector3.down * 0.5f;
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, groundCheckDistance, groundLayer);

        // Visualize the ray (red in Scene view)
        Debug.DrawRay(rayOrigin, Vector2.down * groundCheckDistance, Color.red);

        return hit.collider != null;
    }

    private void OnDrawGizmosSelected()
    {
        // Draw a small sphere at the ray origin for debugging
        Gizmos.color = Color.yellow;
        Vector3 rayOrigin = transform.position + Vector3.down * 0.5f;
        Gizmos.DrawWireSphere(rayOrigin, 0.05f);
    }
}