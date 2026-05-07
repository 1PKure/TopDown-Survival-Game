using UnityEngine;

public class EnemyPatrolBehaviour : StateMachineBehaviour
{
    private EnemyBase enemy;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemy = animator.GetComponentInParent<EnemyBase>();

        if (enemy == null)
        {
            return;
        }

        enemy.StartPatrol();
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (enemy == null)
        {
            return;
        }

        if (enemy.CanDetectTarget())
        {
            enemy.SetChasing(true);
            return;
        }

        enemy.UpdatePatrol();
    }
}