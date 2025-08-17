// AnimatedHitEffect.cs
using UnityEngine;

public class AnimatedHitEffect : MonoBehaviour
{
    [Header("Animation Settings")]
    public Animator animator;
    public float animationDuration = 1f;

    void OnEnable()
    {
        // Nếu có animator thì play animation từ đầu
        if (animator != null)
        {
            animator.Play(0, 0, 0f); // Play layer 0, state 0, từ frame 0
            // Hoặc có thể dùng:
            // animator.Rebind(); // Reset về trạng thái ban đầu
            // animator.Update(0f); // Update ngay lập tức
        }

        // Tự deactivate sau animationDuration
        Invoke(nameof(DeactivateEffect), animationDuration);
    }

    void DeactivateEffect()
    {
        gameObject.SetActive(false);
    }

    void OnDisable()
    {
        CancelInvoke();
    }
}
