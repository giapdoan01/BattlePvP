using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D), typeof(SpriteRenderer))]
public class PlayerController : MonoBehaviour
{
    [Header("🏃 Movement Settings")]
    [SerializeField] private float playerSpeed = 5f;
    [SerializeField] private float jumpForce = 10f;

    [Header("🌍 Ground Detection")]
    [SerializeField] private LayerMask groundLayer = -1;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;

    [Header("🎮 Controls")]
    [SerializeField] private KeyCode jumpKey1 = KeyCode.W;
    [SerializeField] private KeyCode jumpKey2 = KeyCode.Space;
    [SerializeField] private KeyCode dropKey = KeyCode.S;

    // Components
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private BoxCollider2D boxCol;

    // States
    private bool isGrounded;
    private bool isFacingRight = true;

    // Original positions (for flipping)
    private Vector2 originalColliderOffset;
    private Vector3 originalGroundCheckLocalPos;

    private void Awake()
    {
        // Get all components
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        boxCol = GetComponent<BoxCollider2D>();

        // Save original positions
        originalColliderOffset = boxCol.offset;

        if (groundCheck != null)
        {
            originalGroundCheckLocalPos = groundCheck.localPosition;
        }
        else
        {
            Debug.LogError("groundCheck not attached in Inspector");
        }
    }

    void Update()
    {
        HandleInput();
        CheckGrounded();
        UpdateAnimation();
    }

    void FixedUpdate()
    {
        MovePlayer();
    }

    void HandleInput()
    {
        // Jump input
        if ((Input.GetKeyDown(jumpKey1) || Input.GetKeyDown(jumpKey2)) && isGrounded)
        {
            Jump();
        }
    }

    void MovePlayer()
    {
        float moveInput = Input.GetAxisRaw("Horizontal");

        // Apply movement
        rb.linearVelocity = new Vector2(moveInput * playerSpeed, rb.linearVelocity.y);

        // Handle flipping
        if (moveInput < 0 && isFacingRight)
            Flip(false);
        else if (moveInput > 0 && !isFacingRight)
            Flip(true);
    }

    void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
    }

    void CheckGrounded()
    {
        if (groundCheck == null) return;

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    void UpdateAnimation()
    {
        if (animator == null) return;

        animator.SetBool("IsWalking", Mathf.Abs(rb.linearVelocity.x) > 0.1f);
        animator.SetBool("IsJumping", !isGrounded);
    }

    void Flip(bool faceRight)
    {
        isFacingRight = faceRight;
        spriteRenderer.flipX = !isFacingRight;

        // Flip collider offset
        Vector2 o = originalColliderOffset;
        o.x = Mathf.Abs(originalColliderOffset.x) * (isFacingRight ? 1f : -1f);
        boxCol.offset = o;

        // Flip ground check position
        if (groundCheck != null)
        {
            Vector3 p = originalGroundCheckLocalPos;
            p.x = Mathf.Abs(originalGroundCheckLocalPos.x) * (isFacingRight ? 1f : -1f);
            groundCheck.localPosition = p;
        }
    }

    // ✅ PUBLIC METHODS CHO ONEWAYPLATFORM
    public Vector2 GetVelocity()
    {
        return rb.linearVelocity;
    }

    public bool IsMovingUp()
    {
        bool movingUp = rb.linearVelocity.y > 0.1f;
        return movingUp;
    }

    public bool IsGrounded()
    {
        return isGrounded;
    }

    public bool IsPressingDown()
    {
        bool pressing = Input.GetKey(dropKey) || Input.GetAxisRaw("Vertical") < -0.5f;
        return pressing;
    }

    public Renderer GetRenderer()
    {
        return spriteRenderer;
    }

    public Transform GetTransform()
    {
        return transform;
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);

            // Draw velocity direction
            Gizmos.color = Color.yellow;
            if (rb != null)
                Gizmos.DrawRay(transform.position, rb.linearVelocity);
        }
    }
}
