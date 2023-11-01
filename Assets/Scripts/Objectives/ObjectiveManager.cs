using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public abstract class ObjectiveManager : GameManager
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

        if (objectiveDisplay != null)
        {
            description = "Objective Complete!";
            objectiveDisplay.color = Color.green;
        }
        ShowObjectiveBriefly();

        // Notify the GameManager that the level is completed
        GameManager.instance.MarkLevelAsCompleted(levelIndex);
        
        StartCoroutine(WaitAndReturnToLevelSelect(5f)); // Wait for the same duration as the blinking text
    }

    private IEnumerator WaitAndReturnToLevelSelect(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        SceneManager.LoadScene("LevelSelect"); // Replace with your Level Select scene name
    }


    protected virtual void UpdateObjectiveDescription()
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
            StartCoroutine(HideObjectiveAfterSecondsAndReset(5f)); // Hide after 5 seconds
        }
    }

    private System.Collections.IEnumerator HideObjectiveAfterSecondsAndReset(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        if (objectiveDisplay != null)
        {
            StopCoroutine(BlinkText());  // Stop blinking
            objectiveDisplay.color = Color.white;  // Reset color to white
            objectiveDisplay.gameObject.SetActive(false);
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
