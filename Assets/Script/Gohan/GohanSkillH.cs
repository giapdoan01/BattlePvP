using UnityEngine;

public class GohanSkillH : MonoBehaviour, ISkill
{
    public AudioSource SkillHAudioSource;
    public AudioClip SkillHAudioClip;
    public float damage = 20f;
    public float lifeTime = 0.3f; // thời gian hitbox tồn tại

    private GameObject owner;

    // Khởi tạo từ CharacterSkill
    public void Initialize(GameObject skillOwner)
    {
        owner = skillOwner;
    }

    public void Initialize(GameObject skillOwner, bool ownerFacingRight)
    {
        owner = skillOwner;
    }

    void Start()
    {
        Destroy(gameObject, lifeTime); // tự hủy sau khi hết thời gian hitbox
        if (SkillHAudioSource != null && SkillHAudioClip != null)
        {
            SkillHAudioSource.PlayOneShot(SkillHAudioClip);
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
