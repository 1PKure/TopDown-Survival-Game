using UnityEngine;

public class EnemyAttackBehaviour : StateMachineBehaviour
{
    private EnemyBase enemy;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemy = animator.GetComponentInParent<EnemyBase>();

        if (enemy == null)
        {
            return;
        }

        enemy.StartAttack();
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (enemy == null)
        {
            return;
        }

        if (enemy.HasLostTarget())
        {
            enemy.SetAttacking(false);
            enemy.SetChasing(false);
            return;
        }

        if (!enemy.CanAttack())
        {
            enemy.SetAttacking(false);
            enemy.SetChasing(true);
            return;
        }

        enemy.UpdateAttack();
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (enemy == null)
        {
            return;
        }

        enemy.SetAttacking(false);
    }
}