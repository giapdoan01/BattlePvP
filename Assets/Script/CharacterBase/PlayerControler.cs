using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D), typeof(SpriteRenderer))]
public class Player : MonoBehaviour
{
    [SerializeField] private float playerSpeed = 5f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private BoxCollider2D boxCol;

    private bool isGrounded;
    private bool isFacingRight = true;

    private Vector2 originalColliderOffset;
    private Vector3 originalGroundCheckLocalPos;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        boxCol = GetComponent<BoxCollider2D>();

        originalColliderOffset = boxCol.offset;

        if (groundCheck != null)
            originalGroundCheckLocalPos = groundCheck.localPosition;
    }

    void Update()
    {
        MovePlayer();
        JumpPlayer();
        UpdateAnimation();
    }

    void MovePlayer()
    {
        float moveInput = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2(moveInput * playerSpeed, rb.linearVelocity.y);

        if (moveInput < 0 && isFacingRight)
            Flip(false);
        else if (moveInput > 0 && !isFacingRight)
            Flip(true);
    }

    void JumpPlayer()
    {
        if (Input.GetKeyDown(KeyCode.W) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    void UpdateAnimation()
    {
        animator.SetBool("IsWalking", Mathf.Abs(rb.linearVelocity.x) > 0.1f);
        animator.SetBool("IsJumping", !isGrounded);
    }

    void Flip(bool faceRight)
    {
        isFacingRight = faceRight;
        spriteRenderer.flipX = !isFacingRight;

        Vector2 o = originalColliderOffset;
        o.x = Mathf.Abs(originalColliderOffset.x) * (isFacingRight ? 1f : -1f);
        boxCol.offset = o;

        if (groundCheck != null)
        {
            Vector3 p = originalGroundCheckLocalPos;
            p.x = Mathf.Abs(originalGroundCheckLocalPos.x) * (isFacingRight ? 1f : -1f);
            groundCheck.localPosition = p;
        }
    }
}
