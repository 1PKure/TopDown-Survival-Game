using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private PlayerAmmo playerAmmo;

    [Header("Weapon")]
    [SerializeField] private int damage = 25;
    [SerializeField] private float projectileSpeed = 18f;
    [SerializeField] private float shootCooldown = 0.45f;
    [SerializeField] private float projectileMaxDistance = 10f;

    private float nextShootTime;

    private void Awake()
    {
        if (playerAmmo == null)
        {
            playerAmmo = GetComponent<PlayerAmmo>();
        }

        if (playerAmmo == null)
        {
            playerAmmo = GetComponentInParent<PlayerAmmo>();
        }
    }

    private void Update()
    {
        if (Time.timeScale == 0f)
        {
            return;
        }

        if (Input.GetMouseButton(0))
        {
            TryShoot();
        }
    }

    private void TryShoot()
    {
        if (Time.time < nextShootTime)
        {
            return;
        }

        if (projectilePrefab == null || firePoint == null)
        {
            Debug.LogWarning($"{name}: Projectile prefab or fire point is missing.");
            return;
        }

        if (playerAmmo == null)
        {
            Debug.LogWarning($"{name}: PlayerAmmo reference is missing.");
            return;
        }

        if (!playerAmmo.TryConsumeAmmo())
        {
            return;
        }

        nextShootTime = Time.time + shootCooldown;
        Shoot();
    }

    private void Shoot()
    {
        Vector3 direction = transform.forward;
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.01f)
        {
            return;
        }

        direction.Normalize();

        Quaternion projectileRotation = Quaternion.LookRotation(direction);

        GameObject projectileInstance = Instantiate(
            projectilePrefab,
            firePoint.position,
            projectileRotation
        );

        PlayerProjectile projectile = projectileInstance.GetComponent<PlayerProjectile>();

        if (projectile == null)
        {
            Debug.LogWarning($"{name}: PlayerProjectile component missing in projectile prefab.");
            return;
        }

        projectile.Initialize(direction, projectileSpeed, damage, projectileMaxDistance);
    }
}