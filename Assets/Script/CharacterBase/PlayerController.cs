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

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private BoxCollider2D boxCol;


    private bool isGrounded;
    private bool isFacingRight = true;


    private Vector2 originalColliderOffset;
    private Vector3 originalGroundCheckLocalPos;

    public float AIXInput { get; set; } = 0f;
    public bool UseAIControl = false;

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
    
    // Xử lý input người chơi
    void HandleInput()
    {
        if (UseAIControl) return;
        // Jump input
        if ((Input.GetKeyDown(jumpKey1) || Input.GetKeyDown(jumpKey2)) && isGrounded)
        {
            Jump();
        }
    }

    // Di chuyển người chơi
    void MovePlayer()
    {
        float moveInput = UseAIControl ? AIXInput : Input.GetAxisRaw("Horizontal");

        // Apply movement
        rb.linearVelocity = new Vector2(moveInput * playerSpeed, rb.linearVelocity.y);

        // Handle flipping
        if (moveInput < 0 && isFacingRight)
            Flip(false);
        else if (moveInput > 0 && !isFacingRight)
            Flip(true);
    }

    // Nhảy
    public void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
    }

    // Kiểm tra xem người chơi có đang chạm đất không
    void CheckGrounded()
    {
        if (groundCheck == null) return;

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    // Cập nhật trạng thái animation
    void UpdateAnimation()
    {
        if (animator == null) return;

        animator.SetBool("IsWalking", Mathf.Abs(rb.linearVelocity.x) > 0.1f);
        animator.SetBool("IsJumping", !isGrounded);
    }

    // Lấy thông tin hướng người chơi
    public bool IsFacingRight => isFacingRight;

    // Lật hướng người chơi
    public void Flip(bool faceRight)
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

    // Lấy vận tốc hiện tại
    public Vector2 GetVelocity()
    {
        return rb.linearVelocity;
    }

    // Kiểm tra xem người chơi có đang di chuyển lên không (cho OneWayPlatform)
    public bool IsMovingUp()
    {
        bool movingUp = rb.linearVelocity.y > 0.1f;
        return movingUp;
    }

    // Kiểm tra xem người chơi có đang di chuyển xuống không (cho OneWayPlatform)
    public bool IsGrounded()
    {
        return isGrounded;
    }

    // Kiểm tra xem người chơi có đang nhấn nút xuống không (cho OneWayPlatform)
    public bool IsPressingDown()
    {
        bool pressing = Input.GetKey(dropKey) || Input.GetAxisRaw("Vertical") < -0.5f;
        return pressing;
    }

    // Lấy Renderer của người chơi
    public Renderer GetRenderer()
    {
        return spriteRenderer;
    }

    // Lấy Transform của người chơi
    public Transform GetTransform()
    {
        return transform;
    }

    // Vẽ Gizmos để kiểm tra ground check và hướng vận tốc
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
