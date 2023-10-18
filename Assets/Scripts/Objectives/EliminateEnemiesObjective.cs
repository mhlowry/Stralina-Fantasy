using UnityEngine;

public class EliminateEnemiesObjective : ObjectiveManager
{
    public int totalEnemies;
    private int remainingEnemies;

    private void Awake()
    {
        Enemy.OnEnemyDestroyed += HandleEnemyDestroyed; // Subscribe to an event when an enemy is destroyed
    }

    private void Start() 
    {
        InitializeObjective();
    }

    private void OnDestroy()
    {
        Enemy.OnEnemyDestroyed -= HandleEnemyDestroyed; // Unsubscribe when this objective is destroyed
    }

    private void HandleEnemyDestroyed()
    {
        remainingEnemies--;
        UpdateObjectiveDescription();
        CheckObjectiveCompletion();
    }

    public override void InitializeObjective()
    {
        totalEnemies = FindObjectsOfType<Enemy>().Length;
        remainingEnemies = totalEnemies;
        UpdateObjectiveDescription(); 
        base.InitializeObjective(); 
    }


    private void UpdateObjectiveDescription()
    {
        description = "Urgent Quest!\n\n Eliminate all " + totalEnemies + " enemies on the map!\n\n (" + remainingEnemies + " remaining)";
        base.UpdateObjectiveDescription();
        Debug.Log(description);
    }

    public override void CheckObjectiveCompletion()
    {
        if (remainingEnemies <= 0)
        {
            CompleteObjective();
        }
    }
}
