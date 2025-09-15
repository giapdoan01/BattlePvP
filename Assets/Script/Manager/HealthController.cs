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
    public float deathAnimationDelay = 2f;

    // ===== THÊM MỚI - Auto disable components khi chết =====
    [Header("Auto Disable On Death")]
    [Tooltip("Tự động disable các components khi IsDead = true")]
    public bool autoDisableOnDeath = true;
    
    [Tooltip("Các components sẽ bị disable (để trống sẽ tự động tìm tất cả MonoBehaviour trừ HealthController)")]
    public MonoBehaviour[] componentsToDisable;
    // ====================================================

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
    private GameObject targetObject;

    private MonoBehaviour[] allComponents;
    private bool[] originalStates;
    private void Awake()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            animator = GetComponentInParent<Animator>();
        }
        DetermineMainObject();

        if (targetObject != null)
        {
            originalPosition = targetObject.transform.position;
            originalRotation = targetObject.transform.rotation;
        }
        PrepareComponentsForDisabling();
    }

    private void PrepareComponentsForDisabling()
    {
        if (!autoDisableOnDeath) return;

        if (componentsToDisable == null || componentsToDisable.Length == 0)
        {
            var allMonoBehaviours = GetComponents<MonoBehaviour>();
            var validComponents = new System.Collections.Generic.List<MonoBehaviour>();
            
            foreach (var comp in allMonoBehaviours)
            {
                if (comp != this && comp != null) // Không disable chính mình
                {
                    validComponents.Add(comp);
                }
            }
            
            allComponents = validComponents.ToArray();
        }
        else
        {
            allComponents = componentsToDisable;
        }

        originalStates = new bool[allComponents.Length];
        for (int i = 0; i < allComponents.Length; i++)
        {
            if (allComponents[i] != null)
            {
                originalStates[i] = allComponents[i].enabled;
            }
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
        if (useAnimations && animator != null && HasAnimatorParameter(isDeadParameter))
        {
            animator.SetBool(isDeadParameter, true);
            DisableAllComponents();
        }

        OnDeath?.Invoke();

        if (canRespawn)
        {
            Invoke(nameof(HideObjectAfterDeath), deathAnimationDelay);
            Invoke(nameof(StartRespawnProcess), deathAnimationDelay + respawnTime);
        }
        else
        {
            Invoke(nameof(DestroyCharacter), deathAnimationDelay);
        }
    }


    private void DisableAllComponents()
    {
        if (!autoDisableOnDeath || allComponents == null) return;

        for (int i = 0; i < allComponents.Length; i++)
        {
            if (allComponents[i] != null && allComponents[i].enabled)
            {
                allComponents[i].enabled = false;
            }
        }
    }

    private void EnableAllComponents()
    {
        if (!autoDisableOnDeath || allComponents == null) return;

        for (int i = 0; i < allComponents.Length; i++)
        {
            if (allComponents[i] != null && originalStates[i])
            {
                allComponents[i].enabled = true;
            }
        }
    }

    private void HideObjectAfterDeath()
    {
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
        if (!canRespawn) return;

        if (targetObject != null)
        {
            targetObject.SetActive(true);
        }

        IsDead = false;
        IsRespawning = false;
        currentHealth = maxHealth;

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


        EnableAllComponents();


        if (useAnimations && animator != null)
        {
            if (HasAnimatorParameter(isDeadParameter))
                animator.SetBool(isDeadParameter, false);
            if (HasAnimatorParameter(takeDamageParameter))
                animator.SetBool(takeDamageParameter, false);
        }

        OnRespawn?.Invoke();
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

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

    private void OnDestroy()
    {
        CancelInvoke();
    }
}
