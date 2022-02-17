using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BystanderAvatar : MonoBehaviour
{
    public GameObject bystanderTracker;
    public Transform tracker;
    private float bystanderYAxis;  // bystander's euler y-axis
    private float bystanderRotationOffset = 0;

    public GameObject bystanderAvatar;
    public Animator bystanderAnim;
    public bool doInteraction;

    [Header("GOs for Animoji")]
    // Variables for Animoji Setting
    public GameObject presenceAnimojiBoard;
    public RawImage yesInteractionImage;
    public RawImage noInteractionImage;
    public RawImage backsideImage;

    [Header("GOs for Avatar")]
    // variables for Avatar Setting
    public GameObject FOVPos;
    public GameObject guidePos;
    public GameObject originalPos;
    public GameObject middlePos;
    private Transform guidingPos;

    [Header("Time Settings")]
    public float timeToReachTarget;
    public float currentMovementTime = 0f;

    // Settings
    [Header("User-Study Settings")]
    public bool sitToLeft;  // Where is the bystander sitting?
    public bool isAnimojiSetting;
    public bool isAvatarSetting;
    public bool isMixedSetting;

    [Header("Avatar Sub-Settings")]
    public bool isSeated;
    public bool isInFOV;
    public bool isSeatedAndInFOV;
    // public Transform infoBubble;
    // private Text infoText;

    private float mainCameraYAxis;
    public bool isGuiding;
    public bool isguided;

    private float angleinFOV = 50f;
 
    // Start is called before the first frame update
    void Start()
    { 
        guidingPos = GetComponent<Transform>(); // For Avatar Setting (FOV -> Seated)
        bystanderAnim.SetBool("isInteracting", false);

        // Default setting: avatar setting
        if (!(isAnimojiSetting || isMixedSetting || isAvatarSetting))
            isAvatarSetting = true;
        // Default setting: avatar-FOV 
        if (isAvatarSetting && !(isSeated || isInFOV || isSeatedAndInFOV))
            isInFOV = true;

        yesInteractionImage.enabled = false;
        noInteractionImage.enabled = false;
        backsideImage.enabled = false;
        bystanderAvatar.SetActive(false);

        //if(infoBubble != null)
        //{
        //    infoText = GetComponentInChildren<Text>();
        //    infoText.text = "Bystander Rotation:";
        //}

        // bystanderYAxis = bystanderTracker.transform.eulerAngles.y;
        bystanderYAxis = tracker.eulerAngles.y;
        bystanderRotationOffset = bystanderYAxis - 0f;
        mainCameraYAxis = Camera.main.transform.eulerAngles.y;
    }

    // Update is called once per frame
    void Update()
    {
        //transform.position = bystanderTracker.transform.position;   // sync the avatar's postion with the tracker's position
        transform.position = tracker.position;
      //  bystanderYAxis = bystanderTracker.transform.eulerAngles.y;
        bystanderYAxis = tracker.eulerAngles.y;
        mainCameraYAxis = Camera.main.transform.eulerAngles.y;

        // For animoji guiding
        middlePos.transform.position = new Vector3(
            (originalPos.transform.position.x + tracker.position.x)/2, 
            (originalPos.transform.position.y + tracker.position.y)/2, 
            (originalPos.transform.position.z + tracker.position.z)/2);
        // Debug.Log("originalPos: "+ originalPos.transform.position + "   tracker: " + bystanderTracker.transform.position);

        // For avatar guiding to seated pos
        guidingPos.position = new Vector3(
            (FOVPos.transform.position.x + tracker.position.x) / 2,
            (FOVPos.transform.position.y + tracker.position.y) / 2,
            (FOVPos.transform.position.z + tracker.position.z) / 2);
            
        // The bystander is sitting to the left of the VR Player.
        if (sitToLeft)
        {
            // infoText.text = bystanderRotationEulerY.ToString();

            /*
             * CRITICAL ZONE: 30-0 degrees to the VR user
             * The bystander is heading towards the VR user
             */
            if (bystanderYAxis >= 60 && bystanderYAxis <= 100) // 100 <- 90
            {              
                if (isAnimojiSetting)
                {
                    backsideImage.enabled = false;

                    if (doInteraction)
                    {
                        noInteractionImage.enabled = false;
                        yesInteractionImage.enabled = true;
                        yesInteractionImage.transform.localScale = new Vector2(1.5f, 1.5f);
                    }
                    else
                    {
                        yesInteractionImage.enabled = false;
                        noInteractionImage.enabled = true;
                        noInteractionImage.transform.localScale = new Vector2(1.5f, 1.5f);
                    }
                    
                }
                else if (isAvatarSetting)
                {
                    if (doInteraction)
                        bystanderAnim.SetBool("isInteracting", true);
                    else                    
                        bystanderAnim.SetBool("isInteracting", false);

                    if (isInFOV)
                    {
                       // bystanderAvatar.SetActive(true);
                        transform.position = new Vector3(FOVPos.transform.position.x, bystanderTracker.transform.position.y, FOVPos.transform.position.z);
                       // bystanderAvatar.transform.eulerAngles = new Vector3(0, bystanderYAxis + 50, 0);
                        bystanderAvatar.transform.eulerAngles = new Vector3(0, bystanderYAxis + ((bystanderYAxis*(90+angleinFOV)/90) - bystanderYAxis), 0);
                       // Debug.Log(bystanderYAxis + "<---> " + bystanderAvatar.transform.eulerAngles.y);
                        // Debug.Log("Avatar Y axis: " + + bystanderRotationEulerY + "   " + bystanderAvatar.transform.eulerAngles.y);


                        //if (bystanderYAxis <= 0)
                        //{
                        //    bystanderAvatar.transform.eulerAngles = new Vector3(0, bystanderYAxis, 0);
                        //}
                        //else
                        //{
                        //    bystanderAvatar.transform.eulerAngles = new Vector3(0, ((bystanderYAxis * (140 / 90)) - bystanderYAxis), 0);
                        //}

                        //Debug.Log("Bystander Y Axis:" + (bystanderYAxis * 14 / 9 - 90));

                    }

                    if (isSeatedAndInFOV)
                    {
                        if(mainCameraYAxis >= 250 && mainCameraYAxis <= 310) // VR user is heading towards the bystander
                        {                       
                            transform.position = bystanderTracker.transform.position;
                            bystanderAvatar.transform.eulerAngles = new Vector3(0, bystanderYAxis, 0);
                        } 
                        else if(mainCameraYAxis > 310 && mainCameraYAxis <= 315)
                        {
                            bystanderAvatar.SetActive(false);
                        }
                        else
                        {
                            bystanderAvatar.transform.eulerAngles = new Vector3(0, bystanderYAxis + ((bystanderYAxis * (90 + angleinFOV) / 90) - bystanderYAxis), 0);
                            transform.position = new Vector3(FOVPos.transform.position.x, tracker.position.y, FOVPos.transform.position.z);
                           
                            //if (!isGuiding)
                            //{


                            //    GuideToBystander();
                            //}

                            //if(isGuiding && !isguided)
                            //{
                            //    Invoke("SetGuided", 2f);

                            //    //transform.position = new Vector3(middlePos.transform.position.x, bystanderTracker.transform.position.y, middlePos.transform.position.z);
                            //    //Invoke("GuideToBystander", 1f);
                            //}
                        }
                    }
        
                    transform.localEulerAngles = new Vector3(0, bystanderYAxis, 0);
                    bystanderAvatar.SetActive(true);
 
                    // fully fade in (1) the image with the duration of 2
                    // bystandreImage.CrossFadeAlpha(1, 1.0f, false);
                    // transform.localEulerAngles = new Vector3(0, bystanderRotationEulerY, 0); // towards the front seat
                    // transform.localEulerAngles = new Vector3(0, 0, 0); // against the front seat
                }
                else if (isMixedSetting)
                {
                    if (doInteraction)
                    {
                        bystanderAnim.SetBool("isInteracting", true);
                    }
                    else
                    {
                        bystanderAnim.SetBool("isInteracting", false);
                    }
                    // presenceAnimojiBoard.transform.position = new Vector3(Camera.main.transform.position.x - 0.4f, presenceAnimojiBoard.transform.position.y - 0.2f, presenceAnimojiBoard.transform.position.z);

                    if (mainCameraYAxis >= 320 || (mainCameraYAxis > 0 && mainCameraYAxis <= 90)) {
                        if (isInFOV)
                        {
                            bystanderAvatar.SetActive(true);
                            transform.position = new Vector3(FOVPos.transform.position.x, bystanderTracker.transform.position.y, FOVPos.transform.position.z);
                            // bystanderAvatar.transform.eulerAngles = new Vector3(0, (bystanderYAxis + 50), 0);
                            bystanderAvatar.transform.eulerAngles = new Vector3(0, bystanderYAxis + ((bystanderYAxis * (90 + angleinFOV) / 90) - bystanderYAxis), 0);
                        }
                        else
                        {
                            bystanderAvatar.SetActive(false);
                            
                            // presenceAnimojiBoard.transform.position = middlePos.transform.position;
                            presenceAnimojiBoard.transform.position = guidePos.transform.position;
                            // Debug.Log("Panel:" + presenceAnimojiBoard.transform.position + "  Camera: " + Camera.main.transform.position);


                            currentMovementTime += Time.deltaTime;
                            presenceAnimojiBoard.transform.position = Vector3.Lerp(
                                originalPos.transform.position,
                                guidePos.transform.position,
                                currentMovementTime / timeToReachTarget);
                            
                            backsideImage.enabled = false;
                            if (doInteraction)
                            {
                                noInteractionImage.enabled = false;
                                yesInteractionImage.enabled = true;
                                yesInteractionImage.transform.localScale = new Vector2(1.5f, 1.5f);                          
                            }
                            else
                            {
                                yesInteractionImage.enabled = false;
                                noInteractionImage.enabled = true;
                                noInteractionImage.transform.localScale = new Vector2(1.5f, 1.5f);
                            }                  
                        }                      
                    }
                    else if (mainCameraYAxis < 320 && mainCameraYAxis >= 250)
                    {
                        bystanderAvatar.SetActive(true);
                        if (isInFOV)
                        {
                            transform.position = new Vector3(FOVPos.transform.position.x, bystanderTracker.transform.position.y, FOVPos.transform.position.z);
                            // bystanderAvatar.transform.eulerAngles = new Vector3(0, (bystanderYAxis + 50), 0);
                            bystanderAvatar.transform.eulerAngles = new Vector3(0, bystanderYAxis + ((bystanderYAxis * (90 + angleinFOV) / 90) - bystanderYAxis), 0);
                        }
                        else
                        {
                            transform.position = tracker.position;
                            transform.localEulerAngles = new Vector3(0, bystanderYAxis, 0);
                        }
                        noInteractionImage.enabled = false;
                        yesInteractionImage.enabled = false;
                        backsideImage.enabled = false;
                    }
                }
                //if (infoBubble != null)
                //    infoText.text = "Bystander's direnction: " + rotationEulerY;

            }
            else if(bystanderYAxis >= 30 && bystanderYAxis < 60)
            {
                if (isAnimojiSetting)
                {
                    backsideImage.enabled = false;
                    if (doInteraction)
                    {
                        noInteractionImage.enabled = false;
                        yesInteractionImage.enabled = true;
                        yesInteractionImage.transform.localScale = new Vector2(1f, 1f);
                    }
                    else
                    {
                        yesInteractionImage.enabled = false;
                        noInteractionImage.enabled = true;
                        noInteractionImage.transform.localScale = new Vector2(1f, 1f);
                    }                
                }

                if (isAvatarSetting)
                {
                    bystanderAnim.SetBool("isInteracting", false);
                    if (isInFOV) 
                    {
                        transform.position = new Vector3(FOVPos.transform.position.x, bystanderTracker.transform.position.y, FOVPos.transform.position.z);
                        // bystanderAvatar.transform.eulerAngles = new Vector3(0, (bystanderYAxis + 50), 0);
                        bystanderAvatar.transform.eulerAngles = new Vector3(0, bystanderYAxis + ((bystanderYAxis * (90 + angleinFOV) / 90) - bystanderYAxis), 0);
                    }

                    if (isSeatedAndInFOV)
                    {
                        bystanderAvatar.SetActive(true);
                        transform.position = bystanderTracker.transform.position;

                    }

                  //  transform.position = bystanderTracker.transform.position;
                    transform.localEulerAngles = new Vector3(0, bystanderYAxis, 0);
                    //transform.localEulerAngles = new Vector3(0, 180, 0); // towards the front seat
                    // bystandreImage.CrossFadeAlpha(1, 1.0f, false);
                    bystanderAvatar.SetActive(true);
                }

                if (isMixedSetting)
                {
                    bystanderAvatar.SetActive(false);
                    presenceAnimojiBoard.transform.position = originalPos.transform.position;
                    backsideImage.enabled = false;
                    if (doInteraction)
                    {
                        noInteractionImage.enabled = false;
                        yesInteractionImage.enabled = true;
                        yesInteractionImage.transform.localScale = new Vector2(1f, 1f);
                    }
                    else
                    {
                        yesInteractionImage.enabled = false;
                        noInteractionImage.enabled = true;
                        noInteractionImage.transform.localScale = new Vector2(1f, 1f);
                    }
                   
                    bystanderAnim.SetBool("isInteracting", false);
                }
            }
            else if (bystanderYAxis < 30 && bystanderYAxis >= 0)
            {
                if (isAnimojiSetting)
                {
                    backsideImage.enabled = true;
                    yesInteractionImage.enabled = false;
                    noInteractionImage.enabled = false;
                }

                if (isAvatarSetting)
                {
                    bystanderAnim.SetBool("isInteracting", false);
                    if (isInFOV)
                    {
                        transform.position = new Vector3(FOVPos.transform.position.x, bystanderTracker.transform.position.y, FOVPos.transform.position.z);
                        // bystanderAvatar.transform.eulerAngles = new Vector3(0, (bystanderYAxis + 50), 0);
                        bystanderAvatar.transform.eulerAngles = new Vector3(0, bystanderYAxis + ((bystanderYAxis * (90 + angleinFOV) / 90) - bystanderYAxis), 0);
                    }

                    if (isSeatedAndInFOV)
                    {
                        transform.position = bystanderTracker.transform.position;
                    }

                      
                    transform.localEulerAngles = new Vector3(0, bystanderYAxis, 0);
                    // bystandreImage.CrossFadeAlpha(0, 1.0f, false);
                    bystanderAvatar.SetActive(true);
                }

                if (isMixedSetting)
                {
                    bystanderAvatar.SetActive(false);
                    bystanderAnim.SetBool("isInteracting", false);
                    presenceAnimojiBoard.transform.position = originalPos.transform.position;
                    backsideImage.enabled = true;
                    noInteractionImage.enabled = false;
                    yesInteractionImage.enabled = false;
                }
            }
            else
            {
                if (isAnimojiSetting)
                {
                    backsideImage.enabled = false;
                    yesInteractionImage.enabled = false;
                    noInteractionImage.enabled = false;
                }

                if (isAvatarSetting)
                {
                    bystanderAnim.SetBool("isInteracting", false);
                    bystanderAvatar.SetActive(false);
                }

                if (isMixedSetting)
                {
                    bystanderAvatar.SetActive(false);
                    presenceAnimojiBoard.transform.position = originalPos.transform.position;
                    backsideImage.enabled = false;
                    yesInteractionImage.enabled = false;
                    noInteractionImage.enabled = false;
                    bystanderAnim.SetBool("isInteracting", false);
                }
            }
        }
        /*
         * To the right side of the VR user
         */
        else
        {
            
            // critical zone
            if(bystanderYAxis <= 300 && bystanderYAxis >= 260)
            {
                if (isAnimojiSetting)
                {
                    backsideImage.enabled = false;
                    yesInteractionImage.enabled = true;
                    // TODO: image bigger and animations
                }

                if (isAvatarSetting)
                {
                    if (isInFOV)
                    {
                        transform.position = FOVPos.transform.position;
                    }

                    if (isSeatedAndInFOV)
                    {
                        if (mainCameraYAxis >= 60 && mainCameraYAxis <= 100)
                        {
                            transform.position = bystanderTracker.transform.position;
                        }
                        else
                        {
                            transform.position = FOVPos.transform.position;
                        }
                    }





                    transform.localEulerAngles = new Vector3(0, bystanderYAxis, 0);
                   // Debug.Log(bystanderRotationEulerY);
                    bystanderAvatar.SetActive(true);
                }

                if (isMixedSetting)
                {
                    transform.localEulerAngles = new Vector3(0, bystanderYAxis, 0);
                    bystanderAvatar.SetActive(true);

                    yesInteractionImage.enabled = false;
                    backsideImage.enabled = false;
                }
            }
            // pheriperal zone
            else if(bystanderYAxis <= 330 && bystanderYAxis > 300)
            {
                if (isAnimojiSetting)
                {
                    backsideImage.enabled = false;
                    yesInteractionImage.enabled = true;
                }

                if (isAvatarSetting)
                {
                    if (isInFOV || isSeatedAndInFOV)
                    {
                        transform.position = FOVPos.transform.position;
                    }


                    transform.localEulerAngles = new Vector3(0, bystanderYAxis, 0);
                    bystanderAvatar.SetActive(true);
                }

                if (isMixedSetting)
                {
                    bystanderAvatar.SetActive(false);
                    backsideImage.enabled = false;
                    yesInteractionImage.enabled = true;
                }
            }
            else if(bystanderYAxis <= 360 && bystanderYAxis > 300)
            {
                if (isAnimojiSetting)
                {
                    backsideImage.enabled = true;
                    yesInteractionImage.enabled = false;
                }

                if (isAvatarSetting)
                {
                    if (isInFOV)
                    {
                        transform.position = FOVPos.transform.position;
                    }

                    transform.localEulerAngles = new Vector3(0, bystanderYAxis, 0);
                    bystanderAvatar.SetActive(true);
                }

                if (isMixedSetting)
                {
                    bystanderAvatar.SetActive(false);
                    backsideImage.enabled = true;
                    yesInteractionImage.enabled = false;
                }
            }
            else
            {
                if (isAnimojiSetting)
                {
                    backsideImage.enabled = false;
                    yesInteractionImage.enabled = false;
                }

                if (isAvatarSetting)
                {
                    transform.localEulerAngles = new Vector3(0, bystanderYAxis, 0);
                    bystanderAvatar.SetActive(false);
                }

                if (isMixedSetting)
                {
                    bystanderAvatar.SetActive(false);
                    backsideImage.enabled = false;
                    yesInteractionImage.enabled = false;
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

    private void SetAnimoji(string rotationDegrees)
    {
      
        yesInteractionImage.enabled = true;
        backsideImage.enabled = false;
        yesInteractionImage.transform.localScale = new Vector2(1.5f, 1.5f);
        // frontImage.GetComponent<RectTransform>().rect.Set(0, 0, 100, 300);
    }

    public void GuideToBystander()
    {
        transform.position = new Vector3(FOVPos.transform.position.x, bystanderTracker.transform.position.y, FOVPos.transform.position.z);
        //transform.position = new Vector3(middlePos.transform.position.x, bystanderTracker.transform.position.y, middlePos.transform.position.z);
        isGuiding = true;
        if (isguided)
        {
            Invoke("SetGuided", 2f);
        }
       
    }

    public void SetGuided() {
        isguided = true;
        transform.position = Vector3.Lerp(transform.position, new Vector3(middlePos.transform.position.x, bystanderTracker.transform.position.y, middlePos.transform.position.z), Time.deltaTime * 2);
        Invoke("GuideToBystander", 2f);
    }


}
