using UnityEngine;

public class AmmoPickup : MonoBehaviour
{
    [Header("Ammo")]
    [SerializeField] private int ammoAmount = 12;

    [Header("Healing")]
    [SerializeField] private bool healPlayer = true;
    [SerializeField] private int healAmount = 15;

    [Header("Pickup")]
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

        if (healPlayer)
        {
            HealthComponent playerHealth = other.GetComponent<HealthComponent>();

            if (playerHealth == null)
            {
                playerHealth = other.GetComponentInParent<HealthComponent>();
            }

            if (playerHealth != null)
            {
                playerHealth.Heal(healAmount);
                GameFeedbackUI.Instance?.ShowMessage($"+{ammoAmount} Ammo / +{healAmount} HP");
            }
            else
            {
                GameFeedbackUI.Instance?.ShowMessage($"+{ammoAmount} Ammo");
            }
        }
        else
        {
            GameFeedbackUI.Instance?.ShowMessage($"+{ammoAmount} Ammo");
        }

        if (destroyOnPickup)
        {
            Destroy(gameObject);
        }
    }
}