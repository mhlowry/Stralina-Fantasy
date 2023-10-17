using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverMenu : MonoBehaviour
{
    public GameObject gameOver;

    private void OnEnable()
    {
        Player.OnPlayerDeath += OnEnableGameOver;
    }

    private void OnDisable()
    {
        Player.OnPlayerDeath -= OnEnableGameOver;
    }

    public void OnEnableGameOver()
    {
        gameOver.SetActive(true);
    }

    public void ResetLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void MainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}
