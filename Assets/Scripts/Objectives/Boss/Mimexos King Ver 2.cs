using UnityEngine;
using System.Collections;

public class MimexosKingVer2 : ObjectiveManager
{
    private bool isMimexosKingDestroyed = false;

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
        // Check if Mimexos King Ver 2 is still present in the scene
        Enemy mimexosKing = FindMimexosKingInScene();
        if (mimexosKing == null) // If not found, it means it has been destroyed
        {
            isMimexosKingDestroyed = true;
            Debug.Log("Mimexos King Ver 2 destroyed.");
            CheckObjectiveCompletion();
        }
    }

    private Enemy FindMimexosKingInScene()
    {
        // Search for Mimexos King Ver 2 in the scene
        Enemy[] allEnemies = FindObjectsOfType<Enemy>();
        foreach (Enemy enemy in allEnemies)
        {
            if (enemy.name == "Mimexos King Ver 2" && !enemy.GetIsDead())
            {
                Debug.Log($"Mimexos King Ver 2 found and is alive: {enemy.name}");
                return enemy;
            }
        }
        Debug.Log("Mimexos King Ver 2 not found or is dead.");
        return null; // Return null if Mimexos King Ver 2 is not found or is dead
    }


    public override void CheckObjectiveCompletion()
    {
        Debug.Log("Checking objective completion.");
        if (isMimexosKingDestroyed)
        {
            CompleteObjective();
        }
    }

    protected override void UpdateObjectiveDescription()
    {
        if (isMimexosKingDestroyed)
        {
            description = "Mimexos King Ver 2 has been defeated!";
        }
        else
        {
            description = "Defeat the Mimexos King!";
        }
        Debug.Log(description);
        base.UpdateObjectiveDescription();
    }

    public override void CompleteObjective()
    {
        base.CompleteObjective();
    }
}
