using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MCGameManager : MonoBehaviour
{
    public MemoryCube firstSelectedCube;
    public MemoryCube secondSelectedCube;
    public Placeholder[] placeholderList;
    public int[] idList = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

    private void Awake()
    {
        System.Random ran = new System.Random();

        // Debug.Log(idList[3]);
        for (int i = 0; i < placeholderList.Length; i++)
        {
           // int id = idList[i];
           // placeholderList[i].identifier = id;
        }
    }

    private void Update()
    {
        if (Input.GetButtonDown("XRI_Right_TriggerButton"))
        {
           Debug.Log("Trigger Down");
        }
        else
        {
           //Debug.Log("Trigger Up");
        }
    }
    public void CubeClicked(MemoryCube cube)
    {
        if(firstSelectedCube == null)
        {
            firstSelectedCube = cube;
            Debug.Log("mouse click");
        }
        else
        {
            // Second cube selected
            secondSelectedCube = cube;

            // Result
            if (firstSelectedCube.identifier == secondSelectedCube.identifier)
            {
                
                Destroy(firstSelectedCube.gameObject);
                Destroy(secondSelectedCube.gameObject);
                //Destroy(secondSelectedCube);
            }

            // Reset
            firstSelectedCube = null;
            secondSelectedCube = null;
        }     
    }


}
