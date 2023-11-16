using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenu;
    public DataPersistenceManager dataPersistenceManager;

    public static bool isPaused;

    // Start is called before the first frame update
    void Start()
    {
        pauseMenu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void ReturnToBase()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Base_Scene");
        isPaused = false;
    }

    public void LvlSelect()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("LevelSelect");
        isPaused = false;
    }

    public void ExitGame()
    {
        SaveGame();
        Application.Quit();
        Debug.Log("Game is exiting");
    }

    public void SaveGame()
    {
        if (DataPersistenceManager.instance != null)
        {
            DataPersistenceManager.instance.SaveGame();
            Debug.Log("Game Saved");
        }
        else
        {
            Debug.LogError("DataPersistenceManager instance not found");
        }
    }

}
