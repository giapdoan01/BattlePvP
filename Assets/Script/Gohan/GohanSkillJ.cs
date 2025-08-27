using UnityEngine;
using System.Collections;

public class GohanSkillJ : MonoBehaviour, ISkill
{
    [Header("Skill Settings")]
    public float maxSpeed = 20f;
    public float accelerationTime = 5f;
    public float damage = 30f;
    public float lifeTime = 8f;

    [Header("Hit Effect Settings")]
    public GameObject hitEffectPrefab; // Prefab hiệu ứng nổ
    public bool destroyOnHit = false; // Có destroy skill sau khi hit không
    public float hitEffectDuration = 2f; // Thời gian hiệu ứng nổ tồn tại

    [Header("Hit Effect Pool Settings")]
    private static string hitEffectPoolTag = "SkillJHitEffect";
    private static int hitEffectPoolSize = 10;
    private static bool hitEffectPoolInitialized = false;

    private static bool poolInitialized = false;
    private static string poolTag = "SkillJ";
    private static int poolSize = 5;

    private GameObject owner;
    private bool facingRight = true;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private float currentSpeed = 20f;
    private float acceleration;
    private bool isActive = false;

    void Awake()
    {
        // Cache components
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Initialize(GameObject skillOwner, bool ownerFacingRight)
    {
        owner = skillOwner;
        facingRight = ownerFacingRight;

        // Tạo ObjectPool nếu chưa có
        InitializePool();
        InitializeHitEffectPool();

        // Lấy object từ pool
        GetFromPool();
    }

    void InitializePool()
    {
        // Tạo ObjectPool singleton nếu chưa có
        if (ObjectPool.Instance == null)
        {
            GameObject poolObj = new GameObject("ObjectPool");
            poolObj.AddComponent<ObjectPool>();
        }

        // Tạo pool cho SkillJ nếu chưa có
        if (!poolInitialized)
        {
            ObjectPool.Instance.CreatePool(poolTag, gameObject, poolSize);
            poolInitialized = true;
        }
    }

    void InitializeHitEffectPool()
    {
        // Tạo pool cho hit effect nếu có prefab và chưa khởi tạo
        if (hitEffectPrefab != null && !hitEffectPoolInitialized)
        {
            ObjectPool.Instance.CreatePool(hitEffectPoolTag, hitEffectPrefab, hitEffectPoolSize);
            hitEffectPoolInitialized = true;
        }
    }

    void GetFromPool()
    {
        // Lấy object từ pool
        GameObject pooledObj = ObjectPool.Instance.Get(poolTag, transform.position, transform.rotation);

        if (pooledObj != null && pooledObj != gameObject)
        {
            GohanSkillJ pooledSkill = pooledObj.GetComponent<GohanSkillJ>();
            if (pooledSkill != null)
            {
                pooledSkill.ActivateSkill(owner, facingRight);
            }
        }
        else
        {
            // Nếu lấy chính object này từ pool thì activate luôn
            ActivateSkill(owner, facingRight);
        }

        // Destroy object gốc nếu không phải từ pool
        if (pooledObj != gameObject)
        {
            Destroy(gameObject, 0.1f);
        }
    }

    public void ActivateSkill(GameObject skillOwner, bool ownerFacingRight)
    {
        owner = skillOwner;
        facingRight = ownerFacingRight;

        // Setup components nếu chưa có
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        // Flip sprite theo hướng
        FlipSprite();

        // Tính gia tốc
        acceleration = maxSpeed / accelerationTime;

        // Reset trạng thái
        currentSpeed = 0f;
        isActive = true;

        // Bắt đầu di chuyển với gia tốc
        StartCoroutine(AccelerateMovement());

        // Tự deactivate sau lifeTime
        StartCoroutine(DeactivateAfterTime());
    }

    void FlipSprite()
    {
        if (spriteRenderer != null)
        {
            // Flip sprite theo hướng bay
            spriteRenderer.flipX = !facingRight;
        }
    }

    IEnumerator AccelerateMovement()
    {
        float elapsedTime = 0f;

        // Giai đoạn tăng tốc
        while (elapsedTime < accelerationTime && isActive)
        {
            elapsedTime += Time.deltaTime;
            currentSpeed = acceleration * elapsedTime;
            currentSpeed = Mathf.Min(currentSpeed, maxSpeed);

            Vector2 direction = facingRight ? Vector2.right : Vector2.left;
            rb.linearVelocity = direction * currentSpeed;

            yield return null;
        }

        // Duy trì tốc độ tối đa
        while (isActive)
        {
            Vector2 direction = facingRight ? Vector2.right : Vector2.left;
            rb.linearVelocity = direction * maxSpeed;
            yield return null;
        }
    }

    IEnumerator DeactivateAfterTime()
    {
        yield return new WaitForSeconds(lifeTime);
        DeactivateSkill();
    }

    void DeactivateSkill()
    {
        isActive = false;
        rb.linearVelocity = Vector2.zero;
        gameObject.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Bỏ qua nếu va vào chính chủ nhân
        if (other.gameObject == owner)
            return;

        // Gây damage nếu có HealthController
        HealthController target = other.GetComponent<HealthController>();
        if (target != null)
        {
            // Tính vị trí va chạm
            Vector3 hitPosition = GetHitPosition(other);

            // Gây damage
            target.TakeDamage(damage);

            // Tạo hiệu ứng nổ tại vị trí va chạm
            SpawnHitEffect(hitPosition);

            // Debug log
            Debug.Log($"GohanSkillJ hit {other.name} at position {hitPosition}");

        }
    }

    Vector3 GetHitPosition(Collider2D hitCollider)
    {
        // Lấy vị trí va chạm chính xác
        Vector3 skillPosition = transform.position;
        Vector3 targetPosition = hitCollider.transform.position;

        // Tính vị trí va chạm (giữa skill và target)
        Vector3 hitPosition = Vector3.Lerp(skillPosition, targetPosition, 0.5f);

        // Hoặc có thể dùng Collider bounds để tính chính xác hơn
        Bounds skillBounds = GetComponent<Collider2D>().bounds;
        Bounds targetBounds = hitCollider.bounds;

        // Tìm điểm gần nhất giữa 2 collider
        Vector3 closestPointOnSkill = skillBounds.ClosestPoint(targetBounds.center);
        Vector3 closestPointOnTarget = targetBounds.ClosestPoint(skillBounds.center);

        hitPosition = Vector3.Lerp(closestPointOnSkill, closestPointOnTarget, 0.5f);

        return hitPosition;
    }

    void SpawnHitEffect(Vector3 position)
    {
        if (hitEffectPrefab == null) return;

        GameObject effect = null;

        // Thử lấy từ pool trước
        if (ObjectPool.Instance != null && hitEffectPoolInitialized)
        {
            effect = ObjectPool.Instance.Get(hitEffectPoolTag, position, Quaternion.identity);
        }

        // Nếu không có pool thì Instantiate
        if (effect == null)
        {
            effect = Instantiate(hitEffectPrefab, position, Quaternion.identity);
        }

        if (effect != null)
        {
            // Đảm bảo effect được kích hoạt
            effect.SetActive(true);

            // Tự động return về pool hoặc destroy sau một thời gian
            StartCoroutine(ReturnHitEffectToPool(effect));

            Debug.Log($"Hit effect spawned at {position}");
        }
    }

    IEnumerator ReturnHitEffectToPool(GameObject effect)
    {
        yield return new WaitForSeconds(hitEffectDuration);

        if (effect != null)
        {
            // Nếu có pool thì return về pool
            if (ObjectPool.Instance != null && hitEffectPoolInitialized)
            {
                effect.SetActive(false);
                // Pool sẽ tự động nhận object khi SetActive(false)
            }
            else
            {
                // Không có pool thì destroy
                Destroy(effect);
            }
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
