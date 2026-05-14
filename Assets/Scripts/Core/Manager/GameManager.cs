using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private bool isGameOver;
    [SerializeField] private EnemySpawner enemySpawner;
    public bool IsGameOver => isGameOver;

    public void GameOver()
    {
        if (enemySpawner != null)
        {
            enemySpawner.StopSpawning();
        }
        
        if (isGameOver)
        {
            return;
        }

        isGameOver = true;

        Debug.Log("GAME OVER");

        Time.timeScale = 0f;

        // Later:
        // gameOverPanel.SetActive(true);
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
    }
}