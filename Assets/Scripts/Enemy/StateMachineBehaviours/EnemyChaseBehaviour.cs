using UnityEngine;

public class EnemyChaseBehaviour : StateMachineBehaviour
{
    private EnemyBase enemy;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemy = animator.GetComponentInParent<EnemyBase>();

        if (enemy == null)
        {
            return;
        }

        enemy.StartChase();
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (enemy == null)
        {
            return;
        }

        if (enemy.HasLostTarget())
        {
            enemy.SetChasing(false);
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