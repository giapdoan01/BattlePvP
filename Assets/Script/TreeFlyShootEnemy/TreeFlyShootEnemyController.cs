using UnityEngine;

public class TreeFlyShootEnemyController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float patrolRange = 10f;
    [SerializeField] private float waitTime = 1f;

    [Header("Combat Settings")]
    [SerializeField] private float detectionRange = 5f;
    [SerializeField] private LayerMask playerLayer;

    private Vector3 startPosition;
    private Vector3 leftBoundary;
    private Vector3 rightBoundary;
    private Vector3 targetPosition;

    private bool movingRight = true;
    private bool isWaiting = false;
    private float waitTimer = 0f;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    // Combat state
    private Transform targetPlayer;
    private bool isInCombat = false;

    void Start()
    {
        startPosition = transform.position;
        leftBoundary = new Vector3(startPosition.x - patrolRange / 2f, startPosition.y, startPosition.z);
        rightBoundary = new Vector3(startPosition.x + patrolRange / 2f, startPosition.y, startPosition.z);
        targetPosition = movingRight ? rightBoundary : leftBoundary;

        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        CheckForPlayer();

        if (isInCombat)
        {
            HandleCombat();
        }
        else if (isWaiting)
        {
            HandleWaiting();
        }
        else
        {
            HandleMovement();
        }
    }

    void CheckForPlayer()
    {
        Collider2D playerCol = Physics2D.OverlapCircle(transform.position, detectionRange, playerLayer);

        if (playerCol != null)
        {
            targetPlayer = playerCol.transform;
            isInCombat = true;
        }
        else
        {
            targetPlayer = null;
            isInCombat = false;
        }
    }

    void HandleCombat()
    {
        if (targetPlayer == null) return;

        // Dừng di chuyển
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);

        // Quay mặt về phía player
        Vector3 directionToPlayer = targetPlayer.position - transform.position;
        FacePlayer(directionToPlayer.x);
    }

    void HandleMovement()
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        rb.linearVelocity = new Vector2(direction.x * moveSpeed, rb.linearVelocity.y);

        FlipSprite(direction.x);

        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            StartWaiting();
        }
    }

    void HandleWaiting()
    {
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        waitTimer += Time.deltaTime;

        if (waitTimer >= waitTime)
        {
            ChangeDirection();
            isWaiting = false;
            waitTimer = 0f;
        }
    }

    void StartWaiting()
    {
        isWaiting = true;
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
    }

    void ChangeDirection()
    {
        movingRight = !movingRight;
        targetPosition = movingRight ? rightBoundary : leftBoundary;
    }

    void FlipSprite(float directionX)
    {
        if (spriteRenderer != null)
        {
            if (directionX < 0)
                spriteRenderer.flipX = false;
            else if (directionX > 0)
                spriteRenderer.flipX = true;
        }
    }

    void FacePlayer(float directionX)
    {
        if (spriteRenderer != null)
        {
            if (directionX < 0)
                spriteRenderer.flipX = false;
            else if (directionX > 0)
                spriteRenderer.flipX = true;
        }
    }

    void OnDrawGizmosSelected()
    {
        // Vẽ tầm phát hiện
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // Vẽ patrol range
        Gizmos.color = Color.blue;
        if (Application.isPlaying)
        {
            Gizmos.DrawLine(leftBoundary, rightBoundary);
        }
    }

    // Public method để các script khác có thể kiểm tra trạng thái
    public bool IsInCombat()
    {
        return isInCombat;
    }

    public Transform GetTargetPlayer()
    {
        return targetPlayer;
    }
}
