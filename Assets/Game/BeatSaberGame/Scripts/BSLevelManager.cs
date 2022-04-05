using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BSLevelManager : MonoBehaviour
{
    [SerializeField]
    float sceneLoadDelay = 2f;
    int currentLevelIndex; // practice 0

    [SerializeField]
    private int[] studyOrder;

    private static BSLevelManager instance;

    BeatSaberGameManager gameManager;
    BSLogManager logManager;
    UserStudyManager userstudyManager;

    private void Awake()
    {
        gameManager = FindObjectOfType<BeatSaberGameManager>();
        logManager = FindObjectOfType<BSLogManager>();
        userstudyManager = FindObjectOfType<UserStudyManager>();

        studyOrder = userstudyManager.GetStudyOrder();
    }
    private void Start()
    {
        if (studyOrder == null || studyOrder.Length == 0 || studyOrder.Length < 3)
        {
            Debug.Log("study order array is empty");
            int[] studyOrder = new int[3];
            for (int i = 0; i < studyOrder.Length; i++)
            {
                studyOrder[i] = i + 1;
                Debug.Log("study order set: " + studyOrder[i]);
            }
        }
        else
        {
            for (int i = 0; i < studyOrder.Length; i++)
            {
                Debug.Log("study order: " + studyOrder[i]);
            }
        }

        currentLevelIndex = SceneManager.GetActiveScene().buildIndex;
        // currentLevelIndex = 0;
        string currentSceneName = SceneManager.GetActiveScene().name;
        //  logManager.WriteToLogFile("Study Order: " + currentLevelIndex + " , name: " + currentSceneName);
    }

    public void LoadNextLevel()
    {
        currentLevelIndex += 1; // 0 -> 1, 1-> 2, 
                                // int nextIndex = userStudyOrder[currentLevelIndex-1];
                                // Debug.Log("next index: " + nextIndex);
                                //  Debug.Log("Level" + currentLevelIndex + "is called");
                                // Debug.Log(newOrder[currentLevelIndex]);
        if (currentLevelIndex <= 6)
            GoToScene(currentLevelIndex);
        else
            GoToScene("EndScene");
    }

    public void GoToScene(string nameScene)
    {
        SceneManager.LoadScene(nameScene);
    }

    public void GoToScene(int indexScene)
    {
        SceneManager.LoadScene(indexScene);
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

    public void QuitGame()
    {
        Debug.Log("Quitting Game...");
        Application.Quit();
    }

    IEnumerator WaitAndLoad(string sceneName, float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneName);
    }
}
