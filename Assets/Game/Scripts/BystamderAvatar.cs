using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BystamderAvatar : MonoBehaviour
{

    public GameObject bystanderTracker;
    public RawImage bystandreImage;
    public bool sitLeft;

    float rotationEulerY;

    float yRotation;
    float xRotation;
    float zRotation;
    Quaternion bystanderRotation;
    Quaternion myRotation;
    Vector3 newRotation;
    Vector3 mirroredRotation;

    public Transform infoBubble;
    private Text infoText;
    
   // public GameObject mirrorAvatar;

    // Start is called before the first frame update
    void Start()
    {
        //eulerY = bystanderTracker.transform.eulerAngles.y;
        newRotation = new Vector3(0, 0, 0);

        
        if(infoBubble != null)
        {
            infoText = GetComponentInChildren<Text>();
            infoText.text = "Bystander direction Info.";
        }
    }

    // Update is called once per frame
    void Update()
    {
        rotationEulerY = bystanderTracker.transform.eulerAngles.y;
        //Debug.Log(eulerY);
        // transform.localEulerAngles = new Vector3(0, -1 * eulerY, 0);

        if (sitLeft)
        {
            if (rotationEulerY > 30 && rotationEulerY < 110)
            {
              //  Debug.Log("The bystander is turning to the right \n at a " + rotationEulerY + "-degree angle.");
                transform.localEulerAngles = new Vector3(0, 0, 0);

                // fully fade in (1) the image with the duration of 2
                bystandreImage.CrossFadeAlpha(1, 1.0f, false);

                //if (infoBubble != null)
                //    infoText.text = "Bystander's direnction: " + rotationEulerY;
            }
            else
            {
               // Debug.Log("The bystander is turning to the left at a " + rotationEulerY + "-degree angle.");
                transform.localEulerAngles = new Vector3(0, 180, 0);
                bystandreImage.CrossFadeAlpha(0, 1.0f, false);
            }
        }
        else
        {
            if(rotationEulerY < 330 && rotationEulerY > 240)
            {
             //   Debug.Log("The bystander is turning to the left \n at a " + rotationEulerY + "-degree angle.");
                transform.localEulerAngles = new Vector3(0, 0, 0);

                // fully fade in (1) the image with the duration of 2
                bystandreImage.CrossFadeAlpha(1, 1.0f, false);

                //if (infoBubble != null)
                //    infoText.text = "Bystander's direnction: " + rotationEulerY;
            }
            else
            {
                //Invoke("TurnBackwards", 3);
               // Debug.Log("The bystander is turning to the right at a " + rotationEulerY + "-degree angle.");
                transform.localEulerAngles = new Vector3(0, 180, 0);
                // fade out to nothing (0) the image with a duration of 2
                bystandreImage.CrossFadeAlpha(0, 1.0f, false);
            }
        }


        if (infoBubble != null)
        {
            infoText.text = "Bystander's direnction: " + rotationEulerY;
            infoBubble.LookAt(Camera.main.transform.position);
            infoBubble.Rotate(0, 180f, 0);
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
