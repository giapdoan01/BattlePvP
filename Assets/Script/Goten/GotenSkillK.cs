using System.Collections;
using UnityEngine;

public class GotenSkillK : MonoBehaviour, ISkill
{
    [Header("Dash Settings")]
    [SerializeField] private AudioSource SkillKSoundSource; // Nguồn âm thanh để phát âm thanh khi phóng chiêu
    [SerializeField] private AudioClip SkillKSound; // Âm thanh khi phóng chiêu
    [SerializeField] private float damage = 25f;
    [SerializeField] private float dashDistance = 10f;
    [SerializeField] private float dashDuration = 0.3f;
    [SerializeField] private LayerMask obstacleLayerMask = -1;

    private GameObject owner;
    private bool facingRight;
    private Rigidbody2D ownerRigidbody;
    private Collider2D ownerCollider;
    private BoxCollider2D skillCollider;
    
    public void Initialize(GameObject skillOwner)
    {
        Initialize(skillOwner, true);
    }

    public void Initialize(GameObject skillOwner, bool ownerFacingRight)
    {
        owner = skillOwner;
        facingRight = ownerFacingRight;
        
        // Lấy component cần thiết
        ownerRigidbody = owner.GetComponent<Rigidbody2D>();
        ownerCollider = owner.GetComponent<Collider2D>();
        skillCollider = GetComponent<BoxCollider2D>();
        
        if (skillCollider == null)
        {
            // Tạo collider nếu không có
            skillCollider = gameObject.AddComponent<BoxCollider2D>();
            skillCollider.isTrigger = true;
            skillCollider.size = new Vector2(1.2f, 1.5f); // Điều chỉnh kích thước phù hợp
        }
        
        // Đặt vị trí ban đầu của skill object theo owner
        transform.position = owner.transform.position;
        
        // Flip sprite nếu cần
        if (!facingRight)
        {
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
        
        // Bắt đầu dash
        StartCoroutine(ExecuteDash());
    }
    
    private IEnumerator ExecuteDash()
    {
        // Tính toán điểm đến
        Vector3 startPosition = owner.transform.position;
        Vector3 direction = facingRight ? Vector3.right : Vector3.left;
        Vector3 rawDestination = startPosition + (direction * dashDistance);
        
        // Kiểm tra vị trí hợp lệ
        bool isValidPosition = IsValidDashPosition(rawDestination);
        Vector3 finalDestination = isValidPosition ? rawDestination : FindNearestValidPosition(rawDestination);
        
        // Nếu không thể dash, hủy skill
        if (Vector3.Distance(startPosition, finalDestination) < 0.1f)
        {
            DestroySelf();
            yield break;
        }
        
        // Tính toán vận tốc dash
        float dashSpeed = Vector3.Distance(startPosition, finalDestination) / dashDuration;
        Vector3 dashVelocity = direction * dashSpeed;
        
        // Thực hiện dash
        float elapsedTime = 0;
        if (SkillKSoundSource != null && SkillKSound != null)
        {
            SkillKSoundSource.PlayOneShot(SkillKSound);
        }
        
        while (elapsedTime < dashDuration)
        {
            if (owner == null) // Kiểm tra nếu owner bị hủy
            {
                DestroySelf();
                yield break;
            }

            // Tính toán vị trí mới dựa trên thời gian
            float t = elapsedTime / dashDuration;
            Vector3 newPosition = Vector3.Lerp(startPosition, finalDestination, t);

            // Di chuyển owner
            owner.transform.position = newPosition;

            // Đảm bảo vận tốc được duy trì
            if (ownerRigidbody != null)
            {
                ownerRigidbody.linearVelocity = new Vector2(dashVelocity.x, ownerRigidbody.linearVelocity.y);
            }

            // Cập nhật vị trí của skill object theo owner
            transform.position = owner.transform.position;

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        // Đảm bảo owner đến đúng vị trí cuối cùng
        owner.transform.position = finalDestination;
        
        // Dừng vận tốc của owner
        if (ownerRigidbody != null)
        {
            ownerRigidbody.linearVelocity = new Vector2(0, ownerRigidbody.linearVelocity.y);
        }
        
        // Hủy skill
        DestroySelf();
    }
    
    private bool IsValidDashPosition(Vector3 position)
    {
        if (ownerCollider == null) return true;

        bool ownerColliderWasEnabled = ownerCollider.enabled;
        ownerCollider.enabled = false;

        Collider2D[] hitColliders = Physics2D.OverlapBoxAll(
            position,
            ownerCollider.bounds.size,
            0f,
            obstacleLayerMask
        );

        ownerCollider.enabled = ownerColliderWasEnabled;

        foreach (Collider2D hitCollider in hitColliders)
        {
            if (!IsCameraBounds(hitCollider))
            {
                return false; 
            }
        }

        return true;
    }

    private Vector3 FindNearestValidPosition(Vector3 targetPosition)
    {
        Vector3 currentPosition = owner.transform.position;
        Vector3 directionVector = targetPosition - currentPosition;
        Vector3 direction = directionVector.normalized;
        float maxDistance = directionVector.magnitude;

        if (ownerCollider == null) return targetPosition;

        bool ownerColliderWasEnabled = ownerCollider.enabled;
        ownerCollider.enabled = false;

        RaycastHit2D[] hits = Physics2D.RaycastAll(
            currentPosition,
            direction,
            maxDistance,
            obstacleLayerMask
        );

        ownerCollider.enabled = ownerColliderWasEnabled;

        foreach (RaycastHit2D hit in hits)
        {
            if (!IsCameraBounds(hit.collider))
            {
                float safeDistance = 0.5f;
                float adjustedDistance = Mathf.Max(0, hit.distance - safeDistance);
                return currentPosition + (direction * adjustedDistance);
            }
        }

        return targetPosition;
    }

    private bool IsCameraBounds(Collider2D collider)
    {
        if (collider == null) return false;
        string colliderName = collider.name.ToLower();
        return colliderName.Contains("camerabounds") || colliderName.Contains("boundary");
    }
    
    private void DestroySelf()
    {
        Destroy(gameObject);
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        // Bỏ qua nếu va vào chính chủ nhân
        if (other.gameObject == owner)
            return;
            
        // Gây damage nếu đối tượng có HealthController
        HealthController target = other.GetComponent<HealthController>();
        if (target != null)
        {
            target.TakeDamage(damage);
        }
    }
}
