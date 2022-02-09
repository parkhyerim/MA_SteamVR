using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BystamderAvatar : MonoBehaviour
{
    public GameObject bystanderTracker;
    // public RawImage bystandreImage;
    public RawImage frontImage;
    public RawImage backImage;
    public GameObject bystanderAvatar;
    public GameObject presenceAnimojiBoard;
    public GameObject FovPos;
    public GameObject guidePos;
    public GameObject originalPos;
    public GameObject middlePos;
    
    private float bystanderRotationEulerY;
    private float bystanderRotationOffset = 0;

    private Quaternion bystanderRotation;
    private Quaternion myRotation;
    private Vector3 newRotation;

    public bool sitToLeft;  // Where is the bystander sitting?
    public bool isPresenceSetting;
    public bool isMixedSetting;
    public bool isAvatarSetting;

    public bool isSeated;
    public bool isFov;
    public bool isSeatedAndFov;
    //  public Transform infoBubble;
    // private Text infoText;

    /*
     * awareness information: presence, (location), orientation 
     */

    public float timeToReachTarget;
    public float currentMovementTime = 0f;
    private void Awake()
    {
        // bystanderAvatar.SetActive(true);
        
    }

    // Start is called before the first frame update
    void Start()
    {
        if(!(isPresenceSetting || isMixedSetting || isAvatarSetting))
            isPresenceSetting = true;

        if (isAvatarSetting && !(isSeated || isFov || isSeatedAndFov))
            isSeated = true;

        frontImage.enabled = false;
        backImage.enabled = false;
        bystanderAvatar.SetActive(false);

        //eulerY = bystanderTracker.transform.eulerAngles.y;
        newRotation = new Vector3(0, 0, 0);
      
        //if(infoBubble != null)
        //{
        //    infoText = GetComponentInChildren<Text>();
        //    infoText.text = "Bystander Rotation:";
        //}

        bystanderRotationEulerY = bystanderTracker.transform.eulerAngles.y;
        bystanderRotationOffset = bystanderRotationEulerY - 0f;
    }

    // Update is called once per frame
    void Update()
    {

       // Debug.Log(Camera.main.transform.eulerAngles);
        bystanderRotationEulerY = bystanderTracker.transform.eulerAngles.y;
        
        transform.position = bystanderTracker.transform.position;

        middlePos.transform.position = new Vector3(
            (originalPos.transform.position.x + bystanderTracker.transform.position.x)/2, 
            (originalPos.transform.position.y + bystanderTracker.transform.position.y)/2, 
            (originalPos.transform.position.z + bystanderTracker.transform.position.z)/2);
       // Debug.Log("originalPos: "+ originalPos.transform.position + "   tracker: " + bystanderTracker.transform.position);
       // Debug.Log("transPos: " + transPos.transform.position);
        // The bystander is sitting to the left of the VR Player.
        if (sitToLeft)
        {
            // infoText.text = bystanderRotationEulerY.ToString();

            // The bystander is turning to the right
            /*
             * critical zone: 30-0 degrees to the VR user
            */
            if (bystanderRotationEulerY >= 60 && bystanderRotationEulerY <= 100) 
            {
                if (isPresenceSetting)
                {
                    frontImage.enabled = true;
                    backImage.enabled = false;
                    frontImage.transform.localScale = new Vector2(1.5f, 1.5f);
                   // frontImage.GetComponent<RectTransform>().rect.Set(0, 0, 100, 300);
                }

                if (isAvatarSetting)
                {                
                    if (isFov)
                    {
                        transform.position = new Vector3(FovPos.transform.position.x, bystanderTracker.transform.position.y, FovPos.transform.position.z);
                        // transform.Rotate(0f, bystanderTracker.transform.rotation.y, 0f);
                        //transform.localEulerAngles = new Vector3(0, bystanderRotationEulerY + 60, 0);
                        transform.Rotate(new Vector3(0, bystanderRotationEulerY + 60, 0));
                        Debug.Log("Avatar Y: " + transform.localEulerAngles.y);
                    }

                    if (isSeatedAndFov)
                    {
                        if(Camera.main.transform.eulerAngles.y >= 250 && Camera.main.transform.eulerAngles.y <= 310)
                        {
                            transform.position = bystanderTracker.transform.position;
                        } 
                        else if(Camera.main.transform.eulerAngles.y > 310 && Camera.main.transform.eulerAngles.y <= 315)
                        {
                            bystanderAvatar.SetActive(false);
                        }
                        else
                        {
                            transform.position = new Vector3(FovPos.transform.position.x, bystanderTracker.transform.position.y, FovPos.transform.position.z);
                        }
                    }
        
                    transform.localEulerAngles = new Vector3(0, bystanderRotationEulerY, 0);
                    bystanderAvatar.SetActive(true);
 
                    // fully fade in (1) the image with the duration of 2
                    // bystandreImage.CrossFadeAlpha(1, 1.0f, false);
                    // transform.localEulerAngles = new Vector3(0, bystanderRotationEulerY, 0); // towards the front seat
                    // transform.localEulerAngles = new Vector3(0, 0, 0); // against the front seat
                }

                if (isMixedSetting)
                {
                    Debug.Log("isMixed Critica: " + Camera.main.transform.eulerAngles.y);
                    // presenceAnimojiBoard.transform.position = new Vector3(Camera.main.transform.position.x - 0.4f, presenceAnimojiBoard.transform.position.y - 0.2f, presenceAnimojiBoard.transform.position.z);
                    
                    if (Camera.main.transform.eulerAngles.y >= 320 || (Camera.main.transform.eulerAngles.y > 0 && Camera.main.transform.eulerAngles.y <= 90)) {
                        Debug.Log("camera: 320");
                        bystanderAvatar.SetActive(false);


                        frontImage.transform.localScale = new Vector2(1.5f, 1.5f);
                       // presenceAnimojiBoard.transform.position = middlePos.transform.position;
                        presenceAnimojiBoard.transform.position = guidePos.transform.position;
                        // Debug.Log("Panel:" + presenceAnimojiBoard.transform.position + "  Camera: " + Camera.main.transform.position);
                      
                 
                        currentMovementTime += Time.deltaTime;
                        presenceAnimojiBoard.transform.position = Vector3.Lerp(
                            originalPos.transform.position, 
                            guidePos.transform.position,
                            currentMovementTime/timeToReachTarget);
                        frontImage.enabled = true;
                        backImage.enabled = false;

                    } else if(Camera.main.transform.eulerAngles.y <320 && Camera.main.transform.eulerAngles.y >= 250) {
                        Debug.Log("camera: less 320");
                        bystanderAvatar.SetActive(true);
                        transform.localEulerAngles = new Vector3(0, bystanderRotationEulerY, 0);

                        frontImage.enabled = false;
                        backImage.enabled = false;
                    }

                    // Debug.Log("Camera Rotation: " + Camera.main.transform.rotation.y);
                }


                //if (infoBubble != null)
                //    infoText.text = "Bystander's direnction: " + rotationEulerY;

            }
            else if(bystanderRotationEulerY >= 30 && bystanderRotationEulerY < 60)
            {

                if (isPresenceSetting)
                {
                    backImage.enabled = false;
                    frontImage.enabled = true;
                }

                if (isAvatarSetting)
                {
                    if (isFov) 
                    {
                        transform.position = new Vector3(FovPos.transform.position.x, bystanderTracker.transform.position.y, FovPos.transform.position.z);
                        //transform.localEulerAngles = new Vector3(0, bystanderRotationEulerY + 60, 0);
                        transform.Rotate(new Vector3(0, bystanderRotationEulerY + 60, 0));
                        Debug.Log("Avatar Y: " + transform.localEulerAngles);
                    }

                    //if (isSeatedAndFov) {
                    //    bystanderAvatar.SetActive(true);
                    //    transform.position = bystanderTracker.transform.position;

                    //}


                    transform.localEulerAngles = new Vector3(0, bystanderRotationEulerY, 0);
                    //transform.localEulerAngles = new Vector3(0, 180, 0); // towards the front seat
                    // bystandreImage.CrossFadeAlpha(1, 1.0f, false);
                    bystanderAvatar.SetActive(true);
                }

                if (isMixedSetting)
                {
                    bystanderAvatar.SetActive(false);
                    presenceAnimojiBoard.transform.position = originalPos.transform.position;
                    backImage.enabled = false;
                    frontImage.enabled = true;
                    frontImage.transform.localScale = new Vector2(1f, 1f);
                }
            }
            else if (bystanderRotationEulerY < 30 && bystanderRotationEulerY >= 0)
            {
                if (isPresenceSetting)
                {
                    backImage.enabled = true;
                    frontImage.enabled = false;
                }

                if (isAvatarSetting)
                {
                    if (isFov)
                    {
                        transform.position = new Vector3(FovPos.transform.position.x, bystanderTracker.transform.position.y, FovPos.transform.position.z);
                        //transform.localEulerAngles = new Vector3(0, bystanderRotationEulerY + 60, 0);
                        transform.Rotate(new Vector3(0, bystanderRotationEulerY + 60, 0));
                    }

                    transform.localEulerAngles = new Vector3(0, bystanderRotationEulerY, 0);
                    // bystandreImage.CrossFadeAlpha(0, 1.0f, false);
                    bystanderAvatar.SetActive(true);
                }

                if (isMixedSetting)
                {
                    bystanderAvatar.SetActive(false);
                    presenceAnimojiBoard.transform.position = originalPos.transform.position;
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
                    presenceAnimojiBoard.transform.position = originalPos.transform.position;
                    backImage.enabled = false;
                    frontImage.enabled = false;
                }
            }
        }
        // To the right side of the VR user
        else
        {
            
            // critical zone
            if(bystanderRotationEulerY <= 300 && bystanderRotationEulerY >= 260)
            {
                if (isPresenceSetting)
                {
                    backImage.enabled = false;
                    frontImage.enabled = true;
                    // TODO: image bigger and animations
                }

                if (isAvatarSetting)
                {
                    if (isFov)
                    {
                        transform.position = FovPos.transform.position;
                    }

                    if (isSeatedAndFov)
                    {
                        if (Camera.main.transform.eulerAngles.y >= 60 && Camera.main.transform.eulerAngles.y <= 100)
                        {
                            transform.position = bystanderTracker.transform.position;
                        }
                        else
                        {
                            transform.position = FovPos.transform.position;
                        }
                    }





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
                    backImage.enabled = false;
                    frontImage.enabled = true;
                }

                if (isAvatarSetting)
                {
                    if (isFov || isSeatedAndFov)
                    {
                        transform.position = FovPos.transform.position;
                    }


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
                    backImage.enabled = true;
                    frontImage.enabled = false;
                }

                if (isAvatarSetting)
                {
                    if (isFov)
                    {
                        transform.position = FovPos.transform.position;
                    }

                    transform.localEulerAngles = new Vector3(0, bystanderRotationEulerY, 0);
                    bystanderAvatar.SetActive(true);
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
