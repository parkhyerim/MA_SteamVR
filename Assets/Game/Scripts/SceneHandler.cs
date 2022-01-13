using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Valve.VR.Extras;

public class SceneHandler : MonoBehaviour
{
    public SteamVR_LaserPointer[] laserPointer;
    public MemoryCard mc;
    private GameObject cardObject;

    private void Awake()
    {
        for(int i= 0; i < laserPointer.Length; i++)
        {
            laserPointer[i].PointerIn += PointerInside;
            laserPointer[i].PointerOut += PointerOutside;
            laserPointer[i].PointerClick += PointerClick;
        }

       // mc = GetComponent<MemoryCard>();
    }

    public void PointerClick(object sender, PointerEventArgs e)
    {

        if(e.target.tag == "MemoryCard")
        {
            Debug.Log(e.target.gameObject);
            cardObject = e.target.gameObject;
           mc = cardObject.GetComponent<MemoryCard>();
            FindObjectOfType<MemoryCardGameManager>().CardClicked(mc);
        }
        //if(e.target.name == "Table")
        //{
        //   // Debug.Log("Table was click");
        //}else if(e.target.tag == "MemoryCard")
        //{
        //    Debug.Log("MemoryCard was click");
        //}
    }

    public void PointerInside(object sender, PointerEventArgs e)
    {
        if (e.target.name == "Table")
        {
           // Debug.Log("Table was entered");
        }
        else if (e.target.tag == "MemoryCard")
        {
           // SelectMemoryCard(e.target.gameObject);
            Debug.Log("palceholder was entered");
        }
    }

    public void PointerOutside(object sender, PointerEventArgs e)
    {
        if (e.target.name == "Table")
        {
           // Debug.Log("Table was exited");
        }
        else if (e.target.name == "placeholder")
        {
            Debug.Log("palceholder was entered");
        }
    }

    private void SelectMemoryCard(MemoryCard mc)
    {
        FindObjectOfType<MemoryCardGameManager>().CardClicked(mc);
    }

}
