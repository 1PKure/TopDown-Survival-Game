using UnityEngine;

public class EnemyDeathBehaviour : StateMachineBehaviour
{
    private EnemyBase enemy;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemy = animator.GetComponentInParent<EnemyBase>();

        if (enemy == null)
        {
            return;
        }

        enemy.SetChasing(false);
        enemy.SetAttacking(false);
    }
}