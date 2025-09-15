using UnityEngine;

public class TrunksSkillJEffect : MonoBehaviour
{
    [Header("Explosion Settings")]
    public AudioSource explosionSoundSource; // Nguồn âm thanh để phát âm thanh khi nổ
    public AudioClip explosionSound; // Âm thanh khi nổ
    public float damage = 30f;
    public float lifetime = 0.5f;
    
    private GameObject owner;

    void Start()
    {
        // Tự hủy sau khi hết thời gian
        Destroy(gameObject, lifetime);
        // Phát âm thanh nổ
        if (explosionSoundSource != null && explosionSound != null)
        {
            explosionSoundSource.PlayOneShot(explosionSound);
        }
    }
    
    public void SetOwner(GameObject explosionOwner)
    {
        owner = explosionOwner;
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        // Bỏ qua nếu va vào chính chủ nhân
        if (other.gameObject == owner)
            return;
            
        // Gây sát thương cho đối tượng có HealthController
        HealthController target = other.GetComponent<HealthController>();
        if (target != null)
        {
            target.TakeDamage(damage);
        }
    }
}
