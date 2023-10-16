using UnityEngine;

public abstract class ObjectiveManager : MonoBehaviour
{
    public string description; // Description of the objective
    public bool isCompleted = false; // If the objective is completed

    public virtual void InitializeObjective() 
    {
        // If we need to add default implementation, it will go here
    }

    public virtual void CheckObjectiveCompletion() 
    {
        // If we need to add default implementation, it will go here
    }

    /**
    someSortOfUpdateUIMethod();
    {
        When we have a UI to update, we will call this method here.
        This is where we will update the UI with the current status of the objective.
        For example, if we have a text box that displays the objective description, we will update it here.
        If we have a progress bar that displays the progress of the objective, we will update it here.
    }
    **/

    public virtual void CompleteObjective()
    {
        isCompleted = true;
        // I need to add any common logic for when an objective is completed here
        Debug.Log("Objective completed: " + description);
    }
}
