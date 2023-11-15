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

    // Called when base island is loaded
    public void InitializeLevelPortals()
    {
        for (int i = 0; i < levelsCompleted.Length; i++)
        {
            GameObject portal = GameObject.Find("Gate_Level " + (i + 1));
            if (portal)
            {
                // The first level's portal is always active.
                if (i == 0)
                {
                    portal.SetActive(true);
                }
                else
                {
                    // For subsequent levels, activate only if the previous level is completed
                    portal.SetActive(levelsCompleted[i - 1]);
                }
            }
            else
            {
                Debug.LogWarning("Portal for level " + (i + 1) + " not found!");
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
        if (scene.name == "Base_Scene")
        {
            InitializeLevelPortals();
            Debug.Log("Level portals loaded");
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

}
