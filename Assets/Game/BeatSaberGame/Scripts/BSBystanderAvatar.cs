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
    public RawImage yesInteractionFrontImage;
    public RawImage noInteractionFrontImage;
    public RawImage backsideImage;
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
   // public GameObject arrowPos;
   // public GameObject arrowPosForAvatar;
    // public GameObject arrowOriginalPosForAvatar;
    public GameObject guidingPosForAV;

    [Header("GOs for Mixed")]

    [Header("Time Settings")]
    public float timeToReachTarget;
    public float currentMovementTime = 0f;

    [Header("User-Study Settings")]
    public bool sitToLeft;  // Where is the bystander sitting?
    public bool isAnimojiSetting, isAvatarSetting, isMixedSetting;

    [Header("Avatar Sub-Settings")]
    public bool isSeated, isInFOV, isSeatedAndInFOV;

    private float mainCameraYAxis;
    public bool isGuidingToSeated;
    public bool isguided;

    private float angleinFOV = 50f;

    private float guidingLength;
    private float guidingSpeed = 1.0f;
    private float timeElapsedForGuiding;
    public float lerpDurationForAvatar = 3f;
    public float fadeTime = 2f;
    float timeElapsedForAnimoji = 0f;

    [SerializeField]
    private bool inCriticalZone, inShiftZone, inUncriticalZone, inNoZone;
    private bool fromCriticalSection;

    private Color animojiBacksideColor;
    Color highTransparency;
    Color lowTransparency;

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

        // Default setting: Avatar-> SEATED to FOV setting
        if (isAvatarSetting && !(isSeated || isInFOV || isSeatedAndInFOV))
            isSeatedAndInFOV = true;

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

        highTransparency = backsideImage.color;
        lowTransparency = backsideImage.color;
        highTransparency.a = 1f;
        lowTransparency.a = 0.05f;
    }

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
                       // BystanderEnterCriticalZone(); // enter 30 degrees
                        BystanderMoveZone("Enter_CZ");
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
                // SHIFT ZONE: 60 >= [Bystander's degrees] > 30
                else if (bystanderEulerYAxis >= 30 && bystanderEulerYAxis < 60)
                {
                    inShiftZone = true;

                    if (inCriticalZone) // From Critical Zone
                    {
                        BystanderMoveZone("From_CZ_to_SZ");
                        backsideImage.enabled = true;
                        yesInteractionFrontImage.enabled = false;
                        noInteractionFrontImage.enabled = false;
                        inCriticalZone = false;
                    }
                    else
                    {
                       // BystanderMoveZone("From_UZ_to_SZ");
                        backsideImage.enabled = false;
                        yesInteractionFrontImage.enabled = false;
                        noInteractionFrontImage.enabled = true;
                        noInteractionFrontImage.transform.localScale = new Vector2(1f, 1f);
                    }
                }
                // UNCRITICAL ZONE: 80 >= Bystander's degrees > 60
                else if (bystanderEulerYAxis < 30 && bystanderEulerYAxis >= 5)
                {
                    backsideImage.enabled = true;
                    yesInteractionFrontImage.enabled = false;
                    noInteractionFrontImage.enabled = false;

                    //backsideImage.color = c;

                    if (inNoZone)
                    {
                        timeElapsedForAnimoji += Time.deltaTime;

                        if (timeElapsedForAnimoji < fadeTime)
                        {
                            float t = timeElapsedForAnimoji / fadeTime;
                            t = t * t * (3f - 2f * t);
                            backsideImage.color = Color.Lerp(
                                     lowTransparency,
                                      highTransparency,
                                     t);
                        }
                        else
                        {
                            backsideImage.color = highTransparency;
                            inNoZone = false;
                        }
                    }

                    if (inShiftZone)
                    {
                        timeElapsedForAnimoji += Time.deltaTime;

                        if (timeElapsedForAnimoji < fadeTime)
                        {
                            float t = timeElapsedForAnimoji / fadeTime;
                            t = t * t * (3f - 2f * t);
                            backsideImage.color = Color.Lerp(
                                     highTransparency,
                                      lowTransparency,
                                     t);
                        }
                        else
                        {
                            backsideImage.color = lowTransparency;
                        }
                    }                
                    //if (!inUncriticalZone && inNoZone) // From No-Zone to Uncritical Zone
                    //{
                    //    BystanderMoveZone("From_NZ_to_UZ");

                    //    // backside transparency get lower
                    //    yesInteractionFrontImage.enabled = false;
                    //    noInteractionFrontImage.enabled = false;

                    //    inUncriticalZone = true;
                    //    inNoZone = false;
                    //}

                    //if (!inUncriticalZone && inShiftZone) // from shift to uncritical zone
                    //{
                    //    BystanderMoveZone("From_SZ_to_UZ");
                    //    inShiftZone = false;
                    //    inUncriticalZone = true;
                    //}
                }
               // NO ZONE:  Bystander's degrees > 80
                else 
                {
                    // No Visualisation
                    inNoZone = true;
                    inCriticalZone = false;
                    inShiftZone = false;
                    inUncriticalZone = false;
                    backsideImage.enabled = false;
                    yesInteractionFrontImage.enabled = false;
                    noInteractionFrontImage.enabled = false;
                }
            }
            /*
             * AVATAR SETTING
             */
            if (isAvatarSetting)
            {
                //  CRITICAL ZONE: 30-0 degrees to the VR user
                if (bystanderEulerYAxis >= 60 && bystanderEulerYAxis < 100)
                {
                    if (!inCriticalZone)
                    {
                        BystanderEnterCriticalZone(); // enter 30 degrees
                        BystanderMoveZone("Enter the critical zone");
                        inCriticalZone = true;
                    }

                  //  fromCriticalSection = true;
                    if (doInteraction)
                        bystanderAnim.SetBool("isInteracting", true);
                    else
                        bystanderAnim.SetBool("isInteracting", false);

                    if (isInFOV)
                    {
                        // bystanderAvatar.SetActive(true);
                        transform.position = new Vector3(FOVPos.transform.position.x, bystanderTracker.transform.position.y, FOVPos.transform.position.z);
                        // bystanderAvatar.transform.eulerAngles = new Vector3(0, bystanderYAxis + 50, 0);
                        bystanderAvatar.transform.eulerAngles = new Vector3(0, bystanderEulerYAxis + ((bystanderEulerYAxis * (90 + angleinFOV) / 90) - bystanderEulerYAxis), 0);
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
                        // The bystander is inside VR user's FOV
                        if (mainCameraYAxis >= 250 && mainCameraYAxis <= 310)
                        {
                            transform.position = bystanderTracker.transform.position;
                            bystanderAvatar.transform.eulerAngles = new Vector3(0, bystanderEulerYAxis, 0);
                            // arrowImage.enabled = false;
                            isGuidingToSeated = false;
                            currentMovementTime = 0;
                            timeElapsedForGuiding = 0;
                        }
                        //else if(mainCameraYAxis > 310 && mainCameraYAxis <= 315) 
                        //{
                        //    bystanderAvatar.SetActive(false);
                        //    arrowImage.enabled = false;
                        //}
                        else  // The bystander is outside the FOV of the VR user ( 310 < d < 360, ....)
                        {
                            currentMovementTime += Time.deltaTime;

                            if (!isGuidingToSeated)
                            {
                                bystanderAvatar.transform.eulerAngles = new Vector3(0, bystanderEulerYAxis + ((bystanderEulerYAxis * (90 + angleinFOV) / 90) - bystanderEulerYAxis), 0);
                                transform.position = new Vector3(FOVPos.transform.position.x, trackerTrans.position.y, FOVPos.transform.position.z);
                            }
                            else
                            {
                                if (doInteraction)
                                {
                                    // arrowImage.enabled = true;
                                    // arrowImage.transform.position = arrowPos.transform.position;
                                }

                                timeElapsedForGuiding += Time.deltaTime;

                                if (timeElapsedForGuiding < lerpDurationForAvatar)
                                {
                                    float t = timeElapsedForGuiding / lerpDurationForAvatar;
                                    t = t * t * (3f - 2f * t);
                                    transform.position = Vector3.Lerp(
                                             new Vector3(FOVPos.transform.position.x, bystanderTracker.transform.position.y, FOVPos.transform.position.z),
                                              new Vector3(trackerTrans.position.x, trackerTrans.position.y, trackerTrans.position.z),
                                             t);

                                    //  new Vector3(guidingPosForAV.transform.position.x, tracker.position.y, guidingPosForAV.transform.position.z)

                                    bystanderAvatar.transform.rotation = Quaternion.Lerp(
                                        Quaternion.Euler(bystanderAvatar.transform.eulerAngles),
                                        Quaternion.Euler(new Vector3(0, bystanderEulerYAxis + ((bystanderEulerYAxis * (90 + angleinFOV - 10) / 90) - bystanderEulerYAxis), 0)),
                                         t);

                                    if (doInteraction)
                                    {
                                        // arrowImage.transform.position = Vector3.Lerp(
                                        //arrowPos.transform.position,
                                        //new Vector3(guidingPosForAV.transform.position.x, arrowPos.transform.position.y, guidingPosForAV.transform.position.z),
                                        //t);
                                    }
                                }
                                else
                                {
                                    transform.position = new Vector3(guidingPosForAV.transform.position.x, trackerTrans.position.y, guidingPosForAV.transform.position.z);
                                    // new Vector3(guidingPosForAV.transform.position.x, arrowPos.transform.position.y, guidingPosForAV.transform.position.z);
                                }

                            }

                            if (currentMovementTime > 2f)
                            {
                                isGuidingToSeated = true;
                            }
                        }
                    }

                    transform.localEulerAngles = new Vector3(0, bystanderEulerYAxis, 0);
                    bystanderAvatar.SetActive(true);

                    // fully fade in (1) the image with the duration of 2
                    // bystandreImage.CrossFadeAlpha(1, 1.0f, false);
                    // transform.localEulerAngles = new Vector3(0, bystanderRotationEulerY, 0); // towards the front seat
                    // transform.localEulerAngles = new Vector3(0, 0, 0); // against the front seat
                }
                else if (bystanderEulerYAxis >= 30 && bystanderEulerYAxis < 60)
                {
                    if (fromCriticalSection)
                    {
                        bystanderAnim.SetBool("isInteracting", false);
                        //  transform.position = bystanderTracker.transform.position;
                        transform.localEulerAngles = new Vector3(0, 30, 0);
                        //transform.localEulerAngles = new Vector3(0, 180, 0); // towards the front seat
                        // bystandreImage.CrossFadeAlpha(1, 1.0f, false);
                        bystanderAvatar.SetActive(true);

                        if (isInFOV)
                        {

                        }

                        if (isSeatedAndInFOV)
                        {
                            transform.position = bystanderTracker.transform.position;
                            bystanderAvatar.transform.eulerAngles = new Vector3(0, 0, 0);
                        }
                    }
                    else
                    {
                        bystanderAnim.SetBool("isInteracting", false);
                        //  transform.position = bystanderTracker.transform.position;
                        transform.localEulerAngles = new Vector3(0, bystanderEulerYAxis, 0);
                        //transform.localEulerAngles = new Vector3(0, 180, 0); // towards the front seat
                        // bystandreImage.CrossFadeAlpha(1, 1.0f, false);
                        bystanderAvatar.SetActive(true);

                        if (isInFOV)
                        {
                            transform.position = new Vector3(FOVPos.transform.position.x, bystanderTracker.transform.position.y, FOVPos.transform.position.z);
                            // bystanderAvatar.transform.eulerAngles = new Vector3(0, (bystanderYAxis + 50), 0);
                            bystanderAvatar.transform.eulerAngles = new Vector3(0, bystanderEulerYAxis + ((bystanderEulerYAxis * (90 + angleinFOV) / 90) - bystanderEulerYAxis), 0);
                        }

                        if (isSeatedAndInFOV)
                        {
                            transform.position = bystanderTracker.transform.position;
                            // +
                            bystanderAvatar.transform.eulerAngles = new Vector3(0, bystanderEulerYAxis, 0);
                            //  arrowImage.enabled = false;
                        }
                    }
                }
                else if (bystanderEulerYAxis < 30 && bystanderEulerYAxis >= 5)
                {
                    fromCriticalSection = false;
                    bystanderAnim.SetBool("isInteracting", false);
                    bystanderAvatar.SetActive(true);
                    transform.localEulerAngles = new Vector3(0, bystanderEulerYAxis, 0);

                    if (isInFOV)
                    {
                        transform.position = new Vector3(FOVPos.transform.position.x, bystanderTracker.transform.position.y, FOVPos.transform.position.z);
                        // bystanderAvatar.transform.eulerAngles = new Vector3(0, (bystanderYAxis + 50), 0);
                        bystanderAvatar.transform.eulerAngles = new Vector3(0, bystanderEulerYAxis + ((bystanderEulerYAxis * (90 + angleinFOV) / 90) - bystanderEulerYAxis), 0);
                    }

                    if (isSeatedAndInFOV)
                    {
                        // Avatar's position = bystander's seating position
                        transform.position = bystanderTracker.transform.position;
                        //  arrowImage.enabled = false;
                    }
                }
                else
                {
                    // TODO: Is the avatar shown when the bystander is at an angle greater than 90 degrees towards the VR user?
                    // If No
                    // no Avatar
                    bystanderAnim.SetBool("isInteracting", false);
                    bystanderAvatar.SetActive(false);

                    // If yes
                    //bystanderAnim.SetBool("isInteracting", false);
                    //bystanderAvatar.SetActive(true);
                }
            }
            if (isMixedSetting)
            {
                //  CRITICAL ZONE: 30-0 degrees to the VR user
                if (bystanderEulerYAxis >= 60 && bystanderEulerYAxis < 100) // 100 <- 90
                {
                    if (!inCriticalZone)
                    {
                        BystanderEnterCriticalZone(); // enter 30 degrees
                        BystanderMoveZone("Enter the ciritical zone");
                        inCriticalZone = true;
                    }

                    fromCriticalSection = true;

                    // Animation for the 3d-avatar
                    if (doInteraction)
                        bystanderAnim.SetBool("isInteracting", true);
                    else
                        bystanderAnim.SetBool("isInteracting", false);

                    // presenceAnimojiBoard.transform.position = new Vector3(Camera.main.transform.position.x - 0.4f, presenceAnimojiBoard.transform.position.y - 0.2f, presenceAnimojiBoard.transform.position.z);

                    // Avatar is still outside the FOV of VR user
                    if (mainCameraYAxis >= 320 || (mainCameraYAxis > 0 && mainCameraYAxis <= 90))
                    {
                        if (isInFOV)
                        {
                            bystanderAvatar.SetActive(true);
                            transform.position = new Vector3(FOVPos.transform.position.x, bystanderTracker.transform.position.y, FOVPos.transform.position.z);
                            bystanderAvatar.transform.eulerAngles = new Vector3(0, bystanderEulerYAxis + ((bystanderEulerYAxis * (90 + angleinFOV) / 90) - bystanderEulerYAxis), 0);
                        }

                        bystanderAvatar.SetActive(false);

                        // presenceAnimojiBoard.transform.position = guidePos.transform.position;
                        // 

                        timeElapsedForGuiding += Time.deltaTime;

                        if (timeElapsedForGuiding < lerpDurationForAvatar)
                        {
                            //if(doInteraction)
                            //    arrowImage.enabled = true;
                            float t = timeElapsedForGuiding / lerpDurationForAvatar;
                            t = t * t * (3f - 2f * t);
                            presenceAnimojiBoard.transform.position = Vector3.Lerp(
                                    originalPos.transform.position,
                                     guidePos.transform.position,
                                     t);

                            //if (doInteraction)
                            //{
                            //    arrowImage.transform.position = Vector3.Lerp(
                            //        originalArrowPos.transform.position,
                            //        arrowPosition.transform.position,
                            //       t);
                            //}

                        }
                        else
                        {
                            presenceAnimojiBoard.transform.position = guidePos.transform.position;
                            //if (doInteraction)
                            //{
                            //    arrowImage.transform.position = arrowPosition.transform.position;
                            //}
                        }

                        backsideImage.enabled = false;
                        if (doInteraction)
                        {
                            noInteractionFrontImage.enabled = false;
                            yesInteractionFrontImage.enabled = true;
                            yesInteractionFrontImage.transform.localScale = new Vector2(1.5f, 1.5f);
                        }
                        else
                        {
                            yesInteractionFrontImage.enabled = false;
                            noInteractionFrontImage.enabled = true;
                            noInteractionFrontImage.transform.localScale = new Vector2(1.5f, 1.5f);
                            //noInteractionFrontImage.enabled = false;
                            //yesInteractionFrontImage.enabled = true;
                            //yesInteractionFrontImage.transform.localScale = new Vector2(1.5f, 1.5f);
                        }

                    }
                    else if (mainCameraYAxis < 320 && mainCameraYAxis >= 250)
                    {
                        bystanderAvatar.SetActive(true);
                        if (isInFOV)
                        {
                            transform.position = new Vector3(FOVPos.transform.position.x, bystanderTracker.transform.position.y, FOVPos.transform.position.z);
                            // bystanderAvatar.transform.eulerAngles = new Vector3(0, (bystanderYAxis + 50), 0);
                            bystanderAvatar.transform.eulerAngles = new Vector3(0, bystanderEulerYAxis + ((bystanderEulerYAxis * (90 + angleinFOV) / 90) - bystanderEulerYAxis), 0);
                        }

                        transform.position = trackerTrans.position;
                        transform.localEulerAngles = new Vector3(0, bystanderEulerYAxis, 0);

                        noInteractionFrontImage.enabled = false;
                        yesInteractionFrontImage.enabled = false;
                        backsideImage.enabled = false;
                        // arrowImage.enabled = false;
                    }

                }
                else if (bystanderEulerYAxis >= 30 && bystanderEulerYAxis < 60)
                {
                    //bystanderAvatar.SetActive(false);
                    //bystanderAnim.SetBool("isInteracting", false);
                    //presenceAnimojiBoard.transform.position = originalPos.transform.position;
                    //backsideImage.enabled = false;
                    //arrowImage.enabled = false;
                    //if (doInteraction)
                    //{
                    //    //noInteractionFrontImage.enabled = false;
                    //    //yesInteractionFrontImage.enabled = true;
                    //    //yesInteractionFrontImage.transform.localScale = new Vector2(1f, 1f);
                    //    yesInteractionFrontImage.enabled = false;
                    //    noInteractionFrontImage.enabled = true;
                    //    noInteractionFrontImage.transform.localScale = new Vector2(1f, 1f);
                    //}
                    //else
                    //{
                    //    yesInteractionFrontImage.enabled = false;
                    //    noInteractionFrontImage.enabled = true;
                    //    noInteractionFrontImage.transform.localScale = new Vector2(1f, 1f);
                    //}

                    if (mainCameraYAxis >= 320 || (mainCameraYAxis > 0 && mainCameraYAxis <= 90))
                    {

                        bystanderAvatar.SetActive(false);
                        bystanderAnim.SetBool("isInteracting", false);
                        presenceAnimojiBoard.transform.position = originalPos.transform.position;

                        if (fromCriticalSection)
                        {
                            backsideImage.enabled = true;
                            //arrowImage.enabled = false;
                            yesInteractionFrontImage.enabled = false;
                            noInteractionFrontImage.enabled = false;
                        }
                        else
                        {
                            backsideImage.enabled = false;
                            //  arrowImage.enabled = false;
                            if (doInteraction)
                            {
                                //noInteractionFrontImage.enabled = false;
                                //yesInteractionFrontImage.enabled = true;
                                //yesInteractionFrontImage.transform.localScale = new Vector2(1f, 1f);
                                yesInteractionFrontImage.enabled = false;
                                noInteractionFrontImage.enabled = true;
                                noInteractionFrontImage.transform.localScale = new Vector2(1f, 1f);
                            }
                            else
                            {
                                yesInteractionFrontImage.enabled = false;
                                noInteractionFrontImage.enabled = true;
                                noInteractionFrontImage.transform.localScale = new Vector2(1f, 1f);
                            }
                        }
                    }
                    else if (mainCameraYAxis < 320 && mainCameraYAxis >= 250)
                    {
                        bystanderAvatar.SetActive(true);
                        bystanderAnim.SetBool("isInteracting", false);

                        transform.position = trackerTrans.position;
                        transform.localEulerAngles = new Vector3(0, bystanderEulerYAxis, 0);

                        noInteractionFrontImage.enabled = false;
                        yesInteractionFrontImage.enabled = false;
                        backsideImage.enabled = false;
                        //  arrowImage.enabled = false;
                    }
                }
                else if (bystanderEulerYAxis < 30 && bystanderEulerYAxis >= 10)
                {
                    fromCriticalSection = false;
                    // backside Animoji
                    //bystanderAvatar.SetActive(false);
                    //bystanderAnim.SetBool("isInteracting", false);
                    //presenceAnimojiBoard.transform.position = originalPos.transform.position;
                    //backsideImage.enabled = true;
                    //noInteractionFrontImage.enabled = false;
                    //yesInteractionFrontImage.enabled = false;
                    //arrowImage.enabled = false;

                    // The bystander's avatar is outside the VR user's FOV
                    if (mainCameraYAxis >= 320 || (mainCameraYAxis > 0 && mainCameraYAxis <= 90))
                    {
                        bystanderAvatar.SetActive(false);
                        bystanderAnim.SetBool("isInteracting", false);
                        presenceAnimojiBoard.transform.position = originalPos.transform.position;
                        backsideImage.enabled = true;
                        noInteractionFrontImage.enabled = false;
                        yesInteractionFrontImage.enabled = false;
                        //arrowImage.enabled = false;
                    }
                    else if (mainCameraYAxis < 320 && mainCameraYAxis >= 250)
                    {
                        bystanderAvatar.SetActive(true);
                        bystanderAnim.SetBool("isInteracting", false);

                        transform.position = trackerTrans.position;
                        transform.localEulerAngles = new Vector3(0, bystanderEulerYAxis, 0);

                        noInteractionFrontImage.enabled = false;
                        yesInteractionFrontImage.enabled = false;
                        backsideImage.enabled = false;
                        // arrowImage.enabled = false;
                    }
                }
                else
                {
                    fromCriticalSection = false;
                    bystanderAvatar.SetActive(false);
                    bystanderAnim.SetBool("isInteracting", false);
                    presenceAnimojiBoard.transform.position = originalPos.transform.position;
                    backsideImage.enabled = false;
                    yesInteractionFrontImage.enabled = false;
                    noInteractionFrontImage.enabled = false;
                    //arrowImage.enabled = false;
                }
            }

        }
        /******************************************************************
                  * To the right side of the VR user
                  */

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
                    if (isInFOV || isSeatedAndInFOV)
                    {
                        transform.position = FOVPos.transform.position;
                    }


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
                    if (isInFOV)
                    {
                        transform.position = FOVPos.transform.position;
                    }

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

    private void BystanderMoveZone(string state)
    {
        bsgameManager.SetTimeStampForAvatarInCriticalZone();
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

    public void GuideToBystander()
    {
        transform.position = new Vector3(FOVPos.transform.position.x, bystanderTracker.transform.position.y, FOVPos.transform.position.z);
        //transform.position = new Vector3(middlePos.transform.position.x, bystanderTracker.transform.position.y, middlePos.transform.position.z);
        isGuidingToSeated = true;
        if (isguided)
        {
            Invoke("SetGuided", 2f);
        }

    }

    public void SetGuided()
    {
        isguided = true;
        transform.position = Vector3.Lerp(transform.position, new Vector3(middlePos.transform.position.x, bystanderTracker.transform.position.y, middlePos.transform.position.z), Time.deltaTime * 2);
        Invoke("GuideToBystander", 2f);
    }


}
