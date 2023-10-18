using UnityEngine;
using TMPro;

public abstract class ObjectiveManager : MonoBehaviour
{
    public string description;
    public bool isCompleted = false;

    // Reference to the TextMeshProUGUI components
    [SerializeField] protected TextMeshProUGUI questInfoText;
    [SerializeField] protected TextMeshProUGUI objectiveDisplay;

    private void Awake()
    {
        if (objectiveDisplay != null)
        {
            objectiveDisplay.enabled = true;  // Make sure the text starts as visible for the blinking effect
        }
    }

    public virtual void InitializeObjective()
    {
        UpdateObjectiveDescription();
        ShowObjectiveBriefly();
    }

    public virtual void CheckObjectiveCompletion() { }

    public virtual void CompleteObjective()
    {
        isCompleted = true;
        Debug.Log("Objective completed: " + description);
    }

    protected void UpdateObjectiveDescription()
    {
        if (questInfoText != null)
            questInfoText.text = description;
    }

    // Method to update the objective display and show it briefly
    protected void ShowObjectiveBriefly()
    {
        if (objectiveDisplay != null)
        {
            objectiveDisplay.text = description;
            objectiveDisplay.gameObject.SetActive(true); // Make it visible
            StartCoroutine(BlinkText());  // Start blinking
            StartCoroutine(HideObjectiveAfterSeconds(5f)); // Hide after 5 seconds
        }
    }

    private System.Collections.IEnumerator BlinkText()
    {
        while (objectiveDisplay.gameObject.activeSelf)
        {
            objectiveDisplay.enabled = !objectiveDisplay.enabled;  // Toggle visibility
            yield return new WaitForSeconds(0.5f);  // Time interval for blinking, adjust as needed
        }
    }

    // Coroutine to hide the objective display after some seconds
    private System.Collections.IEnumerator HideObjectiveAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        if (objectiveDisplay != null)
        {
            StopCoroutine(BlinkText());  // Stop blinking
            objectiveDisplay.gameObject.SetActive(false);
        }
    }
}
