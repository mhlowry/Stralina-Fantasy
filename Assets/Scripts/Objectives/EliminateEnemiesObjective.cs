using UnityEngine;
using System.Collections;
public class EliminateEnemiesObjective : ObjectiveManager
{
    public int totalEnemies;
    private int remainingEnemies;

    private void Start() 
    {
        Enemy.OnEnemyDestroyed += HandleEnemyDestroyed; // Subscribe to an event when an enemy is destroyed
        Debug.Log("Subscribed to OnEnemyDestroyed event.");

        InitializeObjective();
    }

    private void OnDestroy()
    {
        Enemy.OnEnemyDestroyed -= HandleEnemyDestroyed; // Unsubscribe when this objective is destroyed
        Debug.Log("Unsubscribed from OnEnemyDestroyed event.");
    }

    private void HandleEnemyDestroyed()
    {
        remainingEnemies--;
        Debug.Log($"Enemy destroyed. Remaining enemies: {remainingEnemies}");
        UpdateObjectiveDescription();
        CheckObjectiveCompletion();
    }

    public override void InitializeObjective()
    {
        Enemy[] allEnemies = FindObjectsOfType<Enemy>();
        totalEnemies = allEnemies.Length;
        remainingEnemies = totalEnemies;

        Debug.Log($"Objective initialized. Total enemies: {totalEnemies}");
        foreach (Enemy enemy in allEnemies)
        {
            //Debug.Log($"Detected enemy: {enemy.name}");
        }

        UpdateObjectiveDescription(); 
        base.InitializeObjective(); 
    }


    protected override void UpdateObjectiveDescription()
    {
        if (companionObject == null)
        {
            Debug.Log("companionObject is null, using default description.");
            description = "Urgent Quest!\n\n Eliminate all " + totalEnemies + " enemies on the map!\n\n (" + remainingEnemies + " remaining)";
            base.UpdateObjectiveDescription();
            Debug.Log(description);
        }
        else
        {
            description = "Protect your companion!\n\nEliminate all " + totalEnemies + " enemies on the map!\n\n (" + remainingEnemies + " remaining)";
            base.UpdateObjectiveDescription();
            Debug.Log(description);
        }
    }

    public override void CheckObjectiveCompletion()
    {
        Debug.Log($"Checking objective completion. Remaining enemies: {remainingEnemies}");
        if (remainingEnemies <= 0)
        {
            Debug.Log("Objective completed.");
            CompleteObjective();
        }
    }
}
