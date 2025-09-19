using UnityEngine;

public class KnockbackController : MonoBehaviour
{
    public static void ApplyKnockback(GameObject target, Vector2 sourcePosition, float force, float duration)
    {
        if (target == null) return;

        // Tìm Rigidbody2D trên đối tượng
        Rigidbody2D rb = target.GetComponent<Rigidbody2D>();
        if (rb == null) return;

        // Tính toán hướng knockback (từ nguồn đến mục tiêu)
        Vector2 direction = ((Vector2)target.transform.position - sourcePosition).normalized;
        
        // Áp dụng lực
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(direction * force, ForceMode2D.Impulse);
        
        // Đặt lại velocity sau khoảng thời gian duration
        MonoBehaviour mono = target.GetComponent<MonoBehaviour>();
        if (mono != null)
        {
            mono.StartCoroutine(ResetVelocityAfterDelay(rb, duration));
        }
    }

    private static System.Collections.IEnumerator ResetVelocityAfterDelay(Rigidbody2D rb, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }
    }
}
