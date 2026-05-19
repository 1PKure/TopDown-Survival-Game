using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyMovementVariant : MonoBehaviour
{
    public enum MovementVariant
    {
        Slow,
        Fast
    }

    [Header("Variant")]
    [SerializeField] private MovementVariant movementVariant = MovementVariant.Slow;

    [Header("Speed Settings")]
    [SerializeField] private float slowSpeed = 2.5f;
    [SerializeField] private float fastSpeed = 5f;

    [Header("Acceleration Settings")]
    [SerializeField] private float slowAcceleration = 8f;
    [SerializeField] private float fastAcceleration = 14f;

    [Header("NavMesh Areas")]
    [SerializeField] private string jumpAreaName = "Jump";

    private NavMeshAgent agent;

    public MovementVariant CurrentVariant => movementVariant;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        ApplyVariant();
    }

    private void OnValidate()
    {
        if (!Application.isPlaying)
        {
            agent = GetComponent<NavMeshAgent>();

            if (agent != null)
            {
                ApplyVariant();
            }
        }
    }

    private void ApplyVariant()
    {
        int jumpAreaIndex = NavMesh.GetAreaFromName(jumpAreaName);
        int jumpAreaMask = jumpAreaIndex >= 0 ? 1 << jumpAreaIndex : 0;

        switch (movementVariant)
        {
            case MovementVariant.Slow:
                agent.speed = slowSpeed;
                agent.acceleration = slowAcceleration;
                agent.autoTraverseOffMeshLink = false;

                if (jumpAreaMask != 0)
                {
                    agent.areaMask &= ~jumpAreaMask;
                }

                break;

            case MovementVariant.Fast:
                agent.speed = fastSpeed;
                agent.acceleration = fastAcceleration;
                agent.autoTraverseOffMeshLink = true;

                if (jumpAreaMask != 0)
                {
                    agent.areaMask |= jumpAreaMask;
                }

                break;
        }
    }
}