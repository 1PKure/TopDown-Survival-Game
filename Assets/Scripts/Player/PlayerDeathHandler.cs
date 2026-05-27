using UnityEngine;

[RequireComponent(typeof(HealthComponent))]
public class PlayerDeathHandler : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;

    private HealthComponent healthComponent;

    private void Awake()
    {
        healthComponent = GetComponent<HealthComponent>();

        if (gameManager == null)
        {
            gameManager = FindFirstObjectByType<GameManager>();
        }
    }

    private void OnEnable()
    {
        if (healthComponent != null)
        {
            healthComponent.OnDeath += HandlePlayerDeath;
        }
    }

    private void OnDisable()
    {
        if (healthComponent != null)
        {
            healthComponent.OnDeath -= HandlePlayerDeath;
        }
    }

    private void HandlePlayerDeath()
    {
        if (gameManager != null)
        {
            gameManager.GameOver();
        }
        else
        {
            Debug.LogWarning($"{name}: GameManager reference is missing.");
        }
    }
}