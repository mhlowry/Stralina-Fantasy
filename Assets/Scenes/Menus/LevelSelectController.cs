using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelectController : ObjectiveManager
{

    private void Start()
    {
        //GameManager.instance.InitializeLevelButtons();
    }

    // Call this function when "Level 1" is clicked
    public void OnLevel1Clicked()
    {
        Debug.Log("Level 1 Clicked!");
        //levelIndex = 0;
        SceneManager.LoadScene("Eliminate Enemies Prototype", LoadSceneMode.Single);
    }

    // Call this function when "Level 2" is clicked
    public void OnLevel2Clicked()
    {
        Debug.Log("Level 2 Clicked!");
        //levelIndex = 1;
        SceneManager.LoadScene("Enemy Waves Prototype", LoadSceneMode.Single);
    }

    // Call this function when "Level 3" is clicked
    public void OnLevel3Clicked()
    {
        Debug.Log("Level 3 Clicked!");
        //levelIndex = 2;
        //SceneManager.LoadScene("Level3SceneName", LoadSceneMode.Single);
    }

    public void OnLevel4Clicked()
    {
        Debug.Log("Level 4 Clicked!");
        //levelIndex = 3;
        //SceneManager.LoadScene("Level4SceneName", LoadSceneMode.Single);
    }

    public void OnLevel5Clicked()
    {
        Debug.Log("Level 5 Clicked!");
        //levelIndex = 4;
        //SceneManager.LoadScene("Level5SceneName", LoadSceneMode.Single);
    }

    public void OnLevel6Clicked()
    {
        Debug.Log("Level 6 Clicked!");
        //levelIndex = 5;
        //SceneManager.LoadScene("Level6SceneName", LoadSceneMode.Single);
    }

    public void OnLevel7Clicked()
    {
        Debug.Log("Level 7 Clicked!");
        //levelIndex = 6;
        //SceneManager.LoadScene("Level7SceneName", LoadSceneMode.Single);
    }

    public void OnLevel8Clicked()
    {
        Debug.Log("Level 8 Clicked!");
        //levelIndex = 7;
        //SceneManager.LoadScene("Level8SceneName", LoadSceneMode.Single);
    }

    public void OnLevel9Clicked()
    {
        Debug.Log("Level 9 Clicked!");
        //levelIndex = 8;
        //SceneManager.LoadScene("Level9SceneName", LoadSceneMode.Single);
    }

    public void OnLevel10Clicked()
    {
        Debug.Log("Level 10 Clicked!");
        //levelIndex = 9;
        //SceneManager.LoadScene("Level10SceneName", LoadSceneMode.Single);
    }
}
