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
        if (isDead || isRescued)
        {
            return;
        }

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        GameFeedbackUI.Instance?.ShowMessage($"NPC took {damage} damage. HP: {currentHealth}/{maxHealth}");

        if (currentHealth <= 0)
        {
            Die();
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

    private void Die()
    {
        if (isDead)
        {
            return;
        }

        isDead = true;

        GameFeedbackUI.Instance?.ShowMessage("The NPC died.");

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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, rescueDistance);
    }
}