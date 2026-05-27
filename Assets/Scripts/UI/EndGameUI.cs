using TMPro;
using UnityEngine;

public class EndGameUI : MonoBehaviour
{
    [Header("Panel")]
    [SerializeField] private GameObject endGamePanel;

    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;

    [Header("References")]
    [SerializeField] private ScoreManager scoreManager;

    private void Awake()
    {
        if (scoreManager == null)
        {
            scoreManager = FindFirstObjectByType<ScoreManager>();
        }

        if (endGamePanel != null)
        {
            endGamePanel.SetActive(false);
        }
    }

    public void ShowGameOver()
    {
        Time.timeScale = 0f;

        if (scoreManager != null)
        {
            scoreManager.TryUpdateHighScore();
        }

        if (endGamePanel != null)
        {
            endGamePanel.SetActive(true);
        }

        if (titleText != null)
        {
            titleText.text = "Game Over";
        }

        if (finalScoreText != null && scoreManager != null)
        {
            finalScoreText.text = $"Final Score: {scoreManager.CurrentScore}";
        }

        if (highScoreText != null && scoreManager != null)
        {
            highScoreText.text = $"High Score: {scoreManager.HighScore}";
        }

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}