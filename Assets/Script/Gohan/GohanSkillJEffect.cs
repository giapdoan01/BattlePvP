using UnityEngine;

public class AnimatedHitEffect : MonoBehaviour
{
    [Header("Animation Settings")]
    public Animator animator;
    public float animationDuration = 1f;

    void OnEnable()
    {
        if (animator != null)
        {
            animator.Play(0, 0, 0f);

        }
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
