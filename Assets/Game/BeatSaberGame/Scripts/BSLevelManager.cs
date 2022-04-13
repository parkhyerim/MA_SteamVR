using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BSLevelManager : MonoBehaviour
{
    public float sceneLoadDelay = 5f;
    int currentLevelIndex; // practice 0

    [SerializeField]
   // private int[] studyOrder;

    // private static BSLevelManager instance;

    private void Awake()
    {
       // studyOrder = userstudyManager.GetStudyOrder();
    }
    private void Start()
    {
        //if (studyOrder == null || studyOrder.Length == 0 || studyOrder.Length < 3)
        //{
        //    //Debug.Log("study order array is empty");
        //    int[] myStudyOrder = new int[3];    
        //    for (int i = 0; i < myStudyOrder.Length; i++)
        //    {
        //        myStudyOrder[i] = i; // 0,1,2,3
        //    }
        //    studyOrder = myStudyOrder;
        //    // Debug.Log(studyOrder.Length);
        //}

        currentLevelIndex = SceneManager.GetActiveScene().buildIndex;
      //  Debug.Log("current scene index: " + currentLevelIndex);
      //  string currentSceneName = SceneManager.GetActiveScene().name;
       // logManager.WriteLogFile("Condition: " + currentSceneName + ", Study Order: " + (currentLevelIndex));        
    }

    public void LoadNextLevel()
    {
        currentLevelIndex += 1; // 0 -> 1, 1-> 2 ... 

        if (currentLevelIndex < 4) // trial + three conditions = 4 (index -> 0,1,2,3)
            GoToScene(currentLevelIndex);
        else
            //GoToScene("EndScene");
            LoadGameOver();
    }

    //public void GoToScene(string nameScene)
    //{
    //    SceneManager.LoadScene(nameScene);
    //}

    public void GoToScene(int sceneIndex)
    {
       // SceneManager.LoadScene(sceneIndex);
        StartCoroutine(WaitAndLoad(sceneIndex, sceneLoadDelay));
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("");
    }
    public void LoadGameOver()
    {
        // SceneManager.LoadScene("GameOverScene");
        StartCoroutine(WaitAndLoad("GameOver", sceneLoadDelay));
    }

    IEnumerator WaitAndLoad(string sceneName, float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneName);
    }

    IEnumerator WaitAndLoad(int sceneIndex, float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneIndex);
    }

    public void QuitGame()
    {
        Debug.Log("Quitting Game...");
        Application.Quit();
    }
}
