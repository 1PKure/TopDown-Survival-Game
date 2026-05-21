using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameFeedbackUI : MonoBehaviour
{
    public static GameFeedbackUI Instance { get; private set; }

    [Header("Message UI")]
    [SerializeField] private TextMeshProUGUI messageText;

    [Header("Gameplay UI")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI enemiesAliveText;
    [SerializeField] private TextMeshProUGUI ammoText;

    [Header("Health UI")]
    [SerializeField] private RectTransform healthFillRect;
    [SerializeField] private TextMeshProUGUI healthText;

    [Header("Settings")]
    [SerializeField] private float defaultDuration = 2f;

    private Coroutine currentMessageRoutine;
    private int enemiesAlive;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        HideMessage();
        SetScore(0);
        SetEnemiesAlive(0);
    }

    public void ShowMessage(string message)
    {
        ShowMessage(message, defaultDuration);
    }

    public void ShowMessage(string message, float duration)
    {
        if (messageText == null)
        {
            Debug.LogWarning("GameFeedbackUI: Message Text is not assigned.");
            return;
        }

        if (currentMessageRoutine != null)
        {
            StopCoroutine(currentMessageRoutine);
        }

        currentMessageRoutine = StartCoroutine(ShowMessageRoutine(message, duration));
    }

    public void SetScore(int score)
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {score}";
        }
    }

    public void SetEnemiesAlive(int amount)
    {
        enemiesAlive = Mathf.Max(0, amount);

        if (enemiesAliveText != null)
        {
            enemiesAliveText.text = $"Enemies: {enemiesAlive}";
        }
    }

    public void AddEnemy()
    {
        SetEnemiesAlive(enemiesAlive + 1);
    }

    public void RemoveEnemy()
    {
        SetEnemiesAlive(enemiesAlive - 1);
    }

    public void SetHealth(int currentHealth, int maxHealth)
    {
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        float healthPercent = maxHealth > 0
            ? (float)currentHealth / maxHealth
            : 0f;

        if (healthFillRect != null)
        {
            healthFillRect.anchorMax = new Vector2(healthPercent, 1f);
        }

        if (healthText != null)
        {
            healthText.text = $"HP: {currentHealth}/{maxHealth}";
        }
    }

    public void SetAmmo(int currentAmmo, int magazineSize, int reserveAmmo)
    {
        if (ammoText != null)
        {
            ammoText.text = $"Ammo: {currentAmmo}/{magazineSize} | Reserve: {reserveAmmo}";
        }
    }

    private IEnumerator ShowMessageRoutine(string message, float duration)
    {
        messageText.text = message;
        messageText.gameObject.SetActive(true);

        yield return new WaitForSeconds(duration);

        HideMessage();
    }

    private void HideMessage()
    {
        if (messageText != null)
        {
            messageText.text = string.Empty;
            messageText.gameObject.SetActive(false);
        }
    }
}