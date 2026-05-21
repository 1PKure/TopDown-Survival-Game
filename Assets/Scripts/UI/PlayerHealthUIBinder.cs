using UnityEngine;

[RequireComponent(typeof(HealthComponent))]
public class PlayerHealthUIBinder : MonoBehaviour
{
    private HealthComponent healthComponent;

    private void Awake()
    {
        healthComponent = GetComponent<HealthComponent>();
    }

    private void OnEnable()
    {
        if (healthComponent != null)
        {
            healthComponent.OnHealthChanged += UpdateHealthUI;
        }
    }

    private void Start()
    {
        if (healthComponent != null)
        {
            UpdateHealthUI(healthComponent.CurrentHealth, healthComponent.MaxHealth);
        }
    }

    private void OnDisable()
    {
        if (healthComponent != null)
        {
            healthComponent.OnHealthChanged -= UpdateHealthUI;
        }
    }

    private void UpdateHealthUI(int currentHealth, int maxHealth)
    {
        GameFeedbackUI.Instance?.SetHealth(currentHealth, maxHealth);
    }
}