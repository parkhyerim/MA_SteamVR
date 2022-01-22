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
    public GameObject bystanderAvatar;

    private float bystanderRotationEulerY;
    private float bystanderRotationOffset = 0;

    private Quaternion bystanderRotation;
    private Quaternion myRotation;
    private Vector3 newRotation;

  //  public Transform infoBubble;
   // private Text infoText;

    /*
     * awareness information: presence, (location), orientation 
     */

    private void Awake()
    {
       // bystanderAvatar.SetActive(true);
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

        bystanderAvatar.SetActive(false);
      
        //if(infoBubble != null)
        //{
        //    infoText = GetComponentInChildren<Text>();
        //    infoText.text = "Bystander Rotation:";
        //}

        bystanderRotationEulerY = bystanderTracker.transform.eulerAngles.y;
        bystanderRotationOffset = bystanderRotationEulerY - 0f;
        //Debug.Log(rotationEulerY);
        //Debug.Log(bystanderRotationOffset);
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
           // infoText.text = bystanderRotationEulerY.ToString();

            // The bystander is turning to the right
            // critical zone
            if (bystanderRotationEulerY >= 60 && bystanderRotationEulerY < 110)
            {
                //  Debug.Log(bystanderRotationEulerY);

                if (isPresenceSetting)
                {
                    frontImage.enabled = true;
                    backImage.enabled = false;
                }

                if (isAvatarSetting)
                {
                    transform.localEulerAngles = new Vector3(0, bystanderRotationEulerY, 0);
                    bystanderAvatar.SetActive(true);
                    

                    // fully fade in (1) the image with the duration of 2
                    // bystandreImage.CrossFadeAlpha(1, 1.0f, false);
                    // transform.localEulerAngles = new Vector3(0, bystanderRotationEulerY, 0); // towards the front seat
                    // transform.localEulerAngles = new Vector3(0, 0, 0); // against the front seat

                }

                if (isMixedSetting)
                {
                    transform.localEulerAngles = new Vector3(0, bystanderRotationEulerY, 0);
                    bystanderAvatar.SetActive(true);

                    frontImage.enabled = false;
                    backImage.enabled = false;
                }


                //if (infoBubble != null)
                //    infoText.text = "Bystander's direnction: " + rotationEulerY;

            }
            else if(bystanderRotationEulerY >= 30 && bystanderRotationEulerY < 60)
            {

                if (isPresenceSetting)
                {
                    backImage.enabled = true;
                    frontImage.enabled = false;
                }

                if (isAvatarSetting)
                {
                    transform.localEulerAngles = new Vector3(0, bystanderRotationEulerY, 0);
                    //transform.localEulerAngles = new Vector3(0, 180, 0); // towards the front seat
                    // bystandreImage.CrossFadeAlpha(1, 1.0f, false);
                    bystanderAvatar.SetActive(true);
                }

                if (isMixedSetting)
                {
                    bystanderAvatar.SetActive(false);
                    backImage.enabled = false;
                    frontImage.enabled = true;
                }
            }
            else if (bystanderRotationEulerY < 30 && bystanderRotationEulerY >= 0)
            {
                if (isPresenceSetting)
                {
                    backImage.enabled = false;
                    frontImage.enabled = false;
                }

                if (isAvatarSetting)
                {
                    transform.localEulerAngles = new Vector3(0, bystanderRotationEulerY, 0);
                    // bystandreImage.CrossFadeAlpha(0, 1.0f, false);
                    bystanderAvatar.SetActive(false);
                }

                if (isMixedSetting)
                {
                    bystanderAvatar.SetActive(false);
                    backImage.enabled = true;
                    frontImage.enabled = false;
                }
            }
            else
            {
                if (isPresenceSetting)
                {
                    backImage.enabled = false;
                    frontImage.enabled = false;
                }

                if (isAvatarSetting)
                {
                    bystanderAvatar.SetActive(false);
                }

                if (isMixedSetting)
                {
                    bystanderAvatar.SetActive(false);
                    backImage.enabled = false;
                    frontImage.enabled = false;
                }
            }
        }
        // To the right side of the VR user
        else
        {
            
            // critical zone
            if(bystanderRotationEulerY <= 300 && bystanderRotationEulerY > 250)
            {
                if (isPresenceSetting)
                {
                    backImage.enabled = false;
                    frontImage.enabled = true;
                }

                if (isAvatarSetting)
                {
                    transform.localEulerAngles = new Vector3(0, bystanderRotationEulerY, 0);
                   // Debug.Log(bystanderRotationEulerY);
                    bystanderAvatar.SetActive(true);
                }

                if (isMixedSetting)
                {
                    transform.localEulerAngles = new Vector3(0, bystanderRotationEulerY, 0);
                    bystanderAvatar.SetActive(true);

                    frontImage.enabled = false;
                    backImage.enabled = false;
                }
            }
            // pheriperal zone
            else if(bystanderRotationEulerY <= 330 && bystanderRotationEulerY > 300)
            {
                if (isPresenceSetting)
                {
                    backImage.enabled = true;
                    frontImage.enabled = false;
                }

                if (isAvatarSetting)
                {
                    transform.localEulerAngles = new Vector3(0, bystanderRotationEulerY, 0);
                    bystanderAvatar.SetActive(true);
                }

                if (isMixedSetting)
                {
                    bystanderAvatar.SetActive(false);
                    backImage.enabled = false;
                    frontImage.enabled = true;
                }
            }
            else if(bystanderRotationEulerY <= 360 && bystanderRotationEulerY > 300)
            {
                if (isPresenceSetting)
                {
                    backImage.enabled = false;
                    frontImage.enabled = false;
                }

                if (isAvatarSetting)
                {
                    transform.localEulerAngles = new Vector3(0, bystanderRotationEulerY, 0);
                    bystanderAvatar.SetActive(false);
                }

                if (isMixedSetting)
                {
                    bystanderAvatar.SetActive(false);
                    backImage.enabled = true;
                    frontImage.enabled = false;
                }
            }
            else
            {
                if (isPresenceSetting)
                {
                    backImage.enabled = false;
                    frontImage.enabled = false;
                }

                if (isAvatarSetting)
                {
                    transform.localEulerAngles = new Vector3(0, bystanderRotationEulerY, 0);
                    bystanderAvatar.SetActive(false);
                }

                if (isMixedSetting)
                {
                    bystanderAvatar.SetActive(false);
                    backImage.enabled = false;
                    frontImage.enabled = false;
                }
            }
        }


        //if (infoBubble != null)
        //{
        //   // infoText.text = bystanderRotationEulerY.ToString();
        //    infoBubble.LookAt(Camera.main.transform.position);
        //    infoBubble.Rotate(0, 180f, 0);
        //}
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
