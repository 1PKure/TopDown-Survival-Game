using UnityEngine;

public class MeleeEnemy : EnemyBase
{
    [Header("Melee Settings")]
    [SerializeField] private float attackRange = 1.6f;

    public override bool CanAttack()
    {
        return GetDistanceToTarget() <= attackRange;
    }

    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}