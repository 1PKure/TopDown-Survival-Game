using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform[] patrolPoints;

    [Header("Normal Enemy Prefabs")]
    [SerializeField] private GameObject meleeEnemyPrefab;
    [SerializeField] private GameObject rangedEnemyPrefab;

    [Header("Fast Enemy Prefabs")]
    [SerializeField] private GameObject fastMeleeEnemyPrefab;
    [SerializeField] private GameObject fastRangedEnemyPrefab;

    [Header("Normal Spawn Points")]
    [SerializeField] private Transform[] spawnPoints;

    [Header("Fast Only Spawn Points")]
    [SerializeField] private Transform[] fastOnlySpawnPoints;

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

    [Range(0f, 1f)]
    [SerializeField] private float fastOnlySpawnChance = 0.25f;

    [Range(0f, 1f)]
    [SerializeField] private float fastRangedSpawnChance = 0.2f;

    [Header("Initial Fast Spawn")]
    [SerializeField] private bool spawnFastEnemyOnStart = true;
    [SerializeField] private float initialFastSpawnDelay = 1f;

    private float currentSpawnInterval;
    private int aliveEnemies;
    private Coroutine spawnRoutine;
    private Coroutine difficultyRoutine;
    private Coroutine initialFastSpawnRoutine;
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

        if (spawnFastEnemyOnStart)
        {
            initialFastSpawnRoutine = StartCoroutine(InitialFastSpawnRoutine());
        }
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

        if (initialFastSpawnRoutine != null)
        {
            StopCoroutine(initialFastSpawnRoutine);
        }
    }

    private IEnumerator InitialFastSpawnRoutine()
    {
        yield return new WaitForSeconds(initialFastSpawnDelay);

        if (!isSpawning)
        {
            yield break;
        }

        if (CanSpawnFastEnemy())
        {
            SpawnFastEnemy();
        }
    }

    private IEnumerator SpawnLoop()
    {
        while (isSpawning)
        {
            yield return new WaitForSeconds(currentSpawnInterval);

            if (!CanSpawn())
            {
                continue;
            }

            bool shouldSpawnFastEnemy = Random.value <= fastOnlySpawnChance;

            if (shouldSpawnFastEnemy && CanSpawnFastEnemy())
            {
                SpawnFastEnemy();
            }
            else
            {
                SpawnNormalEnemy();
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

        if (!HasAnyNormalEnemyPrefab() && !HasAnyFastEnemyPrefab())
        {
            Debug.LogWarning($"{name}: No enemy prefabs assigned.");
            return false;
        }

        if (!HasAnyNormalSpawnPoint() && !HasAnyFastOnlySpawnPoint())
        {
            Debug.LogWarning($"{name}: No spawn points assigned.");
            return false;
        }

        return true;
    }

    private bool CanSpawnNormalEnemy()
    {
        if (aliveEnemies >= maxAliveEnemies)
        {
            return false;
        }

        return HasAnyNormalEnemyPrefab() && HasAnyNormalSpawnPoint();
    }

    private bool CanSpawnFastEnemy()
    {
        if (aliveEnemies >= maxAliveEnemies)
        {
            return false;
        }

        return HasAnyFastEnemyPrefab() && HasAnyFastOnlySpawnPoint();
    }

    private bool HasAnyNormalEnemyPrefab()
    {
        return meleeEnemyPrefab != null || rangedEnemyPrefab != null;
    }

    private bool HasAnyFastEnemyPrefab()
    {
        return fastMeleeEnemyPrefab != null || fastRangedEnemyPrefab != null;
    }

    private bool HasAnyNormalSpawnPoint()
    {
        return spawnPoints != null && spawnPoints.Length > 0;
    }

    private bool HasAnyFastOnlySpawnPoint()
    {
        return fastOnlySpawnPoints != null && fastOnlySpawnPoints.Length > 0;
    }

    private void SpawnNormalEnemy()
    {
        if (!CanSpawnNormalEnemy())
        {
            if (CanSpawnFastEnemy())
            {
                SpawnFastEnemy();
            }

            return;
        }

        Transform spawnPoint = GetValidSpawnPoint(spawnPoints);

        if (spawnPoint == null)
        {
            return;
        }

        GameObject prefab = ChooseNormalEnemyPrefab();

        if (prefab == null)
        {
            return;
        }

        SpawnEnemy(prefab, spawnPoint);
    }

    private void SpawnFastEnemy()
    {
        if (!CanSpawnFastEnemy())
        {
            if (CanSpawnNormalEnemy())
            {
                SpawnNormalEnemy();
            }

            return;
        }

        Transform spawnPoint = GetValidSpawnPoint(fastOnlySpawnPoints);

        if (spawnPoint == null)
        {
            return;
        }

        GameObject prefab = ChooseFastEnemyPrefab();

        if (prefab == null)
        {
            return;
        }

        SpawnEnemy(prefab, spawnPoint);
    }

    private void SpawnEnemy(GameObject prefab, Transform spawnPoint)
    {
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

    private Transform GetValidSpawnPoint(Transform[] availableSpawnPoints)
    {
        if (availableSpawnPoints == null || availableSpawnPoints.Length == 0)
        {
            return null;
        }

        const int maxAttempts = 20;

        for (int i = 0; i < maxAttempts; i++)
        {
            Transform candidate = availableSpawnPoints[Random.Range(0, availableSpawnPoints.Length)];

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

        return availableSpawnPoints[Random.Range(0, availableSpawnPoints.Length)];
    }

    private GameObject ChooseNormalEnemyPrefab()
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

    private GameObject ChooseFastEnemyPrefab()
    {
        bool shouldSpawnFastRanged = Random.value <= fastRangedSpawnChance;

        if (shouldSpawnFastRanged && fastRangedEnemyPrefab != null)
        {
            return fastRangedEnemyPrefab;
        }

        if (fastMeleeEnemyPrefab != null)
        {
            return fastMeleeEnemyPrefab;
        }

        return fastRangedEnemyPrefab;
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
        DrawSpawnPointGizmos(spawnPoints, Color.green, 0.5f);
        DrawSpawnPointGizmos(fastOnlySpawnPoints, Color.cyan, 0.65f);
    }

    private void DrawSpawnPointGizmos(Transform[] points, Color color, float radius)
    {
        if (points == null)
        {
            return;
        }

        Gizmos.color = color;

        foreach (Transform point in points)
        {
            if (point == null)
            {
                continue;
            }

            Gizmos.DrawWireSphere(point.position, radius);
        }
    }
}