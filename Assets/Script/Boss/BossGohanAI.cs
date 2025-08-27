using UnityEngine;
using System.Collections;

public class BossGohanAI : MonoBehaviour
{
    [Header("AI Settings")]
    public LayerMask targetLayer;
    public float detectionRange = 8f;
    public float decisionInterval = 0.4f;

    [Header("Movement Settings")]
    public float minDistanceToTarget = 3f;    // Khoảng cách tối thiểu với target
    public float jumpHeightThreshold = 1.5f;  // Khoảng cách Y để quyết định nhảy
    public float idleTime = 2f;               // Thời gian đứng yên khi mất target

    private Transform target;
    private PlayerController playerController;
    private SkillInputHandler skillInput;
    private float decisionTimer;
    private float idleTimer;
    private bool isIdle = false;

    void Awake()
    {
        playerController = GetComponent<PlayerController>();
        skillInput = GetComponent<SkillInputHandler>();
        if (playerController != null)
            playerController.UseAIControl = true;
        if (skillInput != null)
            skillInput.UseAIControl = true;
    }

    void Update()
    {
        AnalyzeTarget();
        RunAI();
    }

    void AnalyzeTarget()
    {
        Transform newTarget = FindClosestTarget();
        
        // Nếu mất target
        if (newTarget == null)
        {
            if (target != null) // Vừa mới mất target
            {
                StartIdleState();
            }
            target = null;
            return;
        }

        // Có target mới
        target = newTarget;
        isIdle = false;

        // Quay mặt về phía target
        if (playerController != null)
        {
            float dir = target.position.x - transform.position.x;
            if (dir > 0 && !playerController.IsFacingRight)
                playerController.Flip(true);
            else if (dir < 0 && playerController.IsFacingRight)
                playerController.Flip(false);
        }
    }

    void StartIdleState()
    {
        isIdle = true;
        idleTimer = idleTime;
        playerController.AIXInput = 0; // Dừng di chuyển
    }

    void RunAI()
    {
        if (isIdle)
        {
            HandleIdleState();
            return;
        }

        decisionTimer -= Time.deltaTime;
        if (decisionTimer <= 0f)
        {
            MakeDecision();
            decisionTimer = decisionInterval;
        }
    }

    void HandleIdleState()
    {
        idleTimer -= Time.deltaTime;
        if (idleTimer <= 0f)
        {
            isIdle = false;
        }
    }

    void MakeDecision()
    {
        if (target == null || playerController == null || skillInput == null) return;

        float horizontalDistance = Mathf.Abs(target.position.x - transform.position.x);
        float verticalDistance = target.position.y - transform.position.y;
        float moveDir = Mathf.Sign(target.position.x - transform.position.x);

        // Chỉ di chuyển về phía target, không đi lùi
        if (horizontalDistance > minDistanceToTarget)
        {
            bool shouldMove = (moveDir > 0 && playerController.IsFacingRight) || (moveDir < 0 && !playerController.IsFacingRight);
            playerController.AIXInput = shouldMove ? moveDir : 0;
        }
        else
        {
            playerController.AIXInput = 0;
        }

        // Xử lý nhảy
        if (verticalDistance > jumpHeightThreshold && playerController.IsGrounded())
        {
            playerController.Jump();
        }

        // Đảm bảo boss quay mặt đúng hướng trước khi dùng skill
        if (playerController != null)
        {
            float dir = target.position.x - transform.position.x;
            if (dir > 0 && !playerController.IsFacingRight)
                playerController.Flip(true);
            else if (dir < 0 && playerController.IsFacingRight)
                playerController.Flip(false);
        }

        // Xử lý skill theo khoảng cách
        float distance = Vector2.Distance(transform.position, target.position);
        
        if (distance <= 1.5f && skillInput.CanUseSkill("H"))
            skillInput.TriggerSkill("H");
        else if (distance <= 6f && skillInput.CanUseSkill("J"))
            skillInput.TriggerSkill("J");
        else if (distance <= 5f && skillInput.CanUseSkill("K"))
            skillInput.TriggerSkill("K");
        else if (distance <= detectionRange && skillInput.CanUseSkill("L"))
            skillInput.TriggerSkill("L");
    }

    Transform FindClosestTarget()
    {
        Collider2D[] targets = Physics2D.OverlapCircleAll(transform.position, detectionRange, targetLayer);
        Transform closest = null;
        float minDistance = Mathf.Infinity;
        foreach (Collider2D col in targets)
        {
            float distance = Vector2.Distance(transform.position, col.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closest = col.transform;
            }
        }
        return closest;
    }

    void OnDrawGizmosSelected()
    {
        // Visualize detection range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        
        // Visualize minimum distance
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, minDistanceToTarget);

        if (target != null)
        {
            // Visualize target line
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, target.position);
        }
    }
}
