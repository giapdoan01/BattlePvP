using UnityEngine;
using System.Collections;

public class EnemyShootBall : MonoBehaviour
{
    [Header("Skill Settings")]
    public float projectileSpeed = 8f;
    public float lifeTime = 5f;
    public float damage = 10f;

    private Rigidbody2D rb;
    private GameObject owner;
    private bool isActive = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Chỉ cần method này để activate skill
    public void ActivateSkill(GameObject skillOwner, Vector2 direction)
    {
        owner = skillOwner;

        // Setup components nếu chưa có
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();

        // Set velocity
        rb.linearVelocity = direction.normalized * projectileSpeed;

        // Reset trạng thái
        isActive = true;

        // Stop coroutine cũ nếu có
        StopAllCoroutines();

        // Tự deactivate sau lifeTime
        StartCoroutine(DeactivateAfterTime());

        Debug.Log($"EnemyShootBall activated with speed: {projectileSpeed}, direction: {direction}");
    }

    IEnumerator DeactivateAfterTime()
    {
        yield return new WaitForSeconds(lifeTime);
        DeactivateSkill();
    }

    void DeactivateSkill()
    {
        isActive = false;
        if (rb != null)
            rb.linearVelocity = Vector2.zero;
        gameObject.SetActive(false);
        Debug.Log("EnemyShootBall deactivated");
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Bỏ qua nếu va vào chính chủ nhân
        if (other.gameObject == owner)
            return;

        if (other.CompareTag("Player"))
        {
            // Gây damage nếu có HealthController
            HealthController target = other.GetComponent<HealthController>();
            if (target != null)
            {
                target.TakeDamage(damage);
                Debug.Log($"Player took {damage} damage from enemy ball");
            }
            DeactivateSkill();
        }
        else if (other.CompareTag("Ground"))
        {
            DeactivateSkill();
        }
    }

    void OnDisable()
    {
        isActive = false;
        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        StopAllCoroutines();
    }
}
