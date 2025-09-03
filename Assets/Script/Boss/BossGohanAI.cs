using UnityEngine;
using System.Collections;

public class BossGohanAI : MonoBehaviour
{
    [Header("AI Settings")]
    public LayerMask targetLayer;
    public float detectionRange = 8f;
    public float decisionInterval = 0.4f;

    [Header("Movement Settings")]
    public float minDistanceToTarget = 3f; 
    public float jumpHeightThreshold = 1.5f;
    public float idleTime = 2f;

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

    // Phân tích và cập nhật mục tiêu hiện tại
    void AnalyzeTarget()
    {
        Transform newTarget = FindClosestTarget();
        
        if (newTarget == null)
        {
            if (target != null) 
            {
                StartIdleState();
            }
            target = null;
            return;
        }

        target = newTarget;
        isIdle = false;

        UpdateFacingDirection();
    }
    
    // Cập nhật hướng đối mặt dựa trên vị trí mục tiêu
    void UpdateFacingDirection()
    {
        if (target == null || playerController == null) return;

        float dir = target.position.x - transform.position.x;
        if (dir > 0.1f && !playerController.IsFacingRight)
            playerController.Flip(true);
        else if (dir < -0.1f && playerController.IsFacingRight)
            playerController.Flip(false);
    }

    // Bắt đầu trạng thái nhàn rỗi
    void StartIdleState()
    {
        isIdle = true;
        idleTimer = idleTime;
        playerController.AIXInput = 0;
    }
    
    // Chạy logic AI chính
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
    
    // Xử lý trạng thái nhàn rỗi
    void HandleIdleState()
    {
        idleTimer -= Time.deltaTime;
        if (idleTimer <= 0f)
        {
            isIdle = false;
        }
    }

    // Quyết định hành động dựa trên vị trí của mục tiêu
    void MakeDecision()
    {
        if (target == null || playerController == null || skillInput == null) return;

        float horizontalDistance = Mathf.Abs(target.position.x - transform.position.x);
        float verticalDistance = target.position.y - transform.position.y;
        float moveDir = Mathf.Sign(target.position.x - transform.position.x);

        if (horizontalDistance > minDistanceToTarget)
        {
            bool shouldMove = (moveDir > 0 && playerController.IsFacingRight) || (moveDir < 0 && !playerController.IsFacingRight);
            playerController.AIXInput = shouldMove ? moveDir : 0;
        }
        else
        {
            playerController.AIXInput = 0;
        }

        if (verticalDistance > jumpHeightThreshold && playerController.IsGrounded())
        {
            playerController.Jump();
        }

        float distance = Vector2.Distance(transform.position, target.position);
        
        if (distance <= 1.5f && skillInput.CanUseSkill("H"))
        {
            UpdateFacingDirectionForSkill();
            skillInput.TriggerSkill("H");
        }
        else if (distance <= 6f && skillInput.CanUseSkill("J"))
        {
            UpdateFacingDirectionForSkill();
            skillInput.TriggerSkill("J");
        }
        else if (distance <= 5f && skillInput.CanUseSkill("K"))
        {
            UpdateFacingDirectionForSkill();
            skillInput.TriggerSkill("K");
        }
        else if (distance <= detectionRange && skillInput.CanUseSkill("L"))
        {
            UpdateFacingDirectionForSkill();
            skillInput.TriggerSkill("L");
        }
    }
    
    // Cập nhật hướng đối mặt trước khi sử dụng kỹ năng
    void UpdateFacingDirectionForSkill()
    {
        if (target == null || playerController == null) return;

        float dir = target.position.x - transform.position.x;
        
        if (dir > 0.05f && !playerController.IsFacingRight)
        {
            playerController.Flip(true);
        }
        else if (dir < -0.05f && playerController.IsFacingRight)
        {
            playerController.Flip(false);
        }

        if (skillInput != null)
        {
            var facingRightField = skillInput.GetType().GetField("isFacingRight");
            if (facingRightField != null)
            {
                facingRightField.SetValue(skillInput, playerController.IsFacingRight);
            }
        }
    }

    //Tim target gần nhất trong phạm vi 'detectionRange'
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

    // Vẽ phạm vi phát hiện và khoảng cách tối thiểu trong Scene view
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, minDistanceToTarget);

        if (target != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, target.position);
            
            Vector3 direction = (target.position - transform.position).normalized;
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(transform.position, direction * 2f);
        }
    }
}