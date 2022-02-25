using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    int currentLevelIndex = 0;
    public int[] studyOrder;
    public int animoji_Y;
    public int animoji_N;
    public int avatar_Y;
    public int avatar_N;
    public int mixed_Y;
    public int mixed_N;

    /**
     * index 0 - practice
     * index 1 - aimoji y
     * index 2 - animoji n
     * index 3 - avatar y
     * index 4 - avatar n
     * index 5 - mixed y
     * index 6 - mixed n
     */
    private void Start()
    {
        studyOrder = new int[6];
        
    }

    public void LoadNextLevel()
    {
        currentLevelIndex += 1;
        Debug.Log("Level" + currentLevelIndex + "is called");
        SceneManager.LoadScene(currentLevelIndex);
    }
}
