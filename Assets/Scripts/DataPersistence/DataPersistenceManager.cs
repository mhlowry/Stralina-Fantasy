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
        Debug.Log("Data saved in: " + Application.persistentDataPath);

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

            GameManager.instance.SetHeardDialogue(gameData.heardDialogue);
            Debug.Log("Loaded heardDialogue: " + gameData.heardDialogue);

            GameManager.instance.SetPlayerLevel(gameData.playerLevel);
            Debug.Log("Loaded current level: " + gameData.playerLevel);

            GameManager.instance.SetPlayerExp(gameData.curExp);
            Debug.Log("Loaded current exp: " + gameData.curExp);

            GameManager.instance.SetPlayerGold(gameData.curGold);
            Debug.Log("Loaded current gold: " + gameData.curGold);

            GameManager.instance.SetHasSpokenToShopkeeper(gameData.hasSpokenToShopkeeper);
            Debug.Log("Loaded hasSpokenToShopkeeper: " + gameData.hasSpokenToShopkeeper);

            GameManager.instance.SetLightAttackBoosts(gameData.lightAttackBoosts);
            Debug.Log("Loaded light attack boosts: " + gameData.lightAttackBoosts);

            GameManager.instance.SetHeavyAttackBoosts(gameData.heavyAttackBoosts);
            Debug.Log("Loaded heavy attack boosts: " + gameData.heavyAttackBoosts);

            GameManager.instance.SetSpeedBoosts(gameData.speedBoosts);
            Debug.Log("Loaded speed boosts: " + gameData.speedBoosts);

            GameManager.instance.SetWillpowerBoosts(gameData.willpowerBoosts);
            Debug.Log("Loaded willpower boosts: " + gameData.willpowerBoosts);

            GameManager.instance.SetHealthPointBoosts(gameData.healthPointBoosts);
            Debug.Log("Loaded health point boosts: " + gameData.healthPointBoosts);
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
        gameData.heardDialogue = GameManager.instance.GetHeardDialogue();
        gameData.playerLevel = GameManager.instance.GetPlayerLevel();
        gameData.curExp = GameManager.instance.GetPlayerExp();
        gameData.curGold = GameManager.instance.GetPlayerGold();
        gameData.hasSpokenToShopkeeper = GameManager.instance.GetHasSpokenToShopkeeper();
        gameData.lightAttackBoosts = GameManager.instance.GetLightAttackBoosts();
        gameData.heavyAttackBoosts = GameManager.instance.GetHeavyAttackBoosts();
        gameData.speedBoosts = GameManager.instance.GetSpeedBoosts();
        gameData.willpowerBoosts = GameManager.instance.GetWillpowerBoosts();
        gameData.healthPointBoosts = GameManager.instance.GetHealthPointBoosts();


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
