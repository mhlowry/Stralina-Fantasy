using UnityEngine;

public class EliminateEnemyWavesObjective : ObjectiveManager
{
    public int totalWaves;
    private int currentWave = 0;
    private int enemiesPerWave;
    private int remainingEnemies;

    private void Awake()
    {
        Enemy.OnEnemyDestroyed += HandleEnemyDestroyed; // Subscribe to the enemy destroyed event
    }

    private void Start() 
    {
        InitializeObjective();
    }

    private void OnDestroy()
    {
        Enemy.OnEnemyDestroyed -= HandleEnemyDestroyed; // Unsubscribe from the enemy destroyed event
    }

    private void HandleEnemyDestroyed()
    {
        remainingEnemies--;
        UpdateObjectiveDescription();
        CheckWaveCompletion();
    }

    public override void InitializeObjective()
    {
        StartWave();
        base.InitializeObjective();
    }

    private void StartWave()
    {
        currentWave++;
        //enemiesPerWave = CalculateEnemiesPerWave(currentWave); 
        remainingEnemies = enemiesPerWave;
        UpdateObjectiveDescription();
    }

    private void CheckWaveCompletion()
    {
        if (remainingEnemies <= 0)
        {
            if (currentWave >= totalWaves)
            {
                CompleteObjective();
            }
            else
            {
                StartWave();
            }
        }
    }

    protected override void UpdateObjectiveDescription()
    {
        description = "Wave " + currentWave + " of " + totalWaves + 
                      "\nEliminate all " + enemiesPerWave + " enemies!\n(" + remainingEnemies + " remaining)";
        base.UpdateObjectiveDescription();
    }
}
