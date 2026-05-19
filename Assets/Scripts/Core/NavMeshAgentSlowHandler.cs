using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NavMeshAgentSlowHandler : MonoBehaviour
{
    private NavMeshAgent agent;
    private float originalSpeed;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        originalSpeed = agent.speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out SlowZone slowZone))
        {
            agent.speed = originalSpeed * slowZone.SpeedMultiplier;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out SlowZone slowZone))
        {
            agent.speed = originalSpeed;
        }
    }
}