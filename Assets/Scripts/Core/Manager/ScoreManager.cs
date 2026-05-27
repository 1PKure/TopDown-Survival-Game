using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    private const string HighScoreKey = "HighScore";

    [SerializeField] private int currentScore;

    public int CurrentScore => currentScore;
    public int HighScore => PlayerPrefs.GetInt(HighScoreKey, 0);

    private void Start()
    {
        GameFeedbackUI.Instance?.SetScore(currentScore);
        GameFeedbackUI.Instance?.SetHighScore(HighScore);
    }

    public void AddScore(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        currentScore += amount;

        GameFeedbackUI.Instance?.SetScore(currentScore);
        TryUpdateHighScore();
    }

    public void RemoveScore(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        currentScore -= amount;
        currentScore = Mathf.Max(0, currentScore);

        GameFeedbackUI.Instance?.SetScore(currentScore);
    }

    public void ResetScore()
    {
        currentScore = 0;
        GameFeedbackUI.Instance?.SetScore(currentScore);
    }

    public bool TryUpdateHighScore()
    {
        if (currentScore <= HighScore)
        {
            return false;
        }

        PlayerPrefs.SetInt(HighScoreKey, currentScore);
        PlayerPrefs.Save();

        GameFeedbackUI.Instance?.SetHighScore(currentScore);
        return true;
    }
}