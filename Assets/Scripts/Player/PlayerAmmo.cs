using System.Collections;
using UnityEngine;

public class PlayerAmmo : MonoBehaviour
{
    [Header("Magazine")]
    [SerializeField] private int magazineSize = 12;
    [SerializeField] private int currentAmmo = 12;

    [Header("Reserve Ammo")]
    [SerializeField] private int maxReserveAmmo = 60;
    [SerializeField] private int currentReserveAmmo = 24;

    [Header("Reload")]
    [SerializeField] private KeyCode reloadKey = KeyCode.R;
    [SerializeField] private float reloadDuration = 1.2f;

    private bool isReloading;

    public int CurrentAmmo => currentAmmo;
    public int MagazineSize => magazineSize;
    public int CurrentReserveAmmo => currentReserveAmmo;
    public bool IsReloading => isReloading;
    public bool HasAmmoInMagazine => currentAmmo > 0;
    public bool HasReserveAmmo => currentReserveAmmo > 0;

    private void Start()
    {
        ClampAmmoValues();
        UpdateAmmoUI();
    }

    private void Update()
    {
        if (Input.GetKeyDown(reloadKey))
        {
            TryReload();
        }
    }

    public bool TryConsumeAmmo()
    {
        if (isReloading)
        {
            GameFeedbackUI.Instance?.ShowMessage("Reloading...");
            return false;
        }

        if (currentAmmo <= 0)
        {
            GameFeedbackUI.Instance?.ShowMessage("No ammo. Press R to reload.");
            return false;
        }

        currentAmmo--;
        UpdateAmmoUI();

        if (currentAmmo <= 0 && currentReserveAmmo > 0)
        {
            GameFeedbackUI.Instance?.ShowMessage("Magazine empty. Press R to reload.");
        }

        return true;
    }

    public void TryReload()
    {
        if (isReloading)
        {
            return;
        }

        if (currentAmmo >= magazineSize)
        {
            GameFeedbackUI.Instance?.ShowMessage("Magazine already full.");
            return;
        }

        if (currentReserveAmmo <= 0)
        {
            GameFeedbackUI.Instance?.ShowMessage("No reserve ammo.");
            return;
        }

        StartCoroutine(ReloadRoutine());
    }

    public void AddReserveAmmo(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        currentReserveAmmo += amount;
        currentReserveAmmo = Mathf.Clamp(currentReserveAmmo, 0, maxReserveAmmo);

        UpdateAmmoUI();
    }

    private IEnumerator ReloadRoutine()
    {
        isReloading = true;

        GameFeedbackUI.Instance?.ShowMessage("Reloading...", reloadDuration);

        yield return new WaitForSeconds(reloadDuration);

        int missingAmmo = magazineSize - currentAmmo;
        int ammoToReload = Mathf.Min(missingAmmo, currentReserveAmmo);

        currentAmmo += ammoToReload;
        currentReserveAmmo -= ammoToReload;

        isReloading = false;

        UpdateAmmoUI();

        GameFeedbackUI.Instance?.ShowMessage("Reloaded.");
    }

    private void ClampAmmoValues()
    {
        magazineSize = Mathf.Max(1, magazineSize);
        maxReserveAmmo = Mathf.Max(0, maxReserveAmmo);

        currentAmmo = Mathf.Clamp(currentAmmo, 0, magazineSize);
        currentReserveAmmo = Mathf.Clamp(currentReserveAmmo, 0, maxReserveAmmo);
    }

    private void UpdateAmmoUI()
    {
        GameFeedbackUI.Instance?.SetAmmo(currentAmmo, magazineSize, currentReserveAmmo);
    }
}