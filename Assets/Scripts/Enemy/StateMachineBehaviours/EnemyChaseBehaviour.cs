using UnityEngine;

public class EnemyChaseBehaviour : StateMachineBehaviour
{
    private EnemyBase enemy;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemy = animator.GetComponentInParent<EnemyBase>();

        if (enemy == null)
        {
            Debug.LogError("EnemyBase not found from Chase Behaviour.");
            return;
        }

        enemy.StartChase();
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (enemy.IsDead)
        {
            return;
        }
        if (enemy == null)
        {
            return;
        }

        if (enemy.IsTargetDead())
        {
            enemy.SetChasing(false);
            enemy.SetAttacking(false);
            return;
        }

        if (enemy.HasLostTarget())
        {
            enemy.SetChasing(false);
            enemy.SetAttacking(false);
            return;
        }

        if (enemy.ShouldRetreat())
        {
            enemy.Retreat();
            return;
        }

        if (enemy.CanAttack())
        {
            enemy.SetAttacking(true);
            return;
        }

        enemy.UpdateChase();
    }
}