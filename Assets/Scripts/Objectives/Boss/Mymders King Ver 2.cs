using UnityEngine;
using System.Collections;

public class MymdersKingVer2 : ObjectiveManager
{
    private bool isMymdersKingDestroyed = false;

    private void Start()
    {
        // Subscribe to an event when an enemy is destroyed
        Enemy.OnEnemyDestroyed += HandleEnemyDestroyed;
        Debug.Log("Subscribed to OnEnemyDestroyed event.");
        InitializeObjective();
    }

    private void OnDestroy()
    {
        // Unsubscribe when this objective is destroyed
        Enemy.OnEnemyDestroyed -= HandleEnemyDestroyed;
        Debug.Log("Unsubscribed from OnEnemyDestroyed event.");
    }

    private void HandleEnemyDestroyed()
    {
        // Check if Mymders King Ver 2 is still present in the scene
        Enemy MymdersKing = FindMymdersKingInScene();
        if (MymdersKing == null) // If not found, it means it has been destroyed
        {
            isMymdersKingDestroyed = true;
            Debug.Log("Mymders King Ver 2 destroyed.");
            CheckObjectiveCompletion();
        }
    }

    private Enemy FindMymdersKingInScene()
    {
        // Search for Mymders King Ver 2 in the scene
        Enemy[] allEnemies = FindObjectsOfType<Enemy>();
        foreach (Enemy enemy in allEnemies)
        {
            if (enemy.name == "Mymders King Ver 2" && !enemy.GetIsDead())
            {
                Debug.Log($"Mymders King Ver 2 found and is alive: {enemy.name}");
                return enemy;
            }
        }
        Debug.Log("Mymders King Ver 2 not found or is dead.");
        return null; // Return null if Mymders King Ver 2 is not found or is dead
    }


    public override void CheckObjectiveCompletion()
    {
        Debug.Log("Checking objective completion.");
        if (isMymdersKingDestroyed)
        {
            CompleteObjective();
        }
    }

    protected override void UpdateObjectiveDescription()
    {
        if (isMymdersKingDestroyed)
        {
            description = "Mymders King Ver 2 has been defeated!";
        }
        else
        {
            description = "Defeat the Mymders King!";
        }
        Debug.Log(description);
        base.UpdateObjectiveDescription();
    }

    public override void CompleteObjective()
    {
        base.CompleteObjective();
    }
}
