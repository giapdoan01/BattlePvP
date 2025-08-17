using System;
using System.Collections;
using UnityEngine;

public class HealthController : MonoBehaviour, TakeDamageInterface, IHealthObserver
{
    public float maxHealth = 100f;
    private float currentHealth;
    private Animator animator;

    public GameObject healthBarPrefab;
    public float takeDamageDuration = 0.5f; 

    public event Action<float, float> OnHealthChanged;

    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;

    private GameObject rootEnemy;

    private void Awake()
    {

        animator = GetComponent<Animator>();
        if (animator == null)
        {
            animator = GetComponentInParent<Animator>();
        }
    }

    void Start()
    {
        currentHealth = maxHealth;

        GameObject barObj = Instantiate(healthBarPrefab, transform);
        barObj.transform.localPosition = new Vector3(0, 2f, 0);

        var healthBar = barObj.GetComponent<HealthBarController>();
        if (healthBar != null)
        {
            healthBar.Bind(this);
        }
    }

    public void TakeDamage(float amount)
    {
        currentHealth = Mathf.Max(0, currentHealth - amount);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (animator != null)
        {
            animator.SetBool("IsTakeDamage", true);
            StartCoroutine(ResetTakeDamageAnimation());
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private IEnumerator ResetTakeDamageAnimation()
    {
        yield return new WaitForSeconds(takeDamageDuration);
        if (animator != null)
        {
            animator.SetBool("IsTakeDamage", false);
        }
    }

    public void Heal(float amount)
    {
        if (currentHealth < maxHealth)
        {
            currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
        }
    }

    void Die()
    {
        if (animator != null)
        {
            animator.SetBool("IsDead", true);
        }
        Invoke(nameof(DestroyCharacter), 1f);
    }

    public void DestroyCharacter()
    {
        if (transform.parent != null)
        {
            Destroy(transform.parent.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        
    }
}
