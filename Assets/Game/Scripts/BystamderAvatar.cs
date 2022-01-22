using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BystamderAvatar : MonoBehaviour
{
    public GameObject bystanderTracker;
    public RawImage bystandreImage;
    public RawImage frontImage;
    public RawImage backImage;
    public bool sitToLeft;  // Where is the bystander sitting?
    public bool isPresenceSetting;
    public bool isMixedSetting;
    public bool isAvatarSetting;

    private float bystanderRotationEulerY;
    private float bystanderRotationOffset = 0;

    private Quaternion bystanderRotation;
    private Quaternion myRotation;
    private Vector3 newRotation;

    public Transform infoBubble;
    private Text infoText;

    /*
     * awareness information: presence, (location), orientation 
     */

    private void Awake()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        if(!isPresenceSetting && !isMixedSetting && !isAvatarSetting)
        {
            isPresenceSetting = true;
        }

        frontImage.enabled = false;
        backImage.enabled = false;

        //eulerY = bystanderTracker.transform.eulerAngles.y;
        newRotation = new Vector3(0, 0, 0);
      
        if(infoBubble != null)
        {
            infoText = GetComponentInChildren<Text>();
            infoText.text = "Bystander Rotation:";
        }

        bystanderRotationEulerY = bystanderTracker.transform.eulerAngles.y;
        bystanderRotationOffset = bystanderRotationEulerY - 0f;
        //Debug.Log(rotationEulerY);
        Debug.Log(bystanderRotationOffset);
    }

    // Update is called once per frame
    void Update()
    {
        bystanderRotationEulerY = bystanderTracker.transform.eulerAngles.y;
        // transform.localEulerAngles = new Vector3(0, -1 * eulerY, 0);
        transform.position = bystanderTracker.transform.position;

        // The bystander is sitting to the left of the VR Player.
        if (sitToLeft)
        {
            // The bystander is turning to the right
            if (bystanderRotationEulerY >= 30 && bystanderRotationEulerY < 110)
            {
                infoText.text = "The bystander is turning to the right \n at a " + bystanderRotationEulerY + "-degree angle.";


                Debug.Log(bystanderRotationEulerY);

                frontImage.enabled = true;
                backImage.enabled = false;

                //if (infoBubble != null)
                //    infoText.text = "Bystander's direnction: " + rotationEulerY;
                transform.localEulerAngles = new Vector3(0, 0, 0);
                // fully fade in (1) the image with the duration of 2
                //  bystandreImage.CrossFadeAlpha(1, 1.0f, false);
            }
            else if(bystanderRotationEulerY >= 260f && bystanderRotationEulerY < 360f)
            {
                infoText.text = "The bystander is turning to the left at a " + bystanderRotationEulerY + "-degree angle.";

                backImage.enabled = true;
                frontImage.enabled = false;

                transform.localEulerAngles = new Vector3(0, 180, 0);
                //   bystandreImage.CrossFadeAlpha(0, 1.0f, false);

            }
            else 
            {
                infoText.text = "The bystander is turning to the left at a " + bystanderRotationEulerY + "-degree angle.";
                backImage.enabled = false;
                frontImage.enabled = false;
            }
        }
        else
        {
            if(bystanderRotationEulerY < 330 && bystanderRotationEulerY > 240)
            {
             //   Debug.Log("The bystander is turning to the left \n at a " + rotationEulerY + "-degree angle.");
                transform.localEulerAngles = new Vector3(0, 0, 0);

                // fully fade in (1) the image with the duration of 2
               // bystandreImage.CrossFadeAlpha(1, 1.0f, false);
                backImage.gameObject.SetActive(false);
                frontImage.gameObject.SetActive(true);
                //if (infoBubble != null)
                //    infoText.text = "Bystander's direnction: " + rotationEulerY;
            }
            else
            {
                //Invoke("TurnBackwards", 3);
               // Debug.Log("The bystander is turning to the right at a " + rotationEulerY + "-degree angle.");
                transform.localEulerAngles = new Vector3(0, 180, 0);
                // fade out to nothing (0) the image with a duration of 2
              //  bystandreImage.CrossFadeAlpha(0, 1.0f, false);
                backImage.gameObject.SetActive(true);
                frontImage.gameObject.SetActive(false);
            }
        }


        if (infoBubble != null)
        {
            infoText.text = "Bystander's direnction: " + bystanderRotationEulerY;
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
