using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] public bool[] levelsCompleted;
    private int playerLevel = 1;
    private int curExp = 0;

    const int maxLevel = 7;
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
        levelsCompleted = new bool[10];
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start() 
    {
        // InitializeLevelButtons();
    }

    // This function is called when the LevelSelect scene starts
    public void InitializeLevelButtons()
    {
        for (int i = 0; i < levelsCompleted.Length; i++)
        {
            Button levelButton = GameObject.Find("Level " + (i + 1)).GetComponent<Button>();
            if (levelButton)
            {
                levelButton.interactable = i == 0 || levelsCompleted[i - 1];
            }
        }
    }

    public void MarkLevelAsCompleted(int levelIndex)
    {
        Debug.Log("Marking level " + (levelIndex + 1) + " as completed");
        if (levelIndex < levelsCompleted.Length)
        {
            levelsCompleted[levelIndex] = true;

            // Save the game data immediately after marking a level as completed
            DataPersistenceManager.instance.SaveGame();
        }
        else
        {
            Debug.LogError("Level index out of bounds");
        }
    }

    public bool IsLevelCompleted(int levelIndex)
    {
        if (levelIndex < levelsCompleted.Length)
        {
            return levelsCompleted[levelIndex];
        }
        Debug.LogError("Level index out of bounds");
        return false;
    }

    public bool[] GetLevelsCompleted()
    {
        return levelsCompleted;
    }

    public void SetLevelsCompleted(bool[] completedLevels)
    {
        if (completedLevels.Length == levelsCompleted.Length)
        {
            levelsCompleted = completedLevels;
            Debug.Log("Levels completion status updated.");
            
            // // Call InitializeLevelButtons after updating levelsCompleted
            // InitializeLevelButtons();
        }
        else
        {
            Debug.LogError("Mismatch in levelsCompleted length.");
        }
    }

    public void SetPlayerLevel(int level)
    {
        playerLevel = level;
    }

    public void SetPlayerExp(int exp)
    {
        curExp = exp;
    }

    public int GetPlayerLevel()
    {
        return playerLevel;
    }

    public int GetPlayerExp()
    {
        return curExp;
    }

    // This method will be called every time a scene is loaded
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "LevelSelect")
        {
            InitializeLevelButtons();
            Debug.Log("LevelSelect buttons loaded");
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

}
