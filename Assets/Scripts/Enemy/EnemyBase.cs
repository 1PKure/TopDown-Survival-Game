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
    private float waitTimer;
    private float attackTimer;

    private static readonly int SpeedHash = Animator.StringToHash("speed");
    private static readonly int IsChasingHash = Animator.StringToHash("isChasing");
    private static readonly int IsAttackingHash = Animator.StringToHash("isAttacking");
    private static readonly int IsDeadHash = Animator.StringToHash("isDead");

    public Transform Target => target;
    public NavMeshAgent Agent => agent;
    public Animator Animator => animator;

    public float DetectionRange => detectionRange;
    public float LoseRange => loseRange;
    public float PatrolSpeed => patrolSpeed;
    public float ChaseSpeed => chaseSpeed;
    public float AttackCooldown => attackCooldown;

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
        if (target != null)
        {
            targetDamageable = target.GetComponent<IDamageable>();
        }

        healthComponent.OnDeath += Die;
    }

    protected virtual void Update()
    {
        UpdateAnimatorSpeed();
    }

    protected virtual void OnDestroy()
    {
        if (healthComponent != null)
        {
            healthComponent.OnDeath -= Die;
        }
    }

    public virtual bool HasTarget()
    {
        return target != null;
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
        return GetDistanceToTarget() <= detectionRange;
    }

    public virtual bool HasLostTarget()
    {
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

        if (patrolPoints != null && patrolPoints.Length > 0)
        {
            agent.SetDestination(patrolPoints[currentPatrolIndex].position);
        }

        waitTimer = 0f;
        SetChasing(false);
        SetAttacking(false);
    }

    public virtual void UpdatePatrol()
    {
        if (patrolPoints == null || patrolPoints.Length == 0)
        {
            return;
        }

        if (agent.pathPending)
        {
            return;
        }

        if (agent.remainingDistance > agent.stoppingDistance)
        {
            return;
        }

        waitTimer += Time.deltaTime;

        if (waitTimer < patrolWaitTime)
        {
            return;
        }

        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
        agent.SetDestination(patrolPoints[currentPatrolIndex].position);
        waitTimer = 0f;
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

        SetChasing(false);
        SetAttacking(true);
    }

    public virtual void UpdateAttack()
    {
        FaceTarget();

        attackTimer += Time.deltaTime;

        if (attackTimer >= attackCooldown)
        {
            attackTimer = 0f;
            Attack();
        }
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

    private void Die()
    {
        if (agent != null)
        {
            agent.isStopped = true;
            agent.ResetPath();
        }

        SetChasing(false);
        SetAttacking(false);
        SetDead(true);

        // Después conectamos score y destroy:
        // ScoreManager.Instance.AddScore(scoreValue);
        // Destroy(gameObject, 3f);
    }

    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, loseRange);
    }
}