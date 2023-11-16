using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenuManager : MonoBehaviour
{
    public GameObject usernamePanel;  // Reference to the panel
    public TMP_InputField usernameInput;  // Reference to the input field

    private void Start()
    {
        // Initially hide the username panel
        usernamePanel.SetActive(false);
    }

    // Call this function when "New Game" is clicked
    public void OnNewGameClicked()
    {
        Debug.Log("New Game Clicked!");
        usernamePanel.SetActive(true);
    }

    // Temporary function before profiles are implemented
    public void OnStartGameClicked()
    {
        Debug.Log("Start Game Clicked!");
        // Start coroutine to load the base island scene
        StartCoroutine(LoadSceneAndSetActive("Base_Scene"));
    }

    public void OnUsernameConfirmClicked()
    {
        Debug.Log("Confirm Clicked!");
        string username = usernameInput.text;
        if (string.IsNullOrEmpty(username))
        {
            Debug.LogWarning("Username is empty!");
            return;
        }
        usernamePanel.SetActive(false);
        Debug.Log("Username: " + username);

        // Start coroutine to load the base island scene
        StartCoroutine(LoadSceneAndSetActive("Base_Scene"));
    }

    private IEnumerator LoadSceneAndSetActive(string sceneName)
    {
        // Asynchronously load the scene
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);

        // Wait until the scene has finished loading
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // Set the loaded scene as active
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
    }
}
