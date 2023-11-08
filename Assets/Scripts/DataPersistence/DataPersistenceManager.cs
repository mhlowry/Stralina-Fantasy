using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DataPersistenceManager : MonoBehaviour
{
    [Header("File Storage Config")]
    [SerializeField] private string fileName;
    private GameData gameData;
    private List<IDataPersistence> dataPersistenceObjects;
    private FileDataHandler dataHandler;

    // We'll be able to get the instance publically
    // However, we can only modify the instance privatly in this class
    public static DataPersistenceManager instance { get; private set; }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Debug.LogError("Found more than one instance of DataPersistenceManager! Destroying extra instances.");
            Destroy(gameObject);
            return;
        }

        instance = this;
    }


    private void Start()
    {
        this.dataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
        this.dataPersistenceObjects = FindAllDataPersistenceObjects();
        LoadGame();
    }

    public void NewGame()
    {
        this.gameData = new GameData();
    }

    public void LoadGame()
    {
        // Load any saved data from a file using the data handler
        this.gameData = dataHandler.Load();
        Debug.Log("Loaded game data: " + gameData);

        // if no data can be loaded, initialize to a new game
        if (this.gameData == null)
        {
            Debug.Log("No game data found. Initializing data to defaults.");
            NewGame();
        } 
        else 
        {
            GameManager.instance.SetLevelsCompleted(gameData.levelsCompleted);
            Debug.Log("Loaded levels completed: " + gameData.levelsCompleted);
            
            GameManager.instance.SetPlayerLevel(gameData.playerLevel);
            Debug.Log("Loaded current level: " + gameData.playerLevel);

            GameManager.instance.SetPlayerExp(gameData.curExp);
            Debug.Log("Loaded current exp: " + gameData.curExp);
        }

        // push the loaded data to all other scripts that need it.
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.LoadData(gameData);
        }
    }

    public void SaveGame()
    {
        gameData.levelsCompleted = GameManager.instance.GetLevelsCompleted();
        gameData.playerLevel = GameManager.instance.GetPlayerLevel();
        gameData.curExp = GameManager.instance.GetPlayerExp();

        // pass the data to other scripts so they can update it
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.SaveData(ref gameData);
        }

        // save that data to a file using the data handler.
        dataHandler.Save(gameData);
    }

    public void OnApplicationQuit()
    {
        SaveGame();
    }

    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        // Find all objects in the scene that implement IDataPersistence
        // and return them as a list
        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>()
            .OfType<IDataPersistence>();

        return new List<IDataPersistence>(dataPersistenceObjects);
    }
     
}
