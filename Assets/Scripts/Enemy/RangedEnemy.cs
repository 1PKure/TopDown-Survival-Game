using UnityEngine;

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
        float distance = GetDistanceToTarget();

        return distance <= idealAttackRange && distance > tooCloseRange;
    }

    public override bool ShouldRetreat()
    {
        return GetDistanceToTarget() <= tooCloseRange;
    }

    public override void Retreat()
    {
        if (target == null || agent == null)
        {
            return;
        }

        Vector3 retreatDirection = (transform.position - target.position).normalized;
        Vector3 retreatPosition = transform.position + retreatDirection * retreatDistance;

        agent.isStopped = false;
        agent.speed = chaseSpeed;
        agent.SetDestination(retreatPosition);
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

        GameObject projectileInstance = Instantiate(
            projectilePrefab,
            firePoint.position,
            firePoint.rotation
        );

        EnemyProjectile projectile = projectileInstance.GetComponent<EnemyProjectile>();

        if (projectile == null)
        {
            return;
        }

        Vector3 direction = (target.position - firePoint.position).normalized;
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