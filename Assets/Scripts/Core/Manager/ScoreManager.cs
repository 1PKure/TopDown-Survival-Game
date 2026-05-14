using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private int currentScore;

    public int CurrentScore => currentScore;

    public void AddScore(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        currentScore += amount;

        Debug.Log($"Score: {currentScore}");

        // Later:
        // HUD.UpdateScore(currentScore);
    }

    public void ResetScore()
    {
        currentScore = 0;
    }
}