using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BSBystanderAvatar : MonoBehaviour
{
    public GameObject bystanderTracker;
    private Transform trackerTransform;
    BeatSaberGameManager bsgameManager;
    BSLogManager logManager;
    UserStudyManager userstudyManager;
    private float trackerEulerYAxis;  // bystander's euler y-axis -> tracker y
    public GameObject bystanderAvatar;
    public Animator bystanderAnim;
    [SerializeField]
    private bool doInteraction = true;
    public RawImage arrowImage;

    [Header("GOs for Animoji")]
    // Variables for Animoji Setting
    public GameObject presenceAnimojiBoard;
    public RawImage yesInteractionFrontImage, noInteractionFrontImage, backsideImage;
    public GameObject arrowPositionForMixed;
    public GameObject originalArrowPos;

    [Header("GOs for Avatar")]
    // variables for Avatar Setting
    public GameObject FOVPos;
    public GameObject guidePosForMixed;
    public GameObject originalAnimojiPanelPos;
    public GameObject middlePos;
    private Transform guidingPos;
    public GameObject arrowPosAvatarStart, arrowPosAvatarEnd, arrowPosAvatarMiddle, arrowParticle, arrowParticlePos;
    public GameObject guidingPosForAV;

    [Header("GOs for Mixed")]
    private float timeElapsedForMixedTransition;

    [Header("Time Settings")]
    public float timeToReachTarget;
    public float movingTime = 0f;

    [Header("User-Study Settings")]
    public bool sitToLeft;  // Where is the bystander sitting?
    public bool isAnimojiSetting, isAvatarSetting, isMixedSetting;
    public bool isPractice;

    [Header("Avatar Sub-Settings")]
    // public bool isSeatedAndInFOV;

    private float mainCameraYAxis;
    private bool isGuidingFOVToSeatedExceed;
    public bool isguided;

    public float angleinFOV = 45f;

    private float guidingLength;
    private float guidingSpeed = 1.0f;
    private float timeElapsedForSeatToFOV, timeElapsedForFOVToSeat, timeElapsedForGuideToSEAT;
    public float guideTimeForAvatar = 3f;
    public float fadeTime = 2f;
    float timeElapsedForAnimoji = 0f;

    [SerializeField]
    private bool inCriticalZone, inTransitionZone, inUncriticalZone, inNoZone;
    private bool lookedOnceSeatedPosition;
    private Color animojiBacksideColor;
    Color noTransparency, lowTransparency;
    public bool askedQuestion;
    private bool firstQuestioned, secondQuestioned, thirdQuestioned;
    private int[] studyOrder = new int[3];
    int currentLevelIndex; // practice 0
    int order;
    public bool LookedOnceSeatedPosition { get => lookedOnceSeatedPosition; set => lookedOnceSeatedPosition = value; }
    public bool IsGuidingFOVToSeatedExceed { get => isGuidingFOVToSeatedExceed; set => isGuidingFOVToSeatedExceed = value; }

    private void Awake()
    {
        bsgameManager = FindObjectOfType<BeatSaberGameManager>();
        logManager = FindObjectOfType<BSLogManager>();
        userstudyManager = FindObjectOfType<UserStudyManager>();
    }
    // Start is called before the first frame update
    void Start()
    {
        doInteraction = true;
        sitToLeft = true;
        guidingPos = GetComponent<Transform>(); // For Avatar Setting (FOV -> Seated)
        bystanderAnim.SetBool("isInteracting", false);
       // Debug.Log("studyorder in avatarscript[b]: " + studyOrder[0] + studyOrder[1] + studyOrder[2] + " length: " + studyOrder.Length);
        studyOrder = userstudyManager.studyOrder;
       // Debug.Log("studyorder in avatarscript[a]: " + studyOrder[0] + studyOrder[1] + studyOrder[2] + " length: " + studyOrder.Length);
        currentLevelIndex = SceneManager.GetActiveScene().buildIndex;
       // Debug.Log(currentLevelIndex);

        for(int i = 0; i < studyOrder.Length; i++)
        {
            int order = studyOrder[i];
           // Debug.Log(i + " index: " + index);
            if (i == currentLevelIndex)
                this.order = order;
        }

      //  Debug.Log("order: " + order);

        if(order == 1)
        {
            isAnimojiSetting = true;
        }else if(order == 2)
        {
            isAvatarSetting = true;
        }else if(order == 3)
        {
            isMixedSetting = true;
        }

        
        // Default setting: Avatar setting
        if (!(isAnimojiSetting || isMixedSetting || isAvatarSetting) && !bsgameManager.isPracticeGame)
            isAvatarSetting = true;

       // Debug.Log("Animoji: " + isAnimojiSetting + " Avatar: " + isAvatarSetting + " Mixed: " + isMixedSetting);
        yesInteractionFrontImage.enabled = false;
        noInteractionFrontImage.enabled = false;
        backsideImage.enabled = false;
        bystanderAvatar.SetActive(false);
        arrowImage.enabled = false;

        trackerTransform = bystanderTracker.transform;
        transform.position = trackerTransform.position;
        trackerEulerYAxis = trackerTransform.eulerAngles.y;
        // bystanderRotationOffset = bystanderEulerYAxis - 0f;
        mainCameraYAxis = Camera.main.transform.eulerAngles.y;

        guidingLength = Vector3.Distance(new Vector3(FOVPos.transform.position.x, bystanderTracker.transform.position.y, FOVPos.transform.position.z), trackerTransform.position);

        noTransparency = backsideImage.color;
        lowTransparency = backsideImage.color;
        noTransparency.a = 1f;
        lowTransparency.a = 0f;
    }

    void Update()
    {
        // transform.position = trackerTrans.position;
        trackerEulerYAxis = trackerTransform.eulerAngles.y;
        mainCameraYAxis = Camera.main.transform.eulerAngles.y;

        // For animoji? Avatar? guiding
        middlePos.transform.position = new Vector3(
            (originalAnimojiPanelPos.transform.position.x + trackerTransform.position.x) / 2,
            (originalAnimojiPanelPos.transform.position.y + trackerTransform.position.y) / 2,
            (originalAnimojiPanelPos.transform.position.z + trackerTransform.position.z) / 2);

        // For avatar guiding to seated pos
        guidingPos.position = new Vector3(
            (FOVPos.transform.position.x + trackerTransform.position.x) / 2,
            (FOVPos.transform.position.y + trackerTransform.position.y) / 2,
            (FOVPos.transform.position.z + trackerTransform.position.z) / 2);

        // The bystander is sitting to the left of the VR Player.
        if (sitToLeft)
        {
   /**********************************************************************************************
   ***************************** ANIMOJI SETTING *************************************************                                                              
   *********************************************************************************************/
            if (isAnimojiSetting)
            {
                //  [Animoji]  CRITICAL ZONE: 30 >= [Bystander's degrees] > 0 to the VR user
                if (trackerEulerYAxis >= 60 && trackerEulerYAxis < 100)
                {
                    if (!inCriticalZone && inTransitionZone) // From Transition Zone (60-30 degrees)
                    {
                        Debug.Log("Enter_CZ");
                        BystanderShiftZone("Enter_CZ");
                        inCriticalZone = true;
                    }

                    // TODO: askedQuestion 
                    if (!askedQuestion)
                    {
                        // Bigger Animoji with FE
                        backsideImage.enabled = false;
                        noInteractionFrontImage.enabled = false;
                        yesInteractionFrontImage.enabled = true;
                        yesInteractionFrontImage.transform.localScale = new Vector2(1.5f, 1.5f);
                    }
                    else
                    {
                        // Bigger Animoji with FE
                        backsideImage.enabled = false;
                        yesInteractionFrontImage.enabled = false;
                        noInteractionFrontImage.enabled = true;
                        noInteractionFrontImage.transform.localScale = new Vector2(1.5f, 1.5f);
                    }
                }
                // [Animoji] TRANSITION ZONE: 60 >= [Bystander's degrees] > 30
                else if (trackerEulerYAxis >= 30 && trackerEulerYAxis < 60)
                {
                    inTransitionZone = true;
                    inNoZone = false;
                    timeElapsedForAnimoji = 0;

                    if (inCriticalZone) // From Critical Zone : Bigger animoji with FE -> backside
                    {
                        Debug.Log("From_CZ_to_TZ");
                        BystanderShiftZone("From_CZ_to_TZ");
                        backsideImage.enabled = true;
                        yesInteractionFrontImage.transform.localScale = new Vector2(1f, 1f);
                        yesInteractionFrontImage.enabled = false;
                        noInteractionFrontImage.enabled = false;
                        inCriticalZone = false;
                    }
                    if (inUncriticalZone) // From Uncritical Zone: backside -> small animoji
                    {
                        Debug.Log("From_UZ_to_TZ");
                        BystanderShiftZone("From_UZ_to_TZ");
                        backsideImage.enabled = false;
                        yesInteractionFrontImage.enabled = false;
                        noInteractionFrontImage.enabled = true;
                        noInteractionFrontImage.transform.localScale = new Vector2(1f, 1f);
                        inUncriticalZone = false;
                    }
                }
                // [Animoji] UNCRITICAL ZONE: 85 >= Bystander's degrees > 60
                else if (trackerEulerYAxis < 30 && trackerEulerYAxis >= 5)
                {
                    inUncriticalZone = true;

                    yesInteractionFrontImage.enabled = false;
                    noInteractionFrontImage.enabled = false;

                    if (inNoZone && !inTransitionZone) // From No-Zone : Full transparency to No transparency
                    {
                        //Debug.Log("FROM_NZ_to_UCZ");
                        // BystanderShiftZone("FROM_NZ_to_UCZ");
                        timeElapsedForAnimoji += Time.deltaTime;

                        if (timeElapsedForAnimoji < fadeTime) // fadetime: 2f (default)
                        {
                            //  Debug.Log("timeForAnimoji (fade in): " + timeElapsedForAnimoji);
                            float t = timeElapsedForAnimoji / fadeTime;
                            t = t * t * (3f - 2f * t);
                            backsideImage.enabled = true;
                            backsideImage.color = Color.Lerp(lowTransparency, noTransparency, t);
                        }
                        else // more than fadetime(2f) 
                        {
                            backsideImage.color = noTransparency;
                            inNoZone = false;
                            //  Debug.Log("Transparency is full and NoZone is set " + inNoZone);
                        }
                    }

                    if (inTransitionZone && !inNoZone) // From Transition Zone: No transparency to Full transparency
                    {
                        //  Debug.Log("FROM_TZ_to_UCZ");
                        //  BystanderShiftZone("FROM_TZ_to_UCZ");
                        timeElapsedForAnimoji += Time.deltaTime;

                        if (timeElapsedForAnimoji < fadeTime)
                        {
                            //Debug.Log("timeForAnimoji (fade out): " + timeElapsedForAnimoji);
                            float t = timeElapsedForAnimoji / fadeTime;
                            t = t * t * (3f - 2f * t);
                            backsideImage.enabled = true;
                            backsideImage.color = Color.Lerp(noTransparency, lowTransparency, t);
                        }
                        else  // more than fadetime(2f) 
                        {
                            backsideImage.color = lowTransparency;
                            inTransitionZone = false;
                            // Debug.Log("Transparency is 0 and Shift-Zone is set " + inTransitionZone);
                        }
                    }
                }
                // [Animoji] NO ZONE:  Bystander's degrees > 85
                else
                {
                    //Debug.Log("ENTER_NZ");
                    // Set flags
                    inNoZone = true;
                    inCriticalZone = false;
                    inTransitionZone = false;
                    inUncriticalZone = false;
                    // No Visualisation
                    backsideImage.enabled = false;
                    yesInteractionFrontImage.enabled = false;
                    noInteractionFrontImage.enabled = false;
                    timeElapsedForAnimoji = 0f;
                }
            }
   /**********************************************************************************************
    ***************************** AVATAR SETTING *************************************************                                                              
    *********************************************************************************************/
            if (isAvatarSetting)
            {
                // [AVATAR]  CRITICAL ZONE: 30-0 degrees to the VR user
                if (trackerEulerYAxis >= 60 && trackerEulerYAxis < 100)
                {
                    // inNoZone = false;
                    // inUncriticalZone = false;
                    // inTransitionZone = false;
                    if (inTransitionZone)
                    {
                        Debug.Log("Avatar: From TZ to CZ");
                        BystanderShiftZone("Enter_CZ"); // enter 30 (30->0) degrees
                        inTransitionZone = false;
                    }
                    inCriticalZone = true;
                    //if (!inCriticalZone) // From transition zone
                    //{
                    //    Debug.Log("Avatar: From TZ to CZ");
                    //    BystanderShiftZone("Enter_CZ"); // enter 30 (30->0) degrees
                    //    inCriticalZone = true;
                    //}

                    // Visualisation with FE
                    bystanderAnim.SetBool("isInteracting", true);

                    // VR user is looking at the seated position: in the far peripheral zone
                    // TODO: depending on the headset models
                    if (mainCameraYAxis >= 250 && mainCameraYAxis <= 300) //310 -> 300
                    {
                            // Avatar: seated position & same rotation
                            transform.position = bystanderTracker.transform.position;
                            bystanderAvatar.transform.eulerAngles = new Vector3(0, trackerEulerYAxis, 0);

                        // ARROW
                        arrowImage.enabled = false;
                        //if (mainCameraYAxis >= 250 && mainCameraYAxis  <= 280)
                        //    {
                        //        arrowImage.enabled = false;
                        //    }
                        //    arrowImage.enabled = true;
                        //    arrowImage.transform.position = arrowPosAvatarMiddle.transform.position;
                        //    arrowPosAvatarMiddle.transform.eulerAngles = new Vector3(0, bysTrackerEulerYAxis, 0);

                        lookedOnceSeatedPosition = true;
                    }
                    //else if(mainCameraYAxis > 310 && mainCameraYAxis <= 315) 
                    //{
                    //    bystanderAvatar.SetActive(false);
                    //    arrowImage.enabled = false;
                    //}
                    else  // The bystander is outside the FOV of the VR user ( 310 < d < 360, ....) 
                    {
                        movingTime += Time.deltaTime;
                        if (movingTime > (guideTimeForAvatar + 2f)) // 2+ 2f
                        { 
                            isGuidingFOVToSeatedExceed = true;
                        }

                        // The VR user haven't look at the seated avatar yet
                        if (!lookedOnceSeatedPosition)
                        {
                            // 1. move to the guding pos (border of fov)
                            if (!isGuidingFOVToSeatedExceed)
                            {
                                    timeElapsedForSeatToFOV += Time.deltaTime;
                                    if (timeElapsedForSeatToFOV < guideTimeForAvatar) // lerpGuideTime:2
                                    {
                                        float t = timeElapsedForSeatToFOV / guideTimeForAvatar;
                                        t = t * t * (3f - 2f * t);
                                        // guiding from tracker position -> fov position
                                        transform.position = Vector3.Lerp(
                                            new Vector3(bystanderTracker.transform.position.x, bystanderTracker.transform.position.y, bystanderTracker.transform.position.z),
                                            new Vector3(guidingPosForAV.transform.position.x, bystanderTracker.transform.position.y, guidingPosForAV.transform.position.z),
                                        //  new Vector3(FOVPos.transform.position.x, bystanderTracker.transform.position.y, FOVPos.transform.position.z),
                                                    t);

                                        // Avatar's rotation angle (manipulation)
                                        bystanderAvatar.transform.rotation = Quaternion.Lerp(
                                                Quaternion.Euler(new Vector3(0, trackerEulerYAxis, 0)),
                                                Quaternion.Euler(new Vector3(0, trackerEulerYAxis + ((trackerEulerYAxis * (90 + angleinFOV) / 90) - trackerEulerYAxis), 0)),
                                                t);
                                    }
                                    else // more than guiding (to FOV) Time (2 sec)
                                    {
                                        // Debug.Log("guide time passed: " + timeElapsedForSEATToFOV);
                                        transform.position = new Vector3(guidingPosForAV.transform.position.x, bystanderTracker.transform.position.y, guidingPosForAV.transform.position.z);
                                        bystanderAvatar.transform.rotation = Quaternion.Euler(new Vector3(0, trackerEulerYAxis + ((trackerEulerYAxis * (90 + angleinFOV) / 90) - trackerEulerYAxis), 0));
                                        arrowImage.enabled = true;
                                        arrowImage.transform.position = arrowPosAvatarMiddle.transform.position;
                                // Destroy(arrowParticle);
                                    }
                            }
                            // more than the expected guiding time
                            // -> intensively guide to the seated position
                            else
                            {
                                // ARROW
                                arrowImage.enabled = true;
                                arrowImage.transform.position = arrowPosAvatarStart.transform.position;
                                    // Instantiate(arrowParticle, arrowParticlePos.transform.position, Quaternion.identity);
                              
                                timeElapsedForFOVToSeat += Time.deltaTime;

                                if (timeElapsedForFOVToSeat < guideTimeForAvatar) // gudige time: default 2
                                {
                                    float t = timeElapsedForFOVToSeat / guideTimeForAvatar;
                                    t = t * t * (3f - 2f * t);
                                    transform.position = Vector3.Lerp(
                                        new Vector3(guidingPosForAV.transform.position.x, bystanderTracker.transform.position.y, guidingPosForAV.transform.position.z),
                                        bystanderTracker.transform.position,
                                        t);

                                    // Angles
                                    bystanderAvatar.transform.rotation = Quaternion.Lerp(
                                        Quaternion.Euler(new Vector3(0, trackerEulerYAxis + ((trackerEulerYAxis * (90 + angleinFOV) / 90) - trackerEulerYAxis), 0)),
                                        Quaternion.Euler(new Vector3(0, trackerEulerYAxis, 0)),
                                        t);
                                    
                                    // Arrow for guiding
                                    arrowImage.transform.position = Vector3.Lerp(
                                        arrowPosAvatarStart.transform.position,
                                        arrowPosAvatarMiddle.transform.position,
                                        t);

                                    //arrowPosAvatarEnd.transform.rotation = Quaternion.Lerp(
                                    //    Quaternion.Euler(new Vector3(0, bysTrackerEulerYAxis + ((bysTrackerEulerYAxis * (90 + angleinFOV) / 90) - bysTrackerEulerYAxis), 0)),
                                    //    Quaternion.Euler(new Vector3(0, bysTrackerEulerYAxis, 0)),
                                    //    t);
                                }
                                // the guding time passed 
                                else
                                {
                                    transform.position = bystanderTracker.transform.position;
                                    bystanderAvatar.transform.rotation = Quaternion.Euler(new Vector3(0, trackerEulerYAxis, 0));
                                    arrowImage.enabled = true;
                                    arrowImage.transform.position = arrowPosAvatarMiddle.transform.position;
                                }
                            }
                        }
                        else // already quided to seated once. (looked once the seated pos) but, looked at now FOV
                        {
                            transform.position = new Vector3(guidingPosForAV.transform.position.x, bystanderTracker.transform.position.y, guidingPosForAV.transform.position.z);
                            bystanderAvatar.transform.eulerAngles = new Vector3(0, trackerEulerYAxis + ((trackerEulerYAxis * (90 + angleinFOV) / 90) - trackerEulerYAxis), 0);
                            arrowImage.enabled = true;       
                            arrowImage.transform.position = arrowPosAvatarMiddle.transform.position;
                        }
                    }
                }
                // [AVATAR] TRANSITION ZONE: 60 >= [Bystander's degrees] > 30
                else if (trackerEulerYAxis >= 30 && trackerEulerYAxis < 60)
                {
                   // inNoZone = false;
                    // From Uncritical Zone to Transition Zone
                    if (inUncriticalZone)
                    {
                        Debug.Log("Avatar:From UCZ to TZ");
                        BystanderShiftZone("From_UCZ_to_TZ");

                        // avatar without FE in the seated position 
                        transform.position = bystanderTracker.transform.position;
                        transform.localEulerAngles = new Vector3(0, trackerEulerYAxis, 0); // tracker y-axis

                        // Visualisation ON (No FE)
                        bystanderAvatar.SetActive(true);
                        bystanderAnim.SetBool("isInteracting", false);
                        arrowImage.enabled = false;

                        inUncriticalZone = false;
                    }
                    else
                    {
                        transform.position = bystanderTracker.transform.position;
                        transform.localEulerAngles = new Vector3(0, trackerEulerYAxis, 0); // tracker y-axis
                    }

                    inTransitionZone = true;

                    if (inCriticalZone) // From Critical Zone to Transition Zone
                    {
                        Debug.Log("Avatar:From CZ to TZ");
                        BystanderShiftZone("From_CZ_to_TZ");
                        // avatar without FE in the situated position            
                        transform.position = bystanderTracker.transform.position;
                        // manimpulation of the y-axis (rotates with bigger y-aixs)
                        // TODO: rational degrees
                       // transform.localEulerAngles = new Vector3(0, trackerEulerYAxis + ((trackerEulerYAxis * (90 + angleinFOV) / 90) - trackerEulerYAxis), 0);
                        transform.localEulerAngles = new Vector3(0, 30, 0);               
                        bystanderAvatar.transform.eulerAngles = new Vector3(0, 0, 0);

                        // Visualisation ON (No FE)
                        bystanderAvatar.SetActive(true);
                        bystanderAnim.SetBool("isInteracting", false);
                        arrowImage.enabled = false;

                        inCriticalZone = false;
                    }
                    else
                    {
                        transform.position = bystanderTracker.transform.position;
                        transform.localEulerAngles = new Vector3(0, 30, 0);
                    }
                    // Reset timer values as 0
                    movingTime = 0f; // for guiding to the critical zone 
                    timeElapsedForSeatToFOV = 0f;
                    timeElapsedForFOVToSeat = 0f;
                }
                // [AVATAR] UNCRITICAL ZONE: 85 >= Bystander's degrees > 60
                else if (trackerEulerYAxis < 30 && trackerEulerYAxis >= 5)
                {
                    if (inNoZone)
                    {
                        Debug.Log("Avatar: From No to UCZ");
                        BystanderShiftZone("From_NZ_To_UCZ");
                        inNoZone = false;
                    }
                    inUncriticalZone = true;
                    if (inTransitionZone)
                    {
                        Debug.Log("Avatar: From TZ to UCZ");
                        BystanderShiftZone("From_TZ_To_UCZ");
                        inTransitionZone = false;
                    }
                  // inCriticalZone = false;

                    // Avatar without FE in the seated position 
                    // Position
                    transform.position = bystanderTracker.transform.position;
                    transform.localEulerAngles = new Vector3(0, trackerEulerYAxis, 0); // tracker y-axis
                    // Visualisation ON (No FE)
                    bystanderAvatar.SetActive(true);
                    bystanderAnim.SetBool("isInteracting", false);
                    // No guiding
                    arrowImage.enabled = false;

                    //movingTime = 0f;
                    //timeElapsedForSeatToFOV = 0f;
                    //timeElapsedForFOVToSeat = 0f;
                }
                // [AVATAR] NO ZONE:  Bystander's degrees > 85
                else
                {
                    inNoZone = true;
                    if (inUncriticalZone)
                    {
                        Debug.Log("Avatar: From UCZ To NZ");
                        BystanderShiftZone("From_UCZ_To_NZ");
                        inUncriticalZone = false;
                    }
                    inTransitionZone = false;
                    inCriticalZone = false;

                    // Nothing is visualisized
                    transform.position = bystanderTracker.transform.position;
                    transform.localEulerAngles = new Vector3(0, trackerEulerYAxis, 0); // tracker y-axis
                    bystanderAvatar.SetActive(false);
                }
            }
    /**********************************************************************************************
    ***************************** MIXED SETTING *************************************************                                                              
    *********************************************************************************************/
            if (isMixedSetting)
            {
                // [MIXED] CRITICAL ZONE: 30 >= [Bystander's degrees] > 0 to the VR user
                if (trackerEulerYAxis >= 60 && trackerEulerYAxis < 100) // 100 <- 90
                {
                    if (!inCriticalZone && inTransitionZone)
                    {
                        Debug.Log("Enter CZ");
                        BystanderShiftZone("Enter CZ");
                        inCriticalZone = true;
                        // inTransitionZone = false;
                    }
                    // presenceAnimojiBoard.transform.position = new Vector3(Camera.main.transform.position.x - 0.4f, presenceAnimojiBoard.transform.position.y - 0.2f, presenceAnimojiBoard.transform.position.z);

                    // Avatar is still outside the FOV of VR user
                    // 320 - 315 - 300
                    if (mainCameraYAxis >= 320 || (mainCameraYAxis > 0 && mainCameraYAxis <= 90))
                    {
                        // Animoji Guiding
                        bystanderAvatar.SetActive(false);

                        //presenceAnimojiBoard.transform.position = guidePos.transform.position;
                        presenceAnimojiBoard.transform.position = originalAnimojiPanelPos.transform.position;
                        timeElapsedForMixedTransition += Time.deltaTime;


                        if (timeElapsedForMixedTransition < guideTimeForAvatar)
                        {
                            Debug.Log("guiding: timeElapsedForMixedTransition: " + timeElapsedForMixedTransition);
                            arrowImage.enabled = true;
                            float t = timeElapsedForMixedTransition / guideTimeForAvatar;
                            t = t * t * (3f - 2f * t);
                            presenceAnimojiBoard.transform.position = Vector3.Lerp(
                                originalAnimojiPanelPos.transform.position,
                                guidePosForMixed.transform.position,
                                t);

                            arrowImage.transform.position = Vector3.Lerp(
                                originalArrowPos.transform.position,
                                arrowPositionForMixed.transform.position,
                                t);
                        }
                        // time passed
                        else
                        {
                            Debug.Log("done: timeElapsedForMixedTransition: " + timeElapsedForMixedTransition);
                            presenceAnimojiBoard.transform.position = guidePosForMixed.transform.position;
                            arrowImage.transform.position = arrowPositionForMixed.transform.position;
                        }

                        backsideImage.enabled = false;
                        noInteractionFrontImage.enabled = false;
                        yesInteractionFrontImage.enabled = true;
                        yesInteractionFrontImage.transform.localScale = new Vector2(1.5f, 1.5f);
                    }
                    // VR user looking at the Bystander
                    // Avatar visualisation
                    else if (mainCameraYAxis < 320 && mainCameraYAxis >= 250)
                    {
                        Debug.Log("VR user is looking at the bystander");
                        noInteractionFrontImage.enabled = false;
                        yesInteractionFrontImage.enabled = false;
                        backsideImage.enabled = false;
                        arrowImage.enabled = false;
                        bystanderAvatar.SetActive(true);
                        transform.position = trackerTransform.position;
                        transform.localEulerAngles = new Vector3(0, trackerEulerYAxis, 0);
                        // TODO: AskedQuestion
                        if (!askedQuestion) // Avatar with FE
                            bystanderAnim.SetBool("isInteracting", true);
                        else // Avatar without FE
                            bystanderAnim.SetBool("isInteracting", false);
                    }
                }
                // [MIXED] TRANSITION ZONE: 60 >= [Bystander's degrees] > 30
                else if (trackerEulerYAxis >= 30 && trackerEulerYAxis < 60)
                {
                    inTransitionZone = true;
                    inNoZone = false;
                    timeElapsedForMixedTransition = 0;
                    presenceAnimojiBoard.transform.position = originalAnimojiPanelPos.transform.position;
                    // VR user is looking at the game -> The bystander's avatar is outside the VR user's FOV
                    // ANIMOJI Visualisation
                    if (mainCameraYAxis >= 320 || (mainCameraYAxis > 0 && mainCameraYAxis <= 90))
                    {
                        Debug.Log("Transitional: Animoji");
                        bystanderAvatar.SetActive(false);
                        bystanderAnim.SetBool("isInteracting", false);
                        presenceAnimojiBoard.transform.position = originalAnimojiPanelPos.transform.position;

                        if (inCriticalZone) // From Critical Zone to Transition Zone
                        {
                            Debug.Log("From_CZ_to_TZ");
                            BystanderShiftZone("From_CZ_to_TZ");
                            // show backside
                            backsideImage.enabled = true;
                            arrowImage.enabled = false;
                            yesInteractionFrontImage.enabled = false;
                            noInteractionFrontImage.enabled = false;
                            inCriticalZone = false;
                        }
                        if (inUncriticalZone) // From Uncritical Zone
                        {
                            Debug.Log("From_UCZ_to_TZ");
                            BystanderShiftZone("From_UCZ_to_TZ");
                            backsideImage.enabled = false;
                            yesInteractionFrontImage.enabled = false;
                            noInteractionFrontImage.enabled = true;
                            noInteractionFrontImage.transform.localScale = new Vector2(1f, 1f);
                            arrowImage.enabled = false;
                            inUncriticalZone = false;
                        }
                    }
                    // VR user is looking at the bystander
                    else if (mainCameraYAxis < 320 && mainCameraYAxis >= 250)
                    {
                        bystanderAvatar.SetActive(true);
                        bystanderAnim.SetBool("isInteracting", false);

                        transform.position = trackerTransform.position;
                        transform.localEulerAngles = new Vector3(0, trackerEulerYAxis, 0);

                        noInteractionFrontImage.enabled = false;
                        yesInteractionFrontImage.enabled = false;
                        backsideImage.enabled = false;
                        arrowImage.enabled = false;
                    }
                }
                // [MIXED] UNCRITICAL ZONE: 85 >= Bystander's degrees > 60
                else if (trackerEulerYAxis < 30 && trackerEulerYAxis >= 5)
                {
                    inUncriticalZone = true;
                    inCriticalZone = false;
                    yesInteractionFrontImage.enabled = false;
                    noInteractionFrontImage.enabled = false;
                    presenceAnimojiBoard.transform.position = originalAnimojiPanelPos.transform.position;
                    // VR user is looking at the game -> The bystander's avatar is outside the VR user's FOV
                    // Animoji Visualisation
                    if (mainCameraYAxis >= 320 || (mainCameraYAxis > 0 && mainCameraYAxis <= 90))
                    {
                        Debug.Log("Uncritical: Animoji");
                        bystanderAvatar.SetActive(false);
                        bystanderAnim.SetBool("isInteracting", false);
                        presenceAnimojiBoard.transform.position = originalAnimojiPanelPos.transform.position;
                        arrowImage.enabled = false;

                        if (inNoZone && !inTransitionZone) // From No-Zone: Full-transparency to No-transparency
                        {
                            timeElapsedForMixedTransition += Time.deltaTime;

                            if (timeElapsedForMixedTransition < fadeTime) // fadeTime: 2f (default)
                            {
                                //  Debug.Log("TimeForMixed (fade in): " + timeElapsedForMixed);
                                float t = timeElapsedForMixedTransition / fadeTime;
                                t = t * t * (3f - 2f * t);
                                backsideImage.enabled = true;
                                backsideImage.color = Color.Lerp(lowTransparency, noTransparency, t);
                            }
                            else // more than fadetime (e.g., 2f)
                            {
                                backsideImage.color = noTransparency;
                                inNoZone = false;
                                // Debug.Log("Transparence is full and nozone is set as: " + inNoZone);
                            }
                        }

                        if (inTransitionZone && !inNoZone) // From Transition Zone: No-Transparency to Full-transparency
                        {
                            timeElapsedForMixedTransition += Time.deltaTime;
                            if (timeElapsedForMixedTransition < fadeTime)
                            {
                                // Debug.Log("TimeForMixed (fade out): " + timeElapsedForMixed);
                                float t = timeElapsedForMixedTransition / fadeTime;
                                t = t * t * (3f - 2f * t);
                                backsideImage.enabled = true;
                                backsideImage.color = Color.Lerp(noTransparency, lowTransparency, t);
                            }
                            else // more than fadetime (e.g., 2f)
                            {
                                backsideImage.color = lowTransparency;
                                inTransitionZone = false;
                                // Debug.Log("Transparence is 0 and shift zone is set as: " + inTransitionZone);
                            }
                        }
                    }
                    // VR user is looking at the seated position 
                    // -> avatar in the seated position
                    else if (mainCameraYAxis < 320 && mainCameraYAxis >= 250)
                    {
                        Debug.Log("Uncritical: Avatar");
                        bystanderAvatar.SetActive(true);
                        bystanderAnim.SetBool("isInteracting", false);
                        transform.position = trackerTransform.position;
                        transform.localEulerAngles = new Vector3(0, trackerEulerYAxis, 0);

                        // noInteractionFrontImage.enabled = false;
                        // yesInteractionFrontImage.enabled = false;
                        backsideImage.enabled = false;
                        arrowImage.enabled = false;
                    }
                }
                // [MIXED] NO ZONE:  Bystander's degrees > 85
                else
                {
                    // Set boolean flags
                    if (!inNoZone)
                    {
                        inNoZone = true;
                        // Debug.Log("in No Zone");
                        //  BystanderShiftZone("NZ");
                    }
                    inUncriticalZone = false;
                    inTransitionZone = false;
                    inCriticalZone = false;
                    // No Visualisation
                    bystanderAvatar.SetActive(false);
                    bystanderAnim.SetBool("isInteracting", false);
                    presenceAnimojiBoard.transform.position = originalAnimojiPanelPos.transform.position; // set the begin postion of Animoji Panel
                    backsideImage.enabled = false;
                    yesInteractionFrontImage.enabled = false;
                    noInteractionFrontImage.enabled = false;
                    arrowImage.enabled = false;
                    timeElapsedForMixedTransition = 0;
                }
            }
        }
        /**************************************************
        ********** To the right side of the VR user
        ***********************************************/
        else
        {
            //    // critical zone
            //    if (bystanderEulerYAxis <= 300 && bystanderEulerYAxis >= 260)
            //    {
            //        if (isAnimojiSetting)
            //        {
            //            backsideImage.enabled = false;
            //            yesInteractionFrontImage.enabled = true;
            //            // TODO: image bigger and animations
            //        }
            //        else if (isAvatarSetting)
            //        {
            //            if (mainCameraYAxis >= 60 && mainCameraYAxis <= 100)
            //            {
            //                transform.position = bystanderTracker.transform.position;
            //            }
            //            else
            //            {
            //                transform.position = FOVPos.transform.position;
            //            }

            //            transform.localEulerAngles = new Vector3(0, bystanderEulerYAxis, 0);
            //            // Debug.Log(bystanderRotationEulerY);
            //            bystanderAvatar.SetActive(true);
            //        }
            //        else if (isMixedSetting)
            //        {
            //            transform.localEulerAngles = new Vector3(0, bystanderEulerYAxis, 0);
            //            bystanderAvatar.SetActive(true);

            //            yesInteractionFrontImage.enabled = false;
            //            backsideImage.enabled = false;
            //        }
            //    }
            //    // pheriperal zone
            //    else if (bystanderEulerYAxis <= 330 && bystanderEulerYAxis > 300)
            //    {
            //        if (isAnimojiSetting)
            //        {
            //            backsideImage.enabled = false;
            //            yesInteractionFrontImage.enabled = true;
            //        }

            //        if (isAvatarSetting)
            //        {

            //           transform.position = FOVPos.transform.position;


            //            transform.localEulerAngles = new Vector3(0, bystanderEulerYAxis, 0);
            //            bystanderAvatar.SetActive(true);
            //        }

            //        if (isMixedSetting)
            //        {
            //            bystanderAvatar.SetActive(false);
            //            backsideImage.enabled = false;
            //            yesInteractionFrontImage.enabled = true;
            //        }
            //    }
            //    else if (bystanderEulerYAxis <= 360 && bystanderEulerYAxis > 300)
            //    {
            //        if (isAnimojiSetting)
            //        {
            //            backsideImage.enabled = true;
            //            yesInteractionFrontImage.enabled = false;
            //        }

            //        if (isAvatarSetting)
            //        {
            //            transform.localEulerAngles = new Vector3(0, bystanderEulerYAxis, 0);
            //            bystanderAvatar.SetActive(true);
            //        }

            //        if (isMixedSetting)
            //        {
            //            bystanderAvatar.SetActive(false);
            //            backsideImage.enabled = true;
            //            yesInteractionFrontImage.enabled = false;
            //        }
            //    }
            //    else
            //    {
            //        if (isAnimojiSetting)
            //        {
            //            backsideImage.enabled = false;
            //            yesInteractionFrontImage.enabled = false;
            //        }

            //        if (isAvatarSetting)
            //        {
            //            transform.localEulerAngles = new Vector3(0, bystanderEulerYAxis, 0);
            //            bystanderAvatar.SetActive(false);
            //        }

            //        if (isMixedSetting)
            //        {
            //            bystanderAvatar.SetActive(false);
            //            backsideImage.enabled = false;
            //            yesInteractionFrontImage.enabled = false;
            //        }
            //    }
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
