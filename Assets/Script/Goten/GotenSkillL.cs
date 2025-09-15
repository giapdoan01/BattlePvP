using UnityEngine;

public class GotenSkillL : MonoBehaviour, ISkill
{
    public AudioSource skillLSoundSource; // Nguồn âm thanh để phát âm thanh khi phóng chiêu
    public AudioClip skillLSound; // Âm thanh khi phóng chiêu
    public float damage = 20f;
    public float lifeTime = 0.3f; // thời gian hitbox tồn tại

    private GameObject owner;
    private SpriteRenderer spriteRenderer;

    // Khởi tạo từ CharacterSkill
    public void Initialize(GameObject skillOwner)
    {
        owner = skillOwner;
    }

    public void Initialize(GameObject skillOwner, bool ownerFacingRight)
    {
        owner = skillOwner;

        // Flip cả transform để collider cũng lật theo
        if (!ownerFacingRight)
        {
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
    }


    void Start()
    {
        Destroy(gameObject, lifeTime); // tự hủy sau khi hết thời gian hitbox
        if (skillLSoundSource != null && skillLSound != null)
        {
            skillLSoundSource.PlayOneShot(skillLSound);
        }
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
