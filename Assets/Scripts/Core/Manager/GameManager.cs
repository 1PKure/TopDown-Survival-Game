using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private EnemySpawner enemySpawner;
    [SerializeField] private EndGameUI endGameUI;
    [SerializeField] private ScoreManager scoreManager;

    private bool isGameOver;

    public bool IsGameOver => isGameOver;

    private void Awake()
    {
        if (enemySpawner == null)
        {
            enemySpawner = FindFirstObjectByType<EnemySpawner>();
        }

        if (endGameUI == null)
        {
            endGameUI = FindFirstObjectByType<EndGameUI>();
        }

        if (scoreManager == null)
        {
            scoreManager = FindFirstObjectByType<ScoreManager>();
        }
    }

    public void GameOver()
    {
        if (isGameOver)
        {
            return;
        }

        isGameOver = true;

        if (enemySpawner != null)
        {
            enemySpawner.StopSpawning();
        }

        if (scoreManager != null)
        {
            scoreManager.TryUpdateHighScore();
        }

        if (endGameUI != null)
        {
            endGameUI.ShowGameOver();
        }
        else
        {
            Debug.LogWarning($"{name}: EndGameUI reference is missing.");
            Time.timeScale = 0f;
        }

        Debug.Log("GAME OVER");
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;

        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}