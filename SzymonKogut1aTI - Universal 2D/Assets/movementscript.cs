using UnityEngine;

public class movementscript : MonoBehaviour
{
    [Header("Movement")]
    public float torqueForce = 30f;
    public float acceleration = 8f;
    public float maxSpeed = 10f;

    [Header("Jump")]
    public float jumpForce = 8f;

    private Rigidbody2D rb;
    private float currentTorque;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        float move = 0f;

        // A / D movement
        if (Input.GetKey(KeyCode.A))
            move = -1f;
        if (Input.GetKey(KeyCode.D))
            move = 1f;

        // Smooth acceleration
        float targetTorque = -move * torqueForce;
        currentTorque = Mathf.Lerp(currentTorque, targetTorque, acceleration * Time.deltaTime);

        rb.AddTorque(currentTorque);

        // Limit speed
        if (Mathf.Abs(rb.linearVelocity.x) > maxSpeed)
        {
            rb.linearVelocity = new Vector2(
                Mathf.Sign(rb.linearVelocity.x) * maxSpeed,
                rb.linearVelocity.y
            );
        }

        // Ground check (simple but works)
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, 0.6f);

        // Jump
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }
}