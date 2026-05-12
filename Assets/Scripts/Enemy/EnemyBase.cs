using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(HealthComponent))]
public abstract class EnemyBase : MonoBehaviour
{
    [Header("References")]
    [SerializeField] protected Transform target;
    [SerializeField] protected Animator animator;

    [Header("Patrol")]
    [SerializeField] protected Transform[] patrolPoints;
    [SerializeField] protected float patrolWaitTime = 1.5f;

    [Header("Detection")]
    [SerializeField] protected float detectionRange = 8f;
    [SerializeField] protected float loseRange = 12f;

    [Header("Movement")]
    [SerializeField] protected float patrolSpeed = 1.8f;
    [SerializeField] protected float chaseSpeed = 3.2f;
    [SerializeField] protected float rotationSpeed = 12f;

    [Header("Combat")]
    [SerializeField] protected int attackDamage = 10;
    [SerializeField] protected float attackCooldown = 1.2f;

    protected NavMeshAgent agent;
    protected HealthComponent healthComponent;
    protected IDamageable targetDamageable;

    private int currentPatrolIndex;
    private float patrolWaitTimer;
    private bool isWaitingAtPatrolPoint;
    private bool patrolIndexInitialized;

    private float attackTimer;

    private static readonly int SpeedHash = Animator.StringToHash("speed");
    private static readonly int IsChasingHash = Animator.StringToHash("isChasing");
    private static readonly int IsAttackingHash = Animator.StringToHash("isAttacking");
    private static readonly int IsDeadHash = Animator.StringToHash("isDead");

    public Transform Target => target;
    public NavMeshAgent Agent => agent;

    protected virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        healthComponent = GetComponent<HealthComponent>();

        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }
    }

    protected virtual void Start()
    {
        ResolveTargetDamageable();

        if (healthComponent != null)
        {
            healthComponent.OnDeath += HandleDeath;
        }

        SetChasing(false);
        SetAttacking(false);
        SetDead(false);
    }

    protected virtual void Update()
    {
        UpdateAnimatorSpeed();
    }

    protected virtual void OnDestroy()
    {
        if (healthComponent != null)
        {
            healthComponent.OnDeath -= HandleDeath;
        }
    }

    private void ResolveTargetDamageable()
    {
        if (target == null)
        {
            Debug.LogWarning($"{name}: Target is not assigned.");
            return;
        }

        targetDamageable = target.GetComponent<IDamageable>();

        if (targetDamageable == null)
        {
            targetDamageable = target.GetComponentInParent<IDamageable>();
        }

        if (targetDamageable == null)
        {
            targetDamageable = target.GetComponentInChildren<IDamageable>();
        }

        if (targetDamageable == null)
        {
            Debug.LogWarning($"{name}: Target has no IDamageable / HealthComponent.");
        }
    }

    public virtual bool IsTargetDead()
    {
        return targetDamageable != null && targetDamageable.IsDead;
    }

    public virtual float GetDistanceToTarget()
    {
        if (target == null)
        {
            return Mathf.Infinity;
        }

        return Vector3.Distance(transform.position, target.position);
    }

    public virtual bool CanDetectTarget()
    {
        if (target == null || IsTargetDead())
        {
            return false;
        }

        return GetDistanceToTarget() <= detectionRange;
    }

    public virtual bool HasLostTarget()
    {
        if (target == null || IsTargetDead())
        {
            return true;
        }

        return GetDistanceToTarget() > loseRange;
    }

    public virtual void StartPatrol()
    {
        if (agent == null)
        {
            return;
        }

        agent.isStopped = false;
        agent.speed = patrolSpeed;

        SetChasing(false);
        SetAttacking(false);

        patrolWaitTimer = 0f;
        isWaitingAtPatrolPoint = false;

        if (patrolPoints == null || patrolPoints.Length == 0)
        {
            Debug.LogWarning($"{name}: No patrol points assigned.");
            return;
        }

        if (!patrolIndexInitialized)
        {
            currentPatrolIndex = Random.Range(0, patrolPoints.Length);
            patrolIndexInitialized = true;
        }

        SetPatrolDestination();
    }

    public virtual void UpdatePatrol()
    {
        if (agent == null)
        {
            return;
        }

        if (patrolPoints == null || patrolPoints.Length == 0)
        {
            return;
        }

        if (agent.pathPending)
        {
            return;
        }

        if (!isWaitingAtPatrolPoint && agent.remainingDistance <= agent.stoppingDistance)
        {
            isWaitingAtPatrolPoint = true;
            patrolWaitTimer = 0f;

            agent.isStopped = true;
            agent.ResetPath();

            return;
        }

        if (!isWaitingAtPatrolPoint)
        {
            return;
        }

        patrolWaitTimer += Time.deltaTime;

        if (patrolWaitTimer < patrolWaitTime)
        {
            return;
        }

        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;

        isWaitingAtPatrolPoint = false;
        patrolWaitTimer = 0f;

        agent.isStopped = false;

        SetPatrolDestination();
    }

    private void SetPatrolDestination()
    {
        if (patrolPoints == null || patrolPoints.Length == 0)
        {
            return;
        }

        Transform patrolPoint = patrolPoints[currentPatrolIndex];

        if (patrolPoint == null)
        {
            Debug.LogWarning($"{name}: Patrol point at index {currentPatrolIndex} is null.");
            return;
        }

        agent.SetDestination(patrolPoint.position);
    }

    public virtual void StartChase()
    {
        if (agent == null || target == null)
        {
            return;
        }

        agent.isStopped = false;
        agent.speed = chaseSpeed;

        SetChasing(true);
        SetAttacking(false);
    }

    public virtual void UpdateChase()
    {
        if (agent == null || target == null)
        {
            return;
        }

        agent.isStopped = false;
        agent.speed = chaseSpeed;
        agent.SetDestination(target.position);
    }

    public virtual void StartAttack()
    {
        if (agent != null)
        {
            agent.isStopped = true;
            agent.ResetPath();
        }

        attackTimer = attackCooldown;

        SetAttacking(true);
    }

    public virtual void UpdateAttack()
    {
        FaceTarget();

        attackTimer += Time.deltaTime;

        if (attackTimer < attackCooldown)
        {
            return;
        }

        attackTimer = 0f;
        Attack();
    }

    protected virtual void Attack()
    {
        DealDamageToTarget();
    }

    protected virtual void DealDamageToTarget()
    {
        if (targetDamageable == null || targetDamageable.IsDead)
        {
            return;
        }

        targetDamageable.TakeDamage(attackDamage);
    }

    public virtual bool CanAttack()
    {
        return false;
    }

    public virtual bool ShouldRetreat()
    {
        return false;
    }

    public virtual void Retreat()
    {
    }

    public virtual void FaceTarget()
    {
        if (target == null)
        {
            return;
        }

        Vector3 direction = target.position - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.01f)
        {
            return;
        }

        Quaternion targetRotation = Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }

    public virtual void StopMovement()
    {
        if (agent == null)
        {
            return;
        }

        agent.isStopped = true;
        agent.ResetPath();
    }

    public virtual void SetChasing(bool value)
    {
        if (animator == null)
        {
            return;
        }

        animator.SetBool(IsChasingHash, value);
    }

    public virtual void SetAttacking(bool value)
    {
        if (animator == null)
        {
            return;
        }

        animator.SetBool(IsAttackingHash, value);
    }

    public virtual void SetDead(bool value)
    {
        if (animator == null)
        {
            return;
        }

        animator.SetBool(IsDeadHash, value);
    }

    private void UpdateAnimatorSpeed()
    {
        if (animator == null || agent == null)
        {
            return;
        }

        animator.SetFloat(SpeedHash, agent.velocity.magnitude);
    }

    private void HandleDeath()
    {
        StopMovement();

        SetChasing(false);
        SetAttacking(false);
        SetDead(true);
    }

    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, loseRange);
    }
}