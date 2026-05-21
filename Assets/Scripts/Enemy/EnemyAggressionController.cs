using UnityEngine;

[RequireComponent(typeof(EnemyBase))]
public class EnemyAggressionController : MonoBehaviour
{
    [Header("Player Reference")]
    [SerializeField] private HealthComponent playerHealth;

    [Header("Low Health Trigger")]
    [SerializeField, Range(0.05f, 0.9f)] private float lowHealthThreshold = 0.3f;

    [Header("Aggression Multipliers")]
    [SerializeField] private float speedMultiplier = 1.25f;
    [SerializeField] private float damageMultiplier = 1.25f;
    [SerializeField] private float cooldownMultiplier = 0.75f;
    [SerializeField] private float rangeMultiplier = 1.2f;

    [Header("Performance")]
    [SerializeField] private float checkRate = 0.25f;

    private EnemyBase enemyBase;
    private float checkTimer;
    private bool isAggressionActive;

    private void Awake()
    {
        enemyBase = GetComponent<EnemyBase>();
    }

    private void Start()
    {
        ResolvePlayerHealth();
    }

    private void Update()
    {
        if (enemyBase == null || enemyBase.IsDead)
        {
            return;
        }

        checkTimer += Time.deltaTime;

        if (checkTimer < checkRate)
        {
            return;
        }

        checkTimer = 0f;

        UpdateAggressionState();
    }

    private void ResolvePlayerHealth()
    {
        if (playerHealth != null)
        {
            return;
        }

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject == null)
        {
            Debug.LogWarning($"{name}: Player was not found. Make sure the player has the Player tag.");
            return;
        }

        playerHealth = playerObject.GetComponent<HealthComponent>();

        if (playerHealth == null)
        {
            playerHealth = playerObject.GetComponentInParent<HealthComponent>();
        }

        if (playerHealth == null)
        {
            playerHealth = playerObject.GetComponentInChildren<HealthComponent>();
        }

        if (playerHealth == null)
        {
            Debug.LogWarning($"{name}: Player HealthComponent was not found.");
        }
    }

    private void UpdateAggressionState()
    {
        if (playerHealth == null)
        {
            ResolvePlayerHealth();
            return;
        }

        if (playerHealth.IsDead)
        {
            SetAggression(false);
            return;
        }

        float healthPercent = playerHealth.MaxHealth > 0
            ? (float)playerHealth.CurrentHealth / playerHealth.MaxHealth
            : 0f;

        bool shouldBeAggressive = healthPercent <= lowHealthThreshold;

        SetAggression(shouldBeAggressive);
    }

    private void SetAggression(bool value)
    {
        if (isAggressionActive == value)
        {
            return;
        }

        isAggressionActive = value;

        enemyBase.SetAggressiveMode(
            isAggressionActive,
            speedMultiplier,
            damageMultiplier,
            cooldownMultiplier,
            rangeMultiplier
        );
    }
}