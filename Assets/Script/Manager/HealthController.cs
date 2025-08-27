using System;
using System.Collections;
using UnityEngine;

public class HealthController : MonoBehaviour, TakeDamageInterface, IHealthObserver
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    private float currentHealth;
    private Animator animator;

    [Header("UI")]
    public GameObject healthBarPrefab;
    public float takeDamageDuration = 0.5f;

    [Header("Respawn Settings")]
    public bool canRespawn = false;
    public float respawnTime = 3f;
    public Transform respawnPoint;

    [Header("Death/Respawn Control")]
    [Tooltip("Object chính để ẩn/hiện khi chết/hồi sinh (thường là parent root)")]
    public GameObject mainObject;
    [Tooltip("Tự động tìm root parent nếu mainObject không được set")]
    public bool autoFindRootParent = true;

    [Header("Animation Settings")]
    public bool useAnimations = true;
    public string takeDamageParameter = "IsTakeDamage";
    public string isDeadParameter = "IsDead";
    [Tooltip("Thời gian chờ animation death chạy trước khi ẩn object")]
    public float deathAnimationDelay = 1f;

    // Events
    public event Action<float, float> OnHealthChanged;
    public event Action OnDeath;
    public event Action OnRespawn;

    // Properties
    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;
    public bool IsDead { get; private set; }
    public bool IsRespawning { get; private set; }

    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private GameObject targetObject; // Object thực sự sẽ bị ẩn/hiện

    private void Awake()
    {
        // Tìm animator
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            animator = GetComponentInParent<Animator>();
        }

        // Xác định object chính để control
        DetermineMainObject();

        // Lưu vị trí ban đầu của object chính
        if (targetObject != null)
        {
            originalPosition = targetObject.transform.position;
            originalRotation = targetObject.transform.rotation;
        }
    }

    private void DetermineMainObject()
    {
        if (mainObject != null)
        {
            targetObject = mainObject;
        }
        else if (autoFindRootParent)
        {
            // Tìm root parent
            Transform current = transform;
            while (current.parent != null)
            {
                current = current.parent;
            }
            targetObject = current.gameObject;
        }
        else
        {
            targetObject = gameObject;
        }
    }

    void Start()
    {
        currentHealth = maxHealth;
        IsDead = false;
        IsRespawning = false;

        // Tạo health bar
        if (healthBarPrefab != null)
        {
            GameObject barObj = Instantiate(healthBarPrefab, transform);
            barObj.transform.localPosition = new Vector3(0, 2f, 0);

            var healthBar = barObj.GetComponent<HealthBarController>();
            if (healthBar != null)
            {
                healthBar.Bind(this);
            }
        }
    }

    public void TakeDamage(float amount)
    {
        if (IsDead || IsRespawning) return;

        currentHealth = Mathf.Max(0, currentHealth - amount);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        // Animation take damage
        if (useAnimations && animator != null && HasAnimatorParameter(takeDamageParameter))
        {
            animator.SetBool(takeDamageParameter, true);
            StartCoroutine(ResetTakeDamageAnimation());
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private bool HasAnimatorParameter(string paramName)
    {
        if (animator == null) return false;

        for (int i = 0; i < animator.parameterCount; i++)
        {
            if (animator.parameters[i].name == paramName)
                return true;
        }
        return false;
    }

    private IEnumerator ResetTakeDamageAnimation()
    {
        yield return new WaitForSeconds(takeDamageDuration);
        if (useAnimations && animator != null && HasAnimatorParameter(takeDamageParameter))
        {
            animator.SetBool(takeDamageParameter, false);
        }
    }

    public void Heal(float amount)
    {
        if (IsDead || IsRespawning) return;

        if (currentHealth < maxHealth)
        {
            currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
        }
    }

    void Die()
    {
        if (IsDead) return;

        IsDead = true;

        // Animation death
        if (useAnimations && animator != null && HasAnimatorParameter(isDeadParameter))
        {
            animator.SetBool(isDeadParameter, true);
        }

        OnDeath?.Invoke();

        if (canRespawn)
        {
            // Delay ẩn object để animation có thời gian chạy
            Invoke(nameof(HideObjectAfterDeath), deathAnimationDelay);

            // Schedule respawn sau thời gian death animation + respawn time
            Invoke(nameof(StartRespawnProcess), deathAnimationDelay + respawnTime);
        }
        else
        {
            // Delay destroy để animation có thời gian chạy
            Invoke(nameof(DestroyCharacter), deathAnimationDelay + 0.5f);
        }
    }

    private void HideObjectAfterDeath()
    {
        // Ẩn object sau khi animation death đã chạy
        if (targetObject != null && IsDead)
        {
            targetObject.SetActive(false);
        }
    }

    private void StartRespawnProcess()
    {
        Respawn();
    }

    public void Respawn()
    {
        if (!canRespawn)
        {
            return;
        }

        // Hiện lại object chính TRƯỚC KHI reset trạng thái
        if (targetObject != null)
        {
            targetObject.SetActive(true);
        }

        // Reset trạng thái
        IsDead = false;
        IsRespawning = false;
        currentHealth = maxHealth;

        // Reset vị trí của object chính
        if (targetObject != null)
        {
            if (respawnPoint != null)
            {
                targetObject.transform.position = respawnPoint.position;
                targetObject.transform.rotation = respawnPoint.rotation;
            }
            else
            {
                targetObject.transform.position = originalPosition;
                targetObject.transform.rotation = originalRotation;
            }
        }

        // Reset animator
        if (useAnimations && animator != null)
        {
            if (HasAnimatorParameter(isDeadParameter))
                animator.SetBool(isDeadParameter, false);
            if (HasAnimatorParameter(takeDamageParameter))
                animator.SetBool(takeDamageParameter, false);
        }

        // Thông báo sự kiện hồi sinh
        OnRespawn?.Invoke();
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    // Utility functions
    public void SetMainObject(GameObject obj)
    {
        mainObject = obj;
        targetObject = obj;

        if (targetObject != null)
        {
            originalPosition = targetObject.transform.position;
            originalRotation = targetObject.transform.rotation;
        }
    }

    public void SetRespawnEnabled(bool enabled)
    {
        canRespawn = enabled;
    }

    public void SetRespawnTime(float time)
    {
        respawnTime = Mathf.Max(0f, time);
    }

    public void SetRespawnPoint(Transform point)
    {
        respawnPoint = point;
    }

    public void SetDeathAnimationDelay(float delay)
    {
        deathAnimationDelay = Mathf.Max(0f, delay);
    }

    public void ForceRespawn()
    {
        if (IsDead)
        {
            // Cancel tất cả invoke liên quan đến death/respawn
            CancelInvoke(nameof(StartRespawnProcess));
            CancelInvoke(nameof(HideObjectAfterDeath));
            CancelInvoke(nameof(DestroyCharacter));

            Respawn();
        }
        else
        {
            Debug.LogWarning($"{gameObject.name} không thể force respawn vì chưa chết");
        }
    }

    public void DestroyCharacter()
    {

        if (targetObject != null)
        {
            Destroy(targetObject);
        }
        else if (transform.parent != null)
        {
            Destroy(transform.parent.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void KillPermanently()
    {

        bool originalCanRespawn = canRespawn;
        canRespawn = false;

        // Cancel tất cả respawn
        CancelInvoke(nameof(StartRespawnProcess));
        CancelInvoke(nameof(HideObjectAfterDeath));

        if (!IsDead)
        {
            Die();
        }
        else
        {
            // Nếu đã chết thì destroy luôn
            CancelInvoke(nameof(DestroyCharacter));
            Invoke(nameof(DestroyCharacter), 0.1f);
        }

        canRespawn = originalCanRespawn;
    }

    // Context menu functions
    [ContextMenu("Auto Find Root Parent")]
    public void AutoFindRootParent()
    {
        autoFindRootParent = true;
        DetermineMainObject();
    }

    [ContextMenu("Enable Respawn")]
    public void EnableRespawn()
    {
        SetRespawnEnabled(true);
    }

    [ContextMenu("Test Kill and Respawn")]
    public void TestKillAndRespawn()
    {
        SetRespawnEnabled(true);
        SetRespawnTime(2f);
        TakeDamage(maxHealth);
    }

    [ContextMenu("Test Death Animation")]
    public void TestDeathAnimation()
    {
        SetRespawnEnabled(false);
        SetDeathAnimationDelay(2f); // 2 giây để xem animation
        TakeDamage(maxHealth);
    }

    //[ContextMenu("Show Debug Info")]
    //public void ShowDebugInfo()
    //{
    //    Debug.Log($"=== Debug Info cho {gameObject.name} ===");
    //    Debug.Log($"Target Object: {(targetObject != null ? targetObject.name : "null")}");
    //    Debug.Log($"IsDead: {IsDead}");
    //    Debug.Log($"IsRespawning: {IsRespawning}");
    //    Debug.Log($"CanRespawn: {canRespawn}");
    //    Debug.Log($"Current HP: {currentHealth}/{maxHealth}");
    //    Debug.Log($"Death Animation Delay: {deathAnimationDelay}");
    //    Debug.Log($"Respawn Time: {respawnTime}");
    //}

    private void OnDestroy()
    {
        // Cancel tất cả invoke khi object bị destroy
        CancelInvoke();
    }
}
