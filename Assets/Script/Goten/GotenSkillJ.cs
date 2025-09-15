using UnityEngine;

public class GotenSkillJ : MonoBehaviour, ISkill
{
    public AudioSource skillJSoundSource; // Nguồn âm thanh để phát âm thanh khi phóng chiêu
    public AudioClip skillJSound; // Âm thanh khi phóng chiêu
    public float damage = 20f;
    public float lifeTime = 0.3f; // thời gian hitbox tồn tại
    public float moveSpeed = 33.3f; // Tốc độ di chuyển (units/giây) để đạt 10 units trong 0.3 giây

    private GameObject owner;
    private bool movingRight; // Hướng di chuyển

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
        if (skillJSoundSource != null && skillJSound != null)
        {
            skillJSoundSource.PlayOneShot(skillJSound);
        }
    }

    void Update()
    {
        // Di chuyển skill theo hướng
        float direction = movingRight ? 1 : -1;
        transform.Translate(Vector3.right * direction * moveSpeed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Bỏ qua nếu va vào chính chủ nhân
        if (other.gameObject == owner)
            return;

        // Nếu có máu thì gây damage
        HealthController target = other.GetComponent<HealthController>();
        if (target != null)
        {
            target.TakeDamage(damage);
        }
    }
}
