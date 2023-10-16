using UnityEngine;

public class EliminateEnemiesObjective : ObjectiveManager
{
    public int totalEnemies; // Total number of enemies at the start of the level
    private int remainingEnemies; // How many enemies remain
    // Override the InitializeObjective method
    public override void InitializeObjective()
    {
        base.InitializeObjective(); // Call parent's initialization
        remainingEnemies = FindObjectsOfType<Enemy>().Length; 
        description = "Eliminate all " + totalEnemies + " enemies on the map";
    }

    // Override the CheckObjectiveCompletion method
    public override void CheckObjectiveCompletion()
    {
        remainingEnemies = FindObjectsOfType<Enemy>().Length;
        if (remainingEnemies <= 0)
        {
            CompleteObjective();
        }
    }
}
