using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BSBystanderAvatar : MonoBehaviour
{
    public GameObject bystanderTracker;
    private Transform trackerTrans;
    public BeatSaberGameManager bsgameManager;
    public BSLogManager logManager;
    private float bystanderEulerYAxis;  // bystander's euler y-axis
    public GameObject bystanderAvatar;
    public Animator bystanderAnim;
    [SerializeField]
    private bool doInteraction = true;

    [Header("GOs for Animoji")]
    // Variables for Animoji Setting
    public GameObject presenceAnimojiBoard;
    public RawImage yesInteractionFrontImage, noInteractionFrontImage, backsideImage;
    public RawImage arrowImage;
    public GameObject arrowPosition;
    public GameObject originalArrowPos;

    [Header("GOs for Avatar")]
    // variables for Avatar Setting
    public GameObject FOVPos;
    public GameObject guidePos;
    public GameObject originalPos;
    public GameObject middlePos;
    private Transform guidingPos;
    public GameObject arrowPos;
    public GameObject arrowPosForAvatar;
    public GameObject arrowOriginalPosForAvatar;
    public GameObject guidingPosForAV;

    [Header("GOs for Mixed")]
    private float timeElapsedForMixed;

    [Header("Time Settings")]
    public float timeToReachTarget;
    public float currentMovementTime = 0f;

    [Header("User-Study Settings")]
    public bool sitToLeft;  // Where is the bystander sitting?
    public bool isAnimojiSetting, isAvatarSetting, isMixedSetting;

    [Header("Avatar Sub-Settings")]
   // public bool isSeatedAndInFOV;

    private float mainCameraYAxis;
    public bool isGuidingFOVToSeated;
    public bool isguided;

    private float angleinFOV = 50f;

    private float guidingLength;
    private float guidingSpeed = 1.0f;
    private float timeElapsedForSEATToFOV, timeElapsedForFOVToSEAT;
    public float lerpGuideTime = 3f;
    public float fadeTime = 2f;
    float timeElapsedForAnimoji = 0f;

    [SerializeField]
    private bool inCriticalZone, inTransitionZone, inUncriticalZone, inNoZone;
    private bool fromCriticalSection;
    private bool lookedOnceSeatedPosition;
    private Color animojiBacksideColor;
    Color noTransparency, lowTransparency;

    // Start is called before the first frame update
    void Start()
    {
        doInteraction = true;
        sitToLeft = true;
        guidingPos = GetComponent<Transform>(); // For Avatar Setting (FOV -> Seated)
        bystanderAnim.SetBool("isInteracting", false);

        // Default setting: Avatar setting
        if (!(isAnimojiSetting || isMixedSetting || isAvatarSetting) && !bsgameManager.isPracticeGame)
            isAvatarSetting = true;

        yesInteractionFrontImage.enabled = false;
        noInteractionFrontImage.enabled = false;
        backsideImage.enabled = false;
        bystanderAvatar.SetActive(false);
        arrowImage.enabled = false;

        // bystanderYAxis = bystanderTracker.transform.eulerAngles.y;
        trackerTrans = bystanderTracker.transform;
        bystanderEulerYAxis = trackerTrans.eulerAngles.y;
        // bystanderRotationOffset = bystanderEulerYAxis - 0f;
        mainCameraYAxis = Camera.main.transform.eulerAngles.y;

        guidingLength = Vector3.Distance(new Vector3(FOVPos.transform.position.x, bystanderTracker.transform.position.y, FOVPos.transform.position.z), trackerTrans.position);

        noTransparency = backsideImage.color;
        lowTransparency = backsideImage.color;
        noTransparency.a = 1f;
        lowTransparency.a = 0.05f;
    }

    void Update()
    {
        transform.position = trackerTrans.position;
        bystanderEulerYAxis = trackerTrans.eulerAngles.y;
        mainCameraYAxis = Camera.main.transform.eulerAngles.y;

        // For animoji? Avatar? guiding
        middlePos.transform.position = new Vector3(
            (originalPos.transform.position.x + trackerTrans.position.x) / 2,
            (originalPos.transform.position.y + trackerTrans.position.y) / 2,
            (originalPos.transform.position.z + trackerTrans.position.z) / 2);

        // For avatar guiding to seated pos
        guidingPos.position = new Vector3(
            (FOVPos.transform.position.x + trackerTrans.position.x) / 2,
            (FOVPos.transform.position.y + trackerTrans.position.y) / 2,
            (FOVPos.transform.position.z + trackerTrans.position.z) / 2);

        // The bystander is sitting to the left of the VR Player.
        if (sitToLeft)
        {
            if (isAnimojiSetting)
            {
                //  CRITICAL ZONE: 30 >= [Bystander's degrees] > 0 to the VR user
                if (bystanderEulerYAxis >= 60 && bystanderEulerYAxis < 100)
                {
                    if (!inCriticalZone) // From Shift Zone (60-30 degrees)
                    {
                        BystanderShiftZone("Enter_CZ");
                        inCriticalZone = true;
                    }

                    if (doInteraction)  // Bigger Animoji with FE
                    {
                        backsideImage.enabled = false;
                        noInteractionFrontImage.enabled = false;
                        yesInteractionFrontImage.enabled = true;
                        yesInteractionFrontImage.transform.localScale = new Vector2(1.5f, 1.5f);
                    }
                    else  // Bigger Animoji without FE 
                    {                     
                        //backsideImage.enabled = false;
                        //yesInteractionFrontImage.enabled = false;
                        //noInteractionFrontImage.enabled = true;
                        //noInteractionFrontImage.transform.localScale = new Vector2(1.5f, 1.5f);
                    }
                }
                // TRANSITION ZONE: 60 >= [Bystander's degrees] > 30
                else if (bystanderEulerYAxis >= 30 && bystanderEulerYAxis < 60)
                {
                    inTransitionZone = true;
                    Debug.Log("Animoji_Transition Zone");
                    timeElapsedForAnimoji = 0;
                    if (inCriticalZone) // From Critical Zone
                    {
                        BystanderShiftZone("From_CZ_to_TZ");
                        backsideImage.enabled = true;
                        yesInteractionFrontImage.enabled = false;
                        noInteractionFrontImage.enabled = false;
                        inCriticalZone = false;
                    }
                    if(inNoZone) // From Uncritical Zone
                    {
                        BystanderShiftZone("From_UZ_to_TZ");
                        backsideImage.enabled = false;
                        yesInteractionFrontImage.enabled = false;
                        noInteractionFrontImage.enabled = true;
                        noInteractionFrontImage.transform.localScale = new Vector2(1f, 1f);
                        inNoZone = false;
                    }
                }
                // UNCRITICAL ZONE: 80 >= Bystander's degrees > 60
                else if (bystanderEulerYAxis < 30 && bystanderEulerYAxis >= 5)
                {
                    Debug.Log("Animoji_UC Zone");
                    inUncriticalZone = true;

                    yesInteractionFrontImage.enabled = false;
                    noInteractionFrontImage.enabled = false;

                    if (inNoZone && !inTransitionZone) // From No-Zone
                    {
                        timeElapsedForAnimoji += Time.deltaTime;

                        if (timeElapsedForAnimoji < fadeTime)
                        {
                          //  Debug.Log("timeForAnimoji (fade in): " + timeElapsedForAnimoji);
                            float t = timeElapsedForAnimoji / fadeTime;
                            t = t * t * (3f - 2f * t);
                            backsideImage.enabled = true;
                            backsideImage.color = Color.Lerp(lowTransparency, noTransparency, t);
                        }
                        else
                        {
                            backsideImage.color = noTransparency;
                            inNoZone = false;
                          //  Debug.Log("Transparency is full and NoZone is set " + inNoZone);
                        }
                    }

                    if (inTransitionZone && !inNoZone) // From Shift Zone
                    {
                        timeElapsedForAnimoji += Time.deltaTime;

                        if (timeElapsedForAnimoji < fadeTime)
                        {
                            //Debug.Log("timeForAnimoji (fade out): " + timeElapsedForAnimoji);
                            float t = timeElapsedForAnimoji / fadeTime;
                            t = t * t * (3f - 2f * t);
                            backsideImage.enabled = true;
                            backsideImage.color = Color.Lerp(noTransparency, lowTransparency, t);
                        }
                        else
                        {
                            backsideImage.color = lowTransparency;
                            inTransitionZone = false;
                           // Debug.Log("Transparency is 0 and Shift-Zone is set " + inTransitionZone);
                        }
                    }                
                }
               // NO ZONE:  Bystander's degrees > 80/85
                else 
                {
                    Debug.Log("Animoji_No Zone");
                    // No Visualisation
                    inNoZone = true;
                    inCriticalZone = false;
                    inTransitionZone = false;
                    inUncriticalZone = false;
                    backsideImage.enabled = false;
                    yesInteractionFrontImage.enabled = false;
                    noInteractionFrontImage.enabled = false;
                }
            }
            /*****************************
             *** AVATAR SETTING
             ****************************/
            if (isAvatarSetting)
            {
                //  CRITICAL ZONE: 30-0 degrees to the VR user
                if (bystanderEulerYAxis >= 60 && bystanderEulerYAxis < 100)
                {
                    if (!inCriticalZone)
                    {
                        BystanderShiftZone("Enter_CZ");// enter 30 degrees
                        inCriticalZone = true;
                    }

                    if (doInteraction)
                        bystanderAnim.SetBool("isInteracting", true);
                    else
                        bystanderAnim.SetBool("isInteracting", false);
                
                    // The bystander is inside VR user's FOV
                    if (mainCameraYAxis >= 250 && mainCameraYAxis <= 310)
                    {
                        Debug.Log("VRUser looks at the seated position");
                        // Avatar in seated position without manupulating angles
                        transform.position = bystanderTracker.transform.position;
                        bystanderAvatar.transform.eulerAngles = new Vector3(0, bystanderEulerYAxis, 0);
                        arrowImage.enabled = false;
                        lookedOnceSeatedPosition = true;
                       // isGuidingToSeated = true;
                        //currentMovementTime = 0;
                        //timeElapsedForAvatarGuide = 0;
                    }
                    //else if(mainCameraYAxis > 310 && mainCameraYAxis <= 315) 
                    //{
                    //    bystanderAvatar.SetActive(false);
                    //    arrowImage.enabled = false;
                    //}
                    else  // The bystander is outside the FOV of the VR user ( 310 < d < 360, ....) 
                          // /-> focusing on/looking at the game
                    {
                        currentMovementTime += Time.deltaTime;

                        if (!lookedOnceSeatedPosition) // The VR user haven't look at the seated avatar
                        { 
                            Debug.Log("The VR user haven't look at the seated avatar");
                            if (!isGuidingFOVToSeated)
                            {
                                // bystanderAvatar.transform.eulerAngles = new Vector3(0, bystanderEulerYAxis + ((bystanderEulerYAxis * (90 + angleinFOV) / 90) - bystanderEulerYAxis), 0);
                                // transform.position = new Vector3(FOVPos.transform.position.x, trackerTrans.position.y, FOVPos.transform.position.z);

                                timeElapsedForSEATToFOV += Time.deltaTime;
                                if (timeElapsedForSEATToFOV < lerpGuideTime) // lerpGuideTime:2
                                {
                                    Debug.Log("[NOT look at seated]:time elapsed: " + timeElapsedForSEATToFOV);
                                    float t = timeElapsedForSEATToFOV / lerpGuideTime;
                                    t = t * t * (3f - 2f * t);
                                    transform.position = Vector3.Lerp(
                                            new Vector3(trackerTrans.position.x, trackerTrans.position.y, trackerTrans.position.z),
                                            new Vector3(FOVPos.transform.position.x, bystanderTracker.transform.position.y, FOVPos.transform.position.z),
                                                t);

                                    //  new Vector3(guidingPosForAV.transform.position.x, tracker.position.y, guidingPosForAV.transform.position.z)

                                    bystanderAvatar.transform.rotation = Quaternion.Lerp(
                                        Quaternion.Euler(new Vector3(0, bystanderEulerYAxis + ((bystanderEulerYAxis * (90 + angleinFOV - 10) / 90) - bystanderEulerYAxis), 0)),
                                        Quaternion.Euler(new Vector3(0, bystanderEulerYAxis + ((bystanderEulerYAxis * (90 + angleinFOV) / 90) - bystanderEulerYAxis), 0)),
                                            t);
                                }
                                else
                                {
                                    Debug.Log("guide time passed: " + timeElapsedForSEATToFOV);
                                    // transform.position = new Vector3(guidingPosForAV.transform.position.x, trackerTrans.position.y, guidingPosForAV.transform.position.z);

                                    transform.position = new Vector3(FOVPos.transform.position.x, bystanderTracker.transform.position.y, FOVPos.transform.position.z);
                                    // arrow
                                }
                            }
                            // more than the expected guiding time
                            // -> intensively guide to the seated positionZZZZ
                            else
                            {
                                arrowImage.enabled = true;
                                arrowImage.transform.position = arrowPos.transform.position;

                                timeElapsedForFOVToSEAT += Time.deltaTime;

                                if (timeElapsedForFOVToSEAT < lerpGuideTime)
                                {
                                    Debug.Log("timeElapsedForGuiding: " + timeElapsedForFOVToSEAT);
                                    float t = timeElapsedForFOVToSEAT / lerpGuideTime;
                                    t = t * t * (3f - 2f * t);
                                    transform.position = Vector3.Lerp(
                                                new Vector3(FOVPos.transform.position.x, bystanderTracker.transform.position.y, FOVPos.transform.position.z),
                                                new Vector3(guidingPosForAV.transform.position.x, trackerTrans.position.y, guidingPosForAV.transform.position.z),
                                                t);

                                    //  new Vector3(guidingPosForAV.transform.position.x, tracker.position.y, guidingPosForAV.transform.position.z)

                                    bystanderAvatar.transform.rotation = Quaternion.Lerp(
                                        Quaternion.Euler(new Vector3(0, bystanderEulerYAxis + ((bystanderEulerYAxis * (90 + angleinFOV) / 90) - bystanderEulerYAxis), 0)),
                                        Quaternion.Euler(new Vector3(0, bystanderEulerYAxis + ((bystanderEulerYAxis * (90 + angleinFOV - 10) / 90) - bystanderEulerYAxis), 0)),
                                            t);

                                    arrowImage.transform.position = Vector3.Lerp(
                                   arrowPos.transform.position,
                                   new Vector3(guidingPosForAV.transform.position.x, arrowPos.transform.position.y, guidingPosForAV.transform.position.z),
                                   t);
                                }
                                else
                                {
                                    Debug.Log("stop: " + timeElapsedForFOVToSEAT);
                                    //transform.position = new Vector3(guidingPosForAV.transform.position.x, trackerTrans.position.y, guidingPosForAV.transform.position.z);
                                    // new Vector3(guidingPosForAV.transform.position.x, arrowPos.transform.position.y, guidingPosForAV.transform.position.z);
                                   // transform.position = new Vector3(0, bystanderEulerYAxis + ((bystanderEulerYAxis * (90 + angleinFOV - 10) / 90) - bystanderEulerYAxis), 0);
                                }
                            }

                            if (currentMovementTime > 3f)
                            {
                                Debug.Log("currentmove: " + currentMovementTime);
                                isGuidingFOVToSeated = true;
                                // timeElapsedForAvatarGuide = 0;
                            }
                        }
                        else
                        {
                            Debug.Log("already guided: " + lookedOnceSeatedPosition);
                            transform.position = bystanderTracker.transform.position;
                            bystanderAvatar.transform.eulerAngles = new Vector3(0, bystanderEulerYAxis, 0);
                        }                                        
                    }

                 //   transform.localEulerAngles = new Vector3(0, bystanderEulerYAxis, 0);
                    bystanderAvatar.SetActive(true);
                }
                // TRANSITION ZONE: 60 >= [Bystander's degrees] > 30
                else if (bystanderEulerYAxis >= 30 && bystanderEulerYAxis < 60)
                {
                    currentMovementTime = 0f;
                    timeElapsedForSEATToFOV = 0f;
                    inTransitionZone = true;
                    if (inCriticalZone)
                    {
                        bystanderAnim.SetBool("isInteracting", false);
                        //  transform.position = bystanderTracker.transform.position;
                        transform.localEulerAngles = new Vector3(0, 30, 0);
                        //transform.localEulerAngles = new Vector3(0, 180, 0); // towards the front seat
                        // bystandreImage.CrossFadeAlpha(1, 1.0f, false);
                        bystanderAvatar.SetActive(true);

                       
                        transform.position = bystanderTracker.transform.position;
                        bystanderAvatar.transform.eulerAngles = new Vector3(0, 0, 0);
                       
                        inCriticalZone = false;
                    }
                    else
                    {
                        bystanderAnim.SetBool("isInteracting", false);
                        //  transform.position = bystanderTracker.transform.position;
                        transform.localEulerAngles = new Vector3(0, bystanderEulerYAxis, 0);
                        //transform.localEulerAngles = new Vector3(0, 180, 0); // towards the front seat
                        // bystandreImage.CrossFadeAlpha(1, 1.0f, false);
                        bystanderAvatar.SetActive(true);

                      
                        transform.position = bystanderTracker.transform.position;
                        // +
                        bystanderAvatar.transform.eulerAngles = new Vector3(0, bystanderEulerYAxis, 0);
                        arrowImage.enabled = false;                      
                    }
                }
                // UNCRITICAL ZONE: 80/85 >= Bystander's degrees > 60
                else if (bystanderEulerYAxis < 30 && bystanderEulerYAxis >= 5)
                {
                    inUncriticalZone = true;
                    bystanderAvatar.SetActive(true);
                    bystanderAnim.SetBool("isInteracting", false);
                    transform.localEulerAngles = new Vector3(0, bystanderEulerYAxis, 0);

                    //if (isInFOV)
                    //{
                    //    transform.position = new Vector3(FOVPos.transform.position.x, bystanderTracker.transform.position.y, FOVPos.transform.position.z);
                    //    // bystanderAvatar.transform.eulerAngles = new Vector3(0, (bystanderYAxis + 50), 0);
                    //    bystanderAvatar.transform.eulerAngles = new Vector3(0, bystanderEulerYAxis + ((bystanderEulerYAxis * (90 + angleinFOV) / 90) - bystanderEulerYAxis), 0);
                    //}

                  
                    // Avatar's position = bystander's seating position
                    transform.position = bystanderTracker.transform.position;
                    arrowImage.enabled = false;
                    
                }
                // NO ZONE:  Bystander's degrees > 80/85
                else
                {
                    inNoZone = true;
                    inUncriticalZone = false;
                    inTransitionZone = false;
                    inCriticalZone = false;
                    
                    // TODO: Is the avatar shown when the bystander is at an angle greater than 90 degrees towards the VR user?
                    // If No -> no Avatar
                    bystanderAnim.SetBool("isInteracting", false);
                    bystanderAvatar.SetActive(false);

                    // If yes
                    //bystanderAnim.SetBool("isInteracting", false);
                    //bystanderAvatar.SetActive(true);
                }
            }
            /*****************************
             *** MIXED SETTING
             ****************************/
            if (isMixedSetting)
            {
                //  CRITICAL ZONE: 30-0 degrees to the VR user
                if (bystanderEulerYAxis >= 60 && bystanderEulerYAxis < 100) // 100 <- 90
                {
                    if (!inCriticalZone)
                    {
                        BystanderShiftZone("Enter CZ");
                        inCriticalZone = true;
                    }

                    // Animation for the 3d-avatar
                    if (doInteraction)
                        bystanderAnim.SetBool("isInteracting", true);
                    else
                        bystanderAnim.SetBool("isInteracting", false);

                    // presenceAnimojiBoard.transform.position = new Vector3(Camera.main.transform.position.x - 0.4f, presenceAnimojiBoard.transform.position.y - 0.2f, presenceAnimojiBoard.transform.position.z);

                    // Avatar is still outside the FOV of VR user
                    if (mainCameraYAxis >= 320 || (mainCameraYAxis > 0 && mainCameraYAxis <= 90))
                    {
                        bystanderAvatar.SetActive(false);

                        // presenceAnimojiBoard.transform.position = guidePos.transform.position;

                        timeElapsedForSEATToFOV += Time.deltaTime;

                        if (timeElapsedForSEATToFOV < lerpGuideTime)
                        {
                            arrowImage.enabled = true;
                            float t = timeElapsedForSEATToFOV / lerpGuideTime;
                            t = t * t * (3f - 2f * t);
                            presenceAnimojiBoard.transform.position = Vector3.Lerp(
                                    originalPos.transform.position,
                                     guidePos.transform.position,
                                     t);
               
                                arrowImage.transform.position = Vector3.Lerp(
                                    originalArrowPos.transform.position,
                                    arrowPosition.transform.position,
                                   t);
                        }
                        else
                        {
                            presenceAnimojiBoard.transform.position = guidePos.transform.position;
                            if (doInteraction)
                            {
                                arrowImage.transform.position = arrowPosition.transform.position;
                            }
                        }

                        backsideImage.enabled = false;
                        if (doInteraction)
                        {
                            noInteractionFrontImage.enabled = false;
                            yesInteractionFrontImage.enabled = true;
                            yesInteractionFrontImage.transform.localScale = new Vector2(1.5f, 1.5f);
                        }
                    }
                    // VR user looking at the Bystander
                    else if (mainCameraYAxis < 320 && mainCameraYAxis >= 250)
                    {
                        bystanderAvatar.SetActive(true);
                        transform.position = trackerTrans.position;
                        transform.localEulerAngles = new Vector3(0, bystanderEulerYAxis, 0);

                        noInteractionFrontImage.enabled = false;
                        yesInteractionFrontImage.enabled = false;
                        backsideImage.enabled = false;
                        arrowImage.enabled = false;
                    }
                }
                // TRANSITION ZONE: 60 >= [Bystander's degrees] > 30
                else if (bystanderEulerYAxis >= 30 && bystanderEulerYAxis < 60)
                {
                    // VR user is looking at the game -> The bystander's avatar is outside the VR user's FOV
                    if (mainCameraYAxis >= 320 || (mainCameraYAxis > 0 && mainCameraYAxis <= 90))
                    {
                        bystanderAvatar.SetActive(false);
                        bystanderAnim.SetBool("isInteracting", false);
                        presenceAnimojiBoard.transform.position = originalPos.transform.position;
                        inTransitionZone = true;
                        timeElapsedForMixed = 0;

                        if (inCriticalZone) // From Critical Zone
                        {
                            BystanderShiftZone("From_CZ_to_TZ");
                            backsideImage.enabled = true;
                            arrowImage.enabled = false;
                            yesInteractionFrontImage.enabled = false;
                            noInteractionFrontImage.enabled = false;
                            inCriticalZone = false;
                        }
                        if(inNoZone) // From uncritical Zone
                        {
                            BystanderShiftZone("From_UCZ_to_TZ");
                            backsideImage.enabled = false;
                            arrowImage.enabled = false;
                        
                            yesInteractionFrontImage.enabled = false;
                            noInteractionFrontImage.enabled = true;
                            noInteractionFrontImage.transform.localScale = new Vector2(1f, 1f);
                            inNoZone = false;
                        }
                    }
                    // VR user is looking at the bystander
                    else if (mainCameraYAxis < 320 && mainCameraYAxis >= 250)
                    {
                        bystanderAvatar.SetActive(true);
                        bystanderAnim.SetBool("isInteracting", false);

                        transform.position = trackerTrans.position;
                        transform.localEulerAngles = new Vector3(0, bystanderEulerYAxis, 0);

                        noInteractionFrontImage.enabled = false;
                        yesInteractionFrontImage.enabled = false;
                        backsideImage.enabled = false;
                        arrowImage.enabled = false;
                    }
                }
                // UNCRITICAL ZONE: 80/85 >= Bystander's degrees > 60
                else if (bystanderEulerYAxis < 30 && bystanderEulerYAxis >= 5)
                {
                    inUncriticalZone = false;
                    yesInteractionFrontImage.enabled = false;
                    noInteractionFrontImage.enabled = false;

                    // VR user is looking at the game -> The bystander's avatar is outside the VR user's FOV
                    if (mainCameraYAxis >= 320 || (mainCameraYAxis > 0 && mainCameraYAxis <= 90))
                    {
                        bystanderAvatar.SetActive(false);
                        bystanderAnim.SetBool("isInteracting", false);
                        presenceAnimojiBoard.transform.position = originalPos.transform.position;
                      //  backsideImage.enabled = true;
                      //  noInteractionFrontImage.enabled = false;
                      //  yesInteractionFrontImage.enabled = false;
                        arrowImage.enabled = false;

                        if(inNoZone && !inTransitionZone) // From No-Zone
                        {
                            timeElapsedForMixed += Time.deltaTime;
                            if(timeElapsedForMixed < fadeTime)
                            {
                                Debug.Log("TimeForMixed (fade in): " + timeElapsedForMixed);
                                float t = timeElapsedForMixed / fadeTime;
                                t = t * t * (3f - 2f * t);
                                backsideImage.enabled = true;
                                backsideImage.color = Color.Lerp(lowTransparency, noTransparency, t);
                            }
                            else
                            {
                                backsideImage.color = noTransparency;
                                inNoZone = false;
                                Debug.Log("Transparence is full and nozone is set as: " + inNoZone);
                            }
                        }

                        if (inTransitionZone && !inNoZone) // From Shift Zone
                        {
                            timeElapsedForMixed += Time.deltaTime;
                            if (timeElapsedForMixed < fadeTime)
                            {
                                Debug.Log("TimeForMixed (fade out): " + timeElapsedForMixed);
                                float t = timeElapsedForMixed / fadeTime;
                                t = t * t * (3f - 2f * t);
                                backsideImage.enabled = true;
                                backsideImage.color = Color.Lerp(noTransparency, lowTransparency, t);
                            }
                            else
                            {
                                backsideImage.color = lowTransparency;
                                inTransitionZone = false;
                                Debug.Log("Transparence is 0 and shift zone is set as: " + inTransitionZone);
                            }
                        }
                    }
                    // VR user is looking at the seated position 
                    else if (mainCameraYAxis < 320 && mainCameraYAxis >= 250)
                    {
                        // Showing Avatar in seated position
                        bystanderAvatar.SetActive(true);
                        bystanderAnim.SetBool("isInteracting", false);

                        transform.position = trackerTrans.position;
                        transform.localEulerAngles = new Vector3(0, bystanderEulerYAxis, 0);

                        noInteractionFrontImage.enabled = false;
                        yesInteractionFrontImage.enabled = false;
                        backsideImage.enabled = false;
                        arrowImage.enabled = false;
                    }
                }
                else
                {
                    // No Visualisation
                    inNoZone = true;
                    inCriticalZone = false;
                    inTransitionZone = false;
                    inUncriticalZone = false;
                    bystanderAvatar.SetActive(false);
                    bystanderAnim.SetBool("isInteracting", false);
                    presenceAnimojiBoard.transform.position = originalPos.transform.position;
                    backsideImage.enabled = false;
                    yesInteractionFrontImage.enabled = false;
                    noInteractionFrontImage.enabled = false;
                    arrowImage.enabled = false;
                }
            }

        }
        /**************************************************
        ********** To the right side of the VR user
        ***********************************************/
        else
        {
            // critical zone
            if (bystanderEulerYAxis <= 300 && bystanderEulerYAxis >= 260)
            {
                if (isAnimojiSetting)
                {
                    backsideImage.enabled = false;
                    yesInteractionFrontImage.enabled = true;
                    // TODO: image bigger and animations
                }
                else if (isAvatarSetting)
                {
                    if (mainCameraYAxis >= 60 && mainCameraYAxis <= 100)
                    {
                        transform.position = bystanderTracker.transform.position;
                    }
                    else
                    {
                        transform.position = FOVPos.transform.position;
                    }
                  
                    transform.localEulerAngles = new Vector3(0, bystanderEulerYAxis, 0);
                    // Debug.Log(bystanderRotationEulerY);
                    bystanderAvatar.SetActive(true);
                }
                else if (isMixedSetting)
                {
                    transform.localEulerAngles = new Vector3(0, bystanderEulerYAxis, 0);
                    bystanderAvatar.SetActive(true);

                    yesInteractionFrontImage.enabled = false;
                    backsideImage.enabled = false;
                }
            }
            // pheriperal zone
            else if (bystanderEulerYAxis <= 330 && bystanderEulerYAxis > 300)
            {
                if (isAnimojiSetting)
                {
                    backsideImage.enabled = false;
                    yesInteractionFrontImage.enabled = true;
                }

                if (isAvatarSetting)
                {
                   
                   transform.position = FOVPos.transform.position;
                    

                    transform.localEulerAngles = new Vector3(0, bystanderEulerYAxis, 0);
                    bystanderAvatar.SetActive(true);
                }

                if (isMixedSetting)
                {
                    bystanderAvatar.SetActive(false);
                    backsideImage.enabled = false;
                    yesInteractionFrontImage.enabled = true;
                }
            }
            else if (bystanderEulerYAxis <= 360 && bystanderEulerYAxis > 300)
            {
                if (isAnimojiSetting)
                {
                    backsideImage.enabled = true;
                    yesInteractionFrontImage.enabled = false;
                }

                if (isAvatarSetting)
                {
                    transform.localEulerAngles = new Vector3(0, bystanderEulerYAxis, 0);
                    bystanderAvatar.SetActive(true);
                }

                if (isMixedSetting)
                {
                    bystanderAvatar.SetActive(false);
                    backsideImage.enabled = true;
                    yesInteractionFrontImage.enabled = false;
                }
            }
            else
            {
                if (isAnimojiSetting)
                {
                    backsideImage.enabled = false;
                    yesInteractionFrontImage.enabled = false;
                }

                if (isAvatarSetting)
                {
                    transform.localEulerAngles = new Vector3(0, bystanderEulerYAxis, 0);
                    bystanderAvatar.SetActive(false);
                }

                if (isMixedSetting)
                {
                    bystanderAvatar.SetActive(false);
                    backsideImage.enabled = false;
                    yesInteractionFrontImage.enabled = false;
                }
            }
        }
    }

    private void BystanderEnterCriticalZone()
    {
        bsgameManager.SetTimeStampForAvatarInCriticalZone();
    }

    private void BystanderShiftZone(string state)
    {
        bsgameManager.SetTimeStampForAvatarInCriticalZoneWithMessage(state);
    }

    public void SetGuide()
    {
        Debug.Log("setGuide is called");
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

        yesInteractionFrontImage.enabled = true;
        backsideImage.enabled = false;
        yesInteractionFrontImage.transform.localScale = new Vector2(1.5f, 1.5f);
        // frontImage.GetComponent<RectTransform>().rect.Set(0, 0, 100, 300);
    }

    //public void GuideToBystander()
    //{
    //    transform.position = new Vector3(FOVPos.transform.position.x, bystanderTracker.transform.position.y, FOVPos.transform.position.z);
    //    //transform.position = new Vector3(middlePos.transform.position.x, bystanderTracker.transform.position.y, middlePos.transform.position.z);
    //    isGuidingToSeated = true;
    //    if (isguided)
    //    {
    //        Invoke("SetGuided", 2f);
    //    }
    //}

    //public void SetGuided()
    //{
    //    isguided = true;
    //    transform.position = Vector3.Lerp(transform.position, new Vector3(middlePos.transform.position.x, bystanderTracker.transform.position.y, middlePos.transform.position.z), Time.deltaTime * 2);
    //    Invoke("GuideToBystander", 2f);
    //}


    IEnumerator FadeImage(bool fadeOut)
    {
        if (fadeOut)
        {
            for (float f = 0.1f; f <= 1; f += 0.1f)
            {
                Debug.Log("Fade called");
                Color c = backsideImage.color;
                c.a = f;
                backsideImage.color = c;
                yield return null;
            }
        }
        else
        {
            for (float f = 0.1f; f <= 1; f -= 0.1f)
            {
                Color c = backsideImage.color;
                c.a = f;
                backsideImage.color = c;
                yield return null;
            }
        }
    }
}
