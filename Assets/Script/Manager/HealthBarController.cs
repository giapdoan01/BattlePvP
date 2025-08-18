using UnityEngine;

public class HealthBarController : MonoBehaviour
{
    public Transform fillTransform;
    public Transform backgroundTransform;
    private Vector3 originalScale;

    private IHealthObserver targetHealth;

    void Start()
    {
        if (fillTransform != null)
            originalScale = fillTransform.localScale;

        if (backgroundTransform != null && backgroundTransform.localPosition != Vector3.zero)
        {
            backgroundTransform.localPosition = Vector3.zero;
        }
        else
        {
            Debug.LogError("backgroundTransform not attached in Inspector!");
        }

        if (fillTransform != null)
        {
            fillTransform.localPosition = new Vector3(-1.095f, 0f, 0f);
        }
        else
        {
            Debug.LogError("fillTransform not attached in Inspector!");
        }
    }

    void LateUpdate()
    {
        transform.rotation = Camera.main.transform.rotation;
    }

    // Bind Observer vào Publisher
    public void Bind(IHealthObserver health)
    {
        targetHealth = health;
        targetHealth.OnHealthChanged += SetHealth;


    }

    // Hủy đăng ký khi bị Destroy
    private void OnDestroy()
    {
        if (targetHealth != null)
        {
            targetHealth.OnHealthChanged -= SetHealth;
        }
    }

    // Nhận event và update UI
    private void SetHealth(float current, float max)
    {
        float ratio = Mathf.Clamp01(current / max);
        fillTransform.localScale = new Vector3(ratio * originalScale.x, originalScale.y, originalScale.z);
    }
}
