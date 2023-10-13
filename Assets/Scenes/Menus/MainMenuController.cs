using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenuController : MonoBehaviour
{
    // Call this function when "Select Level" is clicked
    public void OnSelectLevelClicked()
    {
        Debug.Log("Select Level Clicked!");
        StartCoroutine(LoadSceneAsync("LevelSelect"));
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        // Asynchronously load the scene
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);

        // Wait until the scene has finished loading
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
}
