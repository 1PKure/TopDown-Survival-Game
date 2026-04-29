using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    private enum EnemyState
    {
        Idle,
        Patrol,
        Chase
    }

    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private Animator animator;

    [Header("Detection")]
    [SerializeField] private float detectionRange = 6f;
    [SerializeField] private float loseRange = 8f;

    [Header("Patrol")]
    [SerializeField] private float patrolWaitTime = 1.5f;

    private NavMeshAgent agent;
    private EnemyState currentState;
    private int currentPatrolIndex;
    private float waitTimer;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }
    }

    private void Start()
    {
        ChangeState(EnemyState.Patrol);
    }

    private void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        switch (currentState)
        {
            case EnemyState.Idle:
                UpdateIdle(distanceToPlayer);
                break;

            case EnemyState.Patrol:
                UpdatePatrol(distanceToPlayer);
                break;

            case EnemyState.Chase:
                UpdateChase(distanceToPlayer);
                break;
        }

        UpdateAnimator();
    }

    private void UpdateIdle(float distanceToPlayer)
    {
        if (distanceToPlayer <= detectionRange)
        {
            ChangeState(EnemyState.Chase);
            return;
        }

        waitTimer += Time.deltaTime;

        if (waitTimer >= patrolWaitTime)
        {
            ChangeState(EnemyState.Patrol);
        }
    }

    private void UpdatePatrol(float distanceToPlayer)
    {
        if (distanceToPlayer <= detectionRange)
        {
            ChangeState(EnemyState.Chase);
            return;
        }

        if (patrolPoints == null || patrolPoints.Length == 0)
        {
            ChangeState(EnemyState.Idle);
            return;
        }

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
            ChangeState(EnemyState.Idle);
        }
    }

    private void UpdateChase(float distanceToPlayer)
    {
        if (distanceToPlayer > loseRange)
        {
            ChangeState(EnemyState.Patrol);
            return;
        }

        agent.SetDestination(player.position);
    }

    private void ChangeState(EnemyState newState)
    {
        currentState = newState;

        switch (currentState)
        {
            case EnemyState.Idle:
                agent.ResetPath();
                waitTimer = 0f;
                break;

            case EnemyState.Patrol:
                if (patrolPoints != null && patrolPoints.Length > 0)
                {
                    agent.SetDestination(patrolPoints[currentPatrolIndex].position);
                }
                break;

            case EnemyState.Chase:
                agent.SetDestination(player.position);
                break;
        }
    }

    private void UpdateAnimator()
    {
        if (animator == null)
        {
            return;
        }

        animator.SetBool("isChasing", currentState == EnemyState.Chase);
        animator.SetFloat("speed", agent.velocity.magnitude);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, loseRange);
    }
}