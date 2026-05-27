using UnityEngine;

public class RescueNpc : MonoBehaviour, IDamageable
{
    [Header("Health")]
    [SerializeField] private int maxHealth = 50;

    [Header("Rescue")]
    [SerializeField] private float rescueDistance = 2.5f;
    [SerializeField] private KeyCode rescueKey = KeyCode.E;

    [Header("Reward")]
    [SerializeField] private int scoreReward = 250;
    [SerializeField] private int ammoReward = 10;

    [Header("Penalty")]
    [SerializeField] private int playerKillPenalty = 200;

    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private GameObject visualRoot;

    private int currentHealth;
    private bool isDead;
    private bool isRescued;

    public bool IsDead => isDead;
    public bool IsRescued => isRescued;
    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;

    private void Awake()
    {
        currentHealth = maxHealth;

        if (visualRoot == null)
        {
            visualRoot = gameObject;
        }
    }

    private void Start()
    {
        ResolvePlayer();
    }

    private void Update()
    {
        if (isDead || isRescued)
        {
            return;
        }

        TryRescue();
    }

    private void ResolvePlayer()
    {
        if (player != null)
        {
            return;
        }

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogWarning($"{name}: Player was not found. Make sure the player has the Player tag.");
        }
    }

    public void TakeDamage(int damage)
    {
        ApplyDamage(damage, false);
    }

    public void TakeDamageFromPlayer(int damage)
    {
        ApplyDamage(damage, true);
    }

    private void ApplyDamage(int damage, bool causedByPlayer)
    {
        if (isDead || isRescued)
        {
            return;
        }

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        GameFeedbackUI.Instance?.ShowMessage($"NPC took {damage} damage. HP: {currentHealth}/{maxHealth}");

        if (currentHealth <= 0)
        {
            Die(causedByPlayer);
        }
    }

    private void TryRescue()
    {
        if (player == null)
        {
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer > rescueDistance)
        {
            return;
        }

        GameFeedbackUI.Instance?.ShowMessage("Press E to rescue the NPC", 0.25f);

        if (Input.GetKeyDown(rescueKey))
        {
            Rescue();
        }
    }

    private void Rescue()
    {
        if (isDead || isRescued)
        {
            return;
        }

        isRescued = true;

        GiveReward();

        GameFeedbackUI.Instance?.ShowMessage("NPC rescued. Reward granted.");

        Destroy(gameObject);
    }

    private void Die(bool killedByPlayer)
    {
        if (isDead)
        {
            return;
        }

        isDead = true;

        if (killedByPlayer)
        {
            ApplyPlayerKillPenalty();
            GameFeedbackUI.Instance?.ShowMessage($"You killed the NPC. -{playerKillPenalty} Score.");
        }
        else
        {
            GameFeedbackUI.Instance?.ShowMessage("The NPC died.");
        }

        Destroy(gameObject, 1.5f);
    }

    private void GiveReward()
    {
        ScoreManager scoreManager = FindFirstObjectByType<ScoreManager>();

        if (scoreManager != null)
        {
            scoreManager.AddScore(scoreReward);
        }
    }

    private void ApplyPlayerKillPenalty()
    {
        ScoreManager scoreManager = FindFirstObjectByType<ScoreManager>();

        if (scoreManager != null)
        {
            scoreManager.RemoveScore(playerKillPenalty);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, rescueDistance);
    }
}