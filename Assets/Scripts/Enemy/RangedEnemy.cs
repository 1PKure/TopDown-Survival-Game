using UnityEngine;
using UnityEngine.AI;

public class RangedEnemy : EnemyBase
{
    [Header("Ranged Settings")]
    [SerializeField] private float idealAttackRange = 7f;
    [SerializeField] private float tooCloseRange = 3f;
    [SerializeField] private float retreatDistance = 4f;

    [Header("Projectile")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float projectileSpeed = 12f;

    public override bool CanAttack()
    {
        if (IsTargetDead())
        {
            return false;
        }

        float distance = GetDistanceToTarget();

        return distance <= idealAttackRange && distance > tooCloseRange;
    }

    public override bool ShouldRetreat()
    {
        if (IsTargetDead())
        {
            return false;
        }

        return GetDistanceToTarget() <= tooCloseRange;
    }

    public override void Retreat()
    {
        if (target == null || agent == null)
        {
            return;
        }

        Vector3 retreatDirection = (transform.position - target.position).normalized;
        Vector3 desiredPosition = transform.position + retreatDirection * retreatDistance;

        if (NavMesh.SamplePosition(desiredPosition, out NavMeshHit hit, retreatDistance, NavMesh.AllAreas))
        {
            agent.isStopped = false;
            agent.speed = chaseSpeed;
            agent.SetDestination(hit.position);
        }
    }

    protected override void Attack()
    {
        Shoot();
    }

    private void Shoot()
    {
        if (projectilePrefab == null || firePoint == null || target == null)
        {
            return;
        }

        Vector3 direction = target.position - firePoint.position;
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.01f)
        {
            return;
        }

        direction.Normalize();

        Quaternion projectileRotation = Quaternion.LookRotation(direction);

        GameObject projectileInstance = Instantiate(
            projectilePrefab,
            firePoint.position,
            projectileRotation
        );

        EnemyProjectile projectile = projectileInstance.GetComponent<EnemyProjectile>();

        if (projectile == null)
        {
            Debug.LogWarning($"{name}: Projectile prefab does not have EnemyProjectile component.");
            return;
        }

        projectile.Initialize(direction, projectileSpeed, attackDamage);
    }

    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, idealAttackRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, tooCloseRange);
    }
}