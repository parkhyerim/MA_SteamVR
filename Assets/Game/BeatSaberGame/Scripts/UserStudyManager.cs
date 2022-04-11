using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserStudyManager : MonoBehaviour
{
    static UserStudyManager instance;

    public string participantID;
    public int[] studyOrder = new int[3] { 1, 2, 3 }; // default order

    private void Awake()
    {
        ManageSingleton();
    }

    void ManageSingleton()
    {
        if (instance != null)
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public string GetID()
    {
        return participantID;
    }

    public int[] GetStudyOrder()
    {
        return studyOrder;
    }
}
