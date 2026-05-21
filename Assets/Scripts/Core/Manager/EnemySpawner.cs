using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform[] patrolPoints;

    [Header("Enemy Prefabs")]
    [SerializeField] private GameObject meleeEnemyPrefab;
    [SerializeField] private GameObject rangedEnemyPrefab;

    [Header("Spawn Points")]
    [SerializeField] private Transform[] spawnPoints;

    [Header("Spawn Settings")]
    [SerializeField] private float initialSpawnInterval = 4f;
    [SerializeField] private float minimumSpawnInterval = 1.2f;
    [SerializeField] private float spawnIntervalDecreaseRate = 0.15f;
    [SerializeField] private float difficultyIncreaseInterval = 20f;
    [SerializeField] private int maxAliveEnemies = 12;
    [SerializeField] private float minimumDistanceFromPlayer = 6f;

    [Header("Enemy Ratio")]
    [Range(0f, 1f)]
    [SerializeField] private float rangedSpawnChance = 0.3f;

    private float currentSpawnInterval;
    private int aliveEnemies;
    private Coroutine spawnRoutine;
    private Coroutine difficultyRoutine;
    private bool isSpawning;

    private void Start()
    {
        currentSpawnInterval = initialSpawnInterval;
        StartSpawning();
    }

    public void StartSpawning()
    {
        if (isSpawning)
        {
            return;
        }

        isSpawning = true;

        spawnRoutine = StartCoroutine(SpawnLoop());
        difficultyRoutine = StartCoroutine(DifficultyLoop());
    }

    public void StopSpawning()
    {
        isSpawning = false;

        if (spawnRoutine != null)
        {
            StopCoroutine(spawnRoutine);
        }

        if (difficultyRoutine != null)
        {
            StopCoroutine(difficultyRoutine);
        }
    }

    private IEnumerator SpawnLoop()
    {
        while (isSpawning)
        {
            yield return new WaitForSeconds(currentSpawnInterval);

            if (CanSpawn())
            {
                SpawnEnemy();
                GameFeedbackUI.Instance?.AddEnemy();
            }
        }
    }

    private IEnumerator DifficultyLoop()
    {
        while (isSpawning)
        {
            yield return new WaitForSeconds(difficultyIncreaseInterval);

            currentSpawnInterval -= spawnIntervalDecreaseRate;
            currentSpawnInterval = Mathf.Max(currentSpawnInterval, minimumSpawnInterval);

            Debug.Log($"Spawn interval reduced to: {currentSpawnInterval}");
        }
    }

    private bool CanSpawn()
    {
        if (aliveEnemies >= maxAliveEnemies)
        {
            return false;
        }

        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogWarning($"{name}: No spawn points assigned.");
            return false;
        }

        if (meleeEnemyPrefab == null && rangedEnemyPrefab == null)
        {
            Debug.LogWarning($"{name}: No enemy prefabs assigned.");
            return false;
        }

        return true;
    }

    private void SpawnEnemy()
    {
        Transform spawnPoint = GetValidSpawnPoint();

        if (spawnPoint == null)
        {
            return;
        }

        GameObject prefab = ChooseEnemyPrefab();

        if (prefab == null)
        {
            return;
        }

        Vector3 spawnPosition = GetValidNavMeshPosition(spawnPoint.position);

        GameObject enemyInstance = Instantiate(
            prefab,
            spawnPosition,
            spawnPoint.rotation
        );

        AssignPlayerTarget(enemyInstance);
        SubscribeToEnemyDeath(enemyInstance);

        aliveEnemies++;
    }

    private Transform GetValidSpawnPoint()
    {
        const int maxAttempts = 20;

        for (int i = 0; i < maxAttempts; i++)
        {
            Transform candidate = spawnPoints[Random.Range(0, spawnPoints.Length)];

            if (candidate == null)
            {
                continue;
            }

            if (player == null)
            {
                return candidate;
            }

            float distanceToPlayer = Vector3.Distance(candidate.position, player.position);

            if (distanceToPlayer >= minimumDistanceFromPlayer)
            {
                return candidate;
            }
        }

        return spawnPoints[Random.Range(0, spawnPoints.Length)];
    }

    private GameObject ChooseEnemyPrefab()
    {
        bool shouldSpawnRanged = Random.value <= rangedSpawnChance;

        if (shouldSpawnRanged && rangedEnemyPrefab != null)
        {
            return rangedEnemyPrefab;
        }

        if (meleeEnemyPrefab != null)
        {
            return meleeEnemyPrefab;
        }

        return rangedEnemyPrefab;
    }

    private Vector3 GetValidNavMeshPosition(Vector3 desiredPosition)
    {
        if (NavMesh.SamplePosition(desiredPosition, out NavMeshHit hit, 3f, NavMesh.AllAreas))
        {
            return hit.position;
        }

        return desiredPosition;
    }

    private void AssignPlayerTarget(GameObject enemyInstance)
    {
        EnemyBase enemy = enemyInstance.GetComponent<EnemyBase>();

        if (enemy == null)
        {
            Debug.LogWarning($"{enemyInstance.name}: EnemyBase component not found.");
            return;
        }

        enemy.SetTarget(player);
        enemy.SetPatrolPoints(patrolPoints);
    }

    private void SubscribeToEnemyDeath(GameObject enemyInstance)
    {
        HealthComponent healthComponent = enemyInstance.GetComponent<HealthComponent>();

        if (healthComponent == null)
        {
            Debug.LogWarning($"{enemyInstance.name}: HealthComponent not found.");
            return;
        }

        healthComponent.OnDeath += HandleEnemyDeath;
    }

    private void HandleEnemyDeath()
    {
        aliveEnemies--;
        aliveEnemies = Mathf.Max(0, aliveEnemies);
    }

    private void OnDrawGizmosSelected()
    {
        if (spawnPoints == null)
        {
            return;
        }

        Gizmos.color = Color.green;

        foreach (Transform spawnPoint in spawnPoints)
        {
            if (spawnPoint == null)
            {
                continue;
            }

            Gizmos.DrawWireSphere(spawnPoint.position, 0.5f);
        }
    }
}