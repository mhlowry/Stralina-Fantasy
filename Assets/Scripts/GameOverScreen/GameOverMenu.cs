using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverMenu : MonoBehaviour
{
    public GameObject gameOver;
    private float timeOfDeath;
    public static bool justDied = false;

    void Update()
    {
        //this is done for style points, waiting a couple seconds before completely stopping time and playing the music.
        if (Time.unscaledTime >= timeOfDeath + 2f && justDied)
        {
            Time.timeScale = 0f;
        }
        else if(justDied)
        {
            Time.timeScale = 0.5f;
        }
    }

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
        timeOfDeath = Time.unscaledTime;
        justDied = true;
    }

    public void ResetLevel()
    {
        Time.timeScale = 1f;
        Player.OnPlayerDeath -= OnEnableGameOver;
        justDied = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void MainMenu()
    {
        Time.timeScale = 1f;
        Player.OnPlayerDeath -= OnEnableGameOver;
        justDied = false;
        SceneManager.LoadScene("MainMenu");
    }
}
