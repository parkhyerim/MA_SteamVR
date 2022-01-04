using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemoryCube : MonoBehaviour
{
    public GameObject pos;

    public int identifier;

    private void Awake()
    {
        transform.position = pos.transform.position;
    }

    private void Update()
    {
       // if (Input.GetButtonDown("Fire1"))
          
        
    }

    public void OnMouseDown()
    {
        Debug.Log("clicked");
        FindObjectOfType<MCGameManager>().CubeClicked(this);
    }
}
