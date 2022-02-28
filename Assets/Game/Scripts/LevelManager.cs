using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    int currentLevelIndex;
    
   // public int[] studyOrder = new int[] { 1, 3, 5, 4, 2, 6 };
    //public int animoji_Y;
    //public int animoji_N;
    //public int avatar_Y;
    //public int avatar_N;
    //public int mixed_Y;
    //public int mixed_N;
   // private int[] newOrder;
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
        currentLevelIndex = SceneManager.GetActiveScene().buildIndex;
        Debug.Log(currentLevelIndex);
       // newOrder = new int[6];
        
       //for(int i = 0; i <studyOrder.Length; i++ )
       // {
       //     newOrder[i] = studyOrder[i];
       // }
    }

    public void LoadNextLevel()
    {
        currentLevelIndex += 1;
        Debug.Log("Level" + currentLevelIndex + "is called");
       // Debug.Log(newOrder[currentLevelIndex]);
       if(currentLevelIndex <=6)
            SceneManager.LoadScene(currentLevelIndex);
    }
}
