using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using TMPro;

public class GameOverMenu : MonoBehaviour
{
    public GameObject gameOver;
    private float timeOfDeath;
    public static bool justDied = false;
    public PlayerInput playerInput;
    [SerializeField] private TMPro.TextMeshProUGUI gameOverText; // set this in inspector pls

    [SerializeField] GameObject DJ;

    private void Awake()
    {
        DJ = GameObject.Find("DJObject");
        justDied = false;
    }

    void Update()
    {
        // This is done for style points, waiting a couple seconds before completely stopping time and playing the music.
        if (Time.unscaledTime >= timeOfDeath + 2f && justDied)
        {
            Time.timeScale = 0f;
        }
        else if (justDied)
        {
            Time.timeScale = 0.5f;
        }
    }

    private void OnEnable()
    {
        Player.OnPlayerDeath += OnEnableGameOver;
        Companion.OnCompanionDeath += OnEnableGameOver; 
    }

    private void OnDisable()
    {
        Player.OnPlayerDeath -= OnEnableGameOver;
        Companion.OnCompanionDeath -= OnEnableGameOver;
    }

    public void OnEnableGameOver()
    {
        gameOver.SetActive(true);
        Destroy(DJ);
        playerInput.SwitchCurrentActionMap("UI");
        timeOfDeath = Time.unscaledTime;
        justDied = true;
    }

    public void ResetLevel()
    {
        Time.timeScale = 1f;
        playerInput.SwitchCurrentActionMap("Gameplay");
        UnsubscribeEvents();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void BaseIsland()
    {
        Time.timeScale = 1f;
        UnsubscribeEvents();
        SceneManager.LoadScene("Base_Scene");
    }

    private void UnsubscribeEvents()
    {
        Player.OnPlayerDeath -= OnEnableGameOver;
        Companion.OnCompanionDeath -= OnEnableGameOver;
        justDied = false;
    }

    // Method to set custom text on the game over screen
    public void SetGameOverText(string text)
    {
        if (gameOverText != null)
        {
            gameOverText.text = text;
        }
    }
}
