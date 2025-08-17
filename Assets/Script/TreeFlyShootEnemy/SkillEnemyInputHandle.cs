using UnityEngine;

public class SkillEnemyInputHandle : MonoBehaviour
{
    [Header("Skill Settings")]
    public string skillKey = "ShootBall";
    public float attackCooldown = 5f;
    public Transform firePoint;

    [Header("Detection")]
    public float detectionRange = 10f;
    public LayerMask playerLayer = 1024; // Layer 10

    private SkillEnemyFactory skillFactory;
    private Transform targetPlayer;
    private Animator animator;

    // ✅ Sử dụng thời gian thực tế thay vì -999
    private float lastAttackTime = 0f;
    private bool canAttack = true; // ✅ Flag để kiểm soát attack

    void Start()
    {
        skillFactory = GetComponent<SkillEnemyFactory>();
        animator = GetComponent<Animator>();

        if (firePoint == null)
        {
            firePoint = transform;
        }

        // ✅ Set thời gian bắt đầu
        lastAttackTime = Time.time;
    }

    void Update()
    {
        DetectPlayer();

        if (targetPlayer != null)
        {
            // ✅ Tính toán thời gian chính xác
            float currentTime = Time.time;
            float timeSinceLastAttack = currentTime - lastAttackTime;


            // ✅ Kiểm tra cooldown chính xác
            if (canAttack && timeSinceLastAttack >= attackCooldown)
            {
                Attack();
            }
        }
    }

    void DetectPlayer()
    {
        // Tìm Player bằng tag
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            float distance = Vector2.Distance(transform.position, player.transform.position);
            if (distance <= detectionRange)
            {
                targetPlayer = player.transform;
                return;
            }
        }

        // Fallback: Physics2D
        Collider2D playerCollider = Physics2D.OverlapCircle(transform.position, detectionRange, playerLayer);
        targetPlayer = playerCollider != null ? playerCollider.transform : null;
    }

    void Attack()
    {
        if (targetPlayer == null || skillFactory == null || !canAttack) return;

        // ✅ Set flag và thời gian TRƯỚC khi attack
        canAttack = false;
        lastAttackTime = Time.time;

        Vector2 direction = (targetPlayer.position - firePoint.position).normalized;
        GameObject skill = skillFactory.CreateSkill(skillKey, gameObject, firePoint.position, direction);

        if (animator != null)
            animator.SetTrigger("Attack");

        // ✅ Bật lại flag sau cooldown
        Invoke(nameof(ResetAttack), attackCooldown);
    }

    // ✅ Reset attack flag
    void ResetAttack()
    {
        canAttack = true;
    }

    // ✅ Debug Gizmos
    void OnDrawGizmosSelected()
    {
        Gizmos.color = targetPlayer != null ? Color.red : Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        if (targetPlayer != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, targetPlayer.position);
        }

        if (firePoint != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(firePoint.position, 0.2f);
        }
    }

    // ✅ Cleanup khi destroy
    void OnDestroy()
    {
        CancelInvoke();
    }
}
