using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{

    int currentLevelIndex = 0;
   public void LoadNextLevel()
    {
        Debug.Log("load next level is called");

        currentLevelIndex += 1;
        SceneManager.LoadScene(currentLevelIndex);

    }
}
