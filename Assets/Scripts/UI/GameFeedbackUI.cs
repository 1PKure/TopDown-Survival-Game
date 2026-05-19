using System.Collections;
using TMPro;
using UnityEngine;

public class GameFeedbackUI : MonoBehaviour
{
    public static GameFeedbackUI Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI messageText;

    [Header("Settings")]
    [SerializeField] private float defaultDuration = 2f;

    private Coroutine currentMessageRoutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        HideMessage();
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