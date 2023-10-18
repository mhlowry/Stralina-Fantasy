using UnityEngine;
using TMPro;

public abstract class ObjectiveManager : MonoBehaviour
{
    public string description;
    public bool isCompleted = false;

    // Reference to the TextMeshProUGUI component
    [SerializeField] protected TextMeshProUGUI questInfoText;


    protected virtual void Awake()
    {
        // Get the reference to the TextMeshProUGUI component
        questInfoText = GameObject.Find("Canvas/PauseScreen/PauseMenu/QuestBoard/QuestInfo/Text (TMP)").GetComponent<TextMeshProUGUI>();
    }

    public virtual void InitializeObjective() 
    {
        UpdateObjectiveDescription();
    }

    public virtual void CheckObjectiveCompletion() { }

    public virtual void CompleteObjective() 
    {
        isCompleted = true;
        Debug.Log("Objective completed: " + description);
    }

    // Method to update the TextMeshProUGUI component's text property
    protected void UpdateObjectiveDescription()
    {
        Debug.Log("Quest info text: " + questInfoText);
        if (questInfoText != null)
            questInfoText.text = description;
    }
}
