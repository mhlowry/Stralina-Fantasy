using System.Collections;
using UnityEngine;

public class WaveSpawner : ObjectiveManager
{
    [SerializeField] private float countdown;
    [SerializeField] private Transform[] spawnPoints;

    public Wave[] waves;
    public int currentWaveIndex = 0;
    private bool readyToCountDown;

    private void Start()
    {
        readyToCountDown = true;
        InitializeObjective();

        for (int i = 0; i < waves.Length; i++)
        {
            waves[i].enemiesLeft = waves[i].enemies.Length;
        }
    }

    private void Update()
    {
        if (currentWaveIndex >= waves.Length)
        {
            if (!isCompleted)
            {
                Debug.Log("You survived every wave!");
                CompleteObjective();
            }
            return; // This ensures no further code in Update() runs after completion
        }

        if (readyToCountDown)
        {
            countdown -= Time.deltaTime;
            if (countdown <= 0)
            {
                readyToCountDown = false;
                countdown = waves[currentWaveIndex].timeToNextWave;
                StartCoroutine(SpawnWave());
            }
        }

        // Ensure that we're within the bounds of the array
        if (currentWaveIndex < waves.Length && waves[currentWaveIndex].enemiesLeft == 0)
        {
            readyToCountDown = true;
            currentWaveIndex++;
            if (currentWaveIndex < waves.Length)
            {
                UpdateObjectiveDescription();
                ShowObjectiveBriefly();
            }
        }
    }

    private IEnumerator SpawnWave()
    {
        if (currentWaveIndex < waves.Length)
        {
            for (int i = 0; i < waves[currentWaveIndex].enemies.Length; i++)
            {
                GameObject enemy = Instantiate(waves[currentWaveIndex].enemies[i], spawnPoints[i % spawnPoints.Length]);
                enemy.transform.SetParent(spawnPoints[i % spawnPoints.Length]);

                yield return new WaitForSeconds(waves[currentWaveIndex].timeToNextEnemy);
            }
        }
    }

    protected override void UpdateObjectiveDescription()
    {
            int wavesRemaining = waves.Length - currentWaveIndex;
            string waveWord = wavesRemaining == 1 ? "wave" : "waves"; // Singular or plural

        if (companionObject == null)
        {
            description = "Urgent Quest!\n\nEliminate all " + waves.Length + " waves!\n\n(" + wavesRemaining + " " + waveWord + " remaining)";
            base.UpdateObjectiveDescription();
            Debug.Log(description);
        }
       else
       {
            description = "Urgent Quest!\n\nProtect your companion!\n\nEliminate all " + waves.Length + " waves!\n\n(" + wavesRemaining + " " + waveWord + " remaining)";
            base.UpdateObjectiveDescription();
            Debug.Log(description);
       }
    }

    public override void InitializeObjective()
    {
        UpdateObjectiveDescription(); 
        base.InitializeObjective(); 
    }

}

[System.Serializable]
public class Wave
{
    public GameObject[] enemies;
    public float timeToNextEnemy;
    public float timeToNextWave;
    [HideInInspector] public int enemiesLeft;
}
