using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelectController : MonoBehaviour
{
    // Call this function when "Level 1" is clicked
    public void OnLevel1Clicked()
    {
        Debug.Log("Level 1 Clicked!");
        SceneManager.LoadScene("Eliminate Enemies Prototype", LoadSceneMode.Single);
    }
}
