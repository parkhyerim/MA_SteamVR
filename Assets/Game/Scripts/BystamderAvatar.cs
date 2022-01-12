using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BystamderAvatar : MonoBehaviour
{

    public GameObject bystanderTracker;
    float yRotation;
    float xRotation;
    float zRotation;
    float eulerY;
    Quaternion bystanderRotation;
    Quaternion myRotation;
    Vector3 newRotation;
    Vector3 mirroredRotation;
    
   // public GameObject mirrorAvatar;

    // Start is called before the first frame update
    void Start()
    {
        //eulerY = bystanderTracker.transform.eulerAngles.y;
        newRotation = new Vector3(0, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        eulerY = bystanderTracker.transform.eulerAngles.y;
        //Debug.Log(eulerY);
       // transform.localEulerAngles = new Vector3(0, -1 * eulerY, 0);
        if (eulerY > 30 && eulerY < 90)
        {
            Debug.Log("right side" + "  " + eulerY);
            transform.localEulerAngles = new Vector3(0, 0, 0);
        }
        else if(eulerY <330 && eulerY > 270)
        {
            Debug.Log("left side" + "  " + eulerY);
            transform.localEulerAngles = new Vector3(0, 0, 0);
        }
        else
        {
            Invoke("TurnBackwards", 3);
            
        }


  

    }

    public void TurnBackwards()
    {
         transform.localEulerAngles = new Vector3(0, 180, 0);
        //transform.Rotate(new Vector3(0, 180, 0) * Time.deltaTime);
    }
    Vector3 EulerAngles(Quaternion trs)
    {
        return trs.eulerAngles;
    }


}
