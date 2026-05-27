using UnityEngine;

public class PlayerProjectile : MonoBehaviour
{
    [Header("Projectile")]
    [SerializeField] private float lifeTime = 2f;
    [SerializeField] private string ignoredTag = "Player";

    private Vector3 direction;
    private Vector3 startPosition;
    private float speed;
    private float maxDistance;
    private int damage;
    private bool initialized;

    public void Initialize(Vector3 shootDirection, float projectileSpeed, int projectileDamage, float projectileMaxDistance)
    {
        direction = shootDirection.normalized;
        speed = projectileSpeed;
        damage = projectileDamage;
        maxDistance = projectileMaxDistance;
        startPosition = transform.position;
        initialized = true;

        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        if (!initialized)
        {
            return;
        }

        transform.position += direction * speed * Time.deltaTime;

        if (direction.sqrMagnitude > 0.01f)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }

        float traveledDistance = Vector3.Distance(startPosition, transform.position);

        if (traveledDistance >= maxDistance)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(ignoredTag))
        {
            return;
        }

        RescueNpc rescueNpc = other.GetComponent<RescueNpc>();

        if (rescueNpc == null)
        {
            rescueNpc = other.GetComponentInParent<RescueNpc>();
        }

        if (rescueNpc != null && !rescueNpc.IsDead)
        {
            rescueNpc.TakeDamageFromPlayer(damage);
            Destroy(gameObject);
            return;
        }

        IDamageable damageable = other.GetComponent<IDamageable>();

        if (damageable == null)
        {
            damageable = other.GetComponentInParent<IDamageable>();
        }

        if (damageable != null && !damageable.IsDead)
        {
            damageable.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }

        if (!other.isTrigger)
        {
            Destroy(gameObject);
        }
    }
}