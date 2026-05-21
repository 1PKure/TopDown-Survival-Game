using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private int currentScore;

    public int CurrentScore => currentScore;

    private void Start()
    {
        GameFeedbackUI.Instance?.SetScore(currentScore);
    }

    public void AddScore(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        currentScore += amount;

        GameFeedbackUI.Instance?.SetScore(currentScore);
        GameFeedbackUI.Instance?.ShowMessage($"+{amount} Score");
    }

    public void ResetScore()
    {
        currentScore = 0;
        GameFeedbackUI.Instance?.SetScore(currentScore);
    }
}