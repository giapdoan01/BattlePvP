using UnityEngine;

public class TrunksSkillJ : MonoBehaviour, ISkill
{
    [Header("Projectile Settings")]
    public AudioSource landingSoundSource; // Nguồn âm thanh để phát âm thanh khi phóng chiêu
    public float damage = 20f;
    public float lifeTime = 0.3f; // thời gian hitbox tồn tại
    public float moveSpeed = 33.3f; // Tốc độ di chuyển (units/giây) để đạt 10 units trong 0.3 giây
    public AudioClip launchSound; // Âm thanh khi phóng chiêu
    [Header("Explosion Settings")]
    public GameObject explosionPrefab; // Prefab của hiệu ứng nổ
    private GameObject owner;
    private bool movingRight; // Hướng di chuyển
    private float distanceTraveled = 0f; // Khoảng cách đã di chuyển
    private float maxDistance = 10f; // Khoảng cách tối đa có thể di chuyển
    private bool hasExploded = false; // Đã nổ chưa
    private bool hasPlayedLaunchSound = false;

    // Khởi tạo từ CharacterSkill
    public void Initialize(GameObject skillOwner)
    {
        owner = skillOwner;
        movingRight = true;
    }

    public void Initialize(GameObject skillOwner, bool ownerFacingRight)
    {
        owner = skillOwner;
        movingRight = ownerFacingRight;

        // Flip cả transform để collider cũng lật theo
        if (!ownerFacingRight)
        {
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
    }
    void Start()
    {
        Destroy(gameObject, lifeTime); // tự hủy sau khi hết thời gian hitbox
        if (landingSoundSource != null && launchSound != null && !hasPlayedLaunchSound)
        {
            landingSoundSource.PlayOneShot(launchSound);
            hasPlayedLaunchSound = true;
        }
    }

    void Update()
    {
        if (hasExploded)
            return;

        // Di chuyển skill theo hướng
        float direction = movingRight ? 1 : -1;
        float distanceThisFrame = moveSpeed * Time.deltaTime;
        transform.Translate(Vector3.right * direction * distanceThisFrame);

        // Cập nhật khoảng cách đã di chuyển
        distanceTraveled += distanceThisFrame;

        // Nếu đã di chuyển đủ khoảng cách, tạo vụ nổ
        if (distanceTraveled >= maxDistance)
        {
            CreateExplosion();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Bỏ qua nếu va vào chính chủ nhân
        if (other.gameObject == owner || hasExploded)
            return;

        // Nếu có máu thì gây damage
        HealthController target = other.GetComponent<HealthController>();
        if (target != null)
        {
            target.TakeDamage(damage);

            // Tạo vụ nổ khi va chạm với kẻ địch
            CreateExplosion();
        }
    }

    private void CreateExplosion()
    {
        if (hasExploded || explosionPrefab == null)
            return;

        hasExploded = true;

        // Tạo hiệu ứng nổ
        GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        // Truyền thông tin về owner cho ExplosionEffect
        TrunksSkillJEffect explosionEffect = explosion.GetComponent<TrunksSkillJEffect>();
        if (explosionEffect != null)
        {
            explosionEffect.SetOwner(owner);
        }

        // Ẩn projectile
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.enabled = false;
        }

        // Vô hiệu hóa collider
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.enabled = false;
        }

        // Hủy GameObject sau khi nổ
        Destroy(gameObject, 0.1f);
    }
}
