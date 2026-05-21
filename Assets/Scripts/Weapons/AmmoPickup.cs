using UnityEngine;

public class AmmoPickup : MonoBehaviour
{
    [Header("Ammo")]
    [SerializeField] private int ammoAmount = 12;

    [Header("Visual")]
    [SerializeField] private bool destroyOnPickup = true;

    private void OnTriggerEnter(Collider other)
    {
        PlayerAmmo playerAmmo = other.GetComponent<PlayerAmmo>();

        if (playerAmmo == null)
        {
            playerAmmo = other.GetComponentInParent<PlayerAmmo>();
        }

        if (playerAmmo == null)
        {
            return;
        }

        playerAmmo.AddReserveAmmo(ammoAmount);

        if (destroyOnPickup)
        {
            Destroy(gameObject);
        }
    }
}