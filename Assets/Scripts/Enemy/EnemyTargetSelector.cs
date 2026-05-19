using UnityEngine;

[RequireComponent(typeof(EnemyBase))]
public class EnemyTargetSelector : MonoBehaviour
{
    [Header("Target Selection")]
    [SerializeField] private Transform playerTarget;
    [SerializeField] private float npcDetectionRange = 8f;
    [SerializeField] private float targetRefreshRate = 0.25f;

    [Header("Priority")]
    [SerializeField] private bool prioritizeNpcIfCloser = true;

    private EnemyBase enemy;
    private float refreshTimer;

    private void Awake()
    {
        enemy = GetComponent<EnemyBase>();
    }

    private void Start()
    {
        ResolvePlayer();

        if (playerTarget != null)
        {
            enemy.SetTarget(playerTarget);
        }
    }

    private void Update()
    {
        refreshTimer += Time.deltaTime;

        if (refreshTimer < targetRefreshRate)
        {
            return;
        }

        refreshTimer = 0f;

        UpdateTarget();
    }

    private void ResolvePlayer()
    {
        if (playerTarget != null)
        {
            return;
        }

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            playerTarget = playerObject.transform;
        }
        else
        {
            Debug.LogWarning($"{name}: Player target was not found. Make sure the player has the Player tag.");
        }
    }

    private void UpdateTarget()
    {
        if (enemy == null || enemy.IsDead)
        {
            return;
        }

        RescueNpc closestNpc = FindClosestValidNpc();

        if (closestNpc == null)
        {
            SetPlayerAsTarget();
            return;
        }

        if (!prioritizeNpcIfCloser)
        {
            enemy.SetTarget(closestNpc.transform);
            return;
        }

        if (playerTarget == null)
        {
            enemy.SetTarget(closestNpc.transform);
            return;
        }

        float distanceToNpc = Vector3.Distance(transform.position, closestNpc.transform.position);
        float distanceToPlayer = Vector3.Distance(transform.position, playerTarget.position);

        if (distanceToNpc <= distanceToPlayer)
        {
            enemy.SetTarget(closestNpc.transform);
        }
        else
        {
            SetPlayerAsTarget();
        }
    }

    private RescueNpc FindClosestValidNpc()
    {
        RescueNpc[] npcs = FindObjectsByType<RescueNpc>(FindObjectsSortMode.None);

        RescueNpc closestNpc = null;
        float closestDistance = Mathf.Infinity;

        foreach (RescueNpc npc in npcs)
        {
            if (npc == null || npc.IsDead || npc.IsRescued)
            {
                continue;
            }

            float distance = Vector3.Distance(transform.position, npc.transform.position);

            if (distance > npcDetectionRange)
            {
                continue;
            }

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestNpc = npc;
            }
        }

        return closestNpc;
    }

    private void SetPlayerAsTarget()
    {
        if (playerTarget == null)
        {
            return;
        }

        enemy.SetTarget(playerTarget);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, npcDetectionRange);
    }
}