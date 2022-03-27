using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BeatSaberBystanderAvatar : MonoBehaviour
{
    public GameObject bystanderTracker;
    public BeatSaberGameManager bsgameManager;
    public Transform tracker;
    private float bystanderEulerYAxis;  // bystander's euler y-axis
    private float bystanderRotationOffset = 0;

    public GameObject bystanderAvatar;
    public Animator bystanderAnim;
    public bool doInteraction;

    [Header("GOs for Animoji")]
    // Variables for Animoji Setting
    public GameObject presenceAnimojiBoard;
    public RawImage yesInteractionFrontImage;
    public RawImage noInteractionFrontImage;
    public RawImage backsideImage;
    // public RawImage arrowImage;
    // public GameObject arrowPosition;
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

    private float mainCameraYAxis;
    public bool isGuidingFromFOVToSeated;
    private bool isGudingFromSeatedToFov;
    public bool isguided;

    private float angleinFOV = 50f;

    private float guidingLength;
    private float guidingSpeed = 1.0f;
    private float timeElapsedForGuidingToFOV, timeElapsedForGuidingToSeated;
    public float lerpDurationForAvatar = 3f;
    public float fadeTime = 2f;
    float timeElapsedForTransparency = 0f;
    private bool guideSuccessOnce;

    [SerializeField]
    private bool fromCriticalSection, inCriticalDegreesSection;

    public BSLogManager logManager;
    // Start is called before the first frame update
    void Start()
    {
        doInteraction = true;
        guidingPos = GetComponent<Transform>(); // For Avatar Setting (FOV -> Seated)
        bystanderAnim.SetBool("isInteracting", false);

        // Default setting: avatar setting
        if (!(isAnimojiSetting || isMixedSetting || isAvatarSetting)
            && !bsgameManager.isPracticeGame)
            isAvatarSetting = true;
        // Default setting: avatar-FOV 
        if (isAvatarSetting && !(isSeated || isInFOV || isSeatedAndInFOV))
            isSeatedAndInFOV = true;

        sitToLeft = true;
        yesInteractionFrontImage.enabled = false;
        noInteractionFrontImage.enabled = false;
        backsideImage.enabled = false;
        bystanderAvatar.SetActive(false);
        //  arrowImage.enabled = false;

        // bystanderYAxis = bystanderTracker.transform.eulerAngles.y;
        bystanderEulerYAxis = tracker.eulerAngles.y;
        bystanderRotationOffset = bystanderEulerYAxis - 0f;
        mainCameraYAxis = Camera.main.transform.eulerAngles.y;

        guidingLength = Vector3.Distance(new Vector3(FOVPos.transform.position.x, bystanderTracker.transform.position.y, FOVPos.transform.position.z), tracker.position);
    }

    void Update()
    {
        //transform.position = bystanderTracker.transform.position;   // sync the avatar's postion with the tracker's position
        transform.position = tracker.position;
        //  bystanderYAxis = bystanderTracker.transform.eulerAngles.y;
        bystanderEulerYAxis = tracker.eulerAngles.y;
        mainCameraYAxis = Camera.main.transform.eulerAngles.y;

        // For animoji guiding
        middlePos.transform.position = new Vector3(
            (originalPos.transform.position.x + tracker.position.x) / 2,
            (originalPos.transform.position.y + tracker.position.y) / 2,
            (originalPos.transform.position.z + tracker.position.z) / 2);

        // For avatar guiding to seated pos
        guidingPos.position = new Vector3(
            (FOVPos.transform.position.x + tracker.position.x) / 2,
            (FOVPos.transform.position.y + tracker.position.y) / 2,
            (FOVPos.transform.position.z + tracker.position.z) / 2);

        // The bystander is sitting to the left of the VR Player.
        if (sitToLeft)
        {
            /***************************************************
             ** CRITICAL ZONE: 30-0 degrees to the VR user (90: toward the front seat)
             ** The bystander is heading towards the VR user
             *  (F)
             *  (B) /-> (V)
             *  B: 30 >= d >= 0 (-10)
             ****************************************************/
            if (bystanderEulerYAxis >= 60 && bystanderEulerYAxis < 100) // 100 <- 90
            {
                fromCriticalSection = true;
                // Check the bystander come from uncritical zone (d > 30)
                if (!inCriticalDegreesSection)
                {
                    BystanderEnterCriticalSection(); // enter 30 degrees
                    inCriticalDegreesSection = true;
                }
                /*
                 * ANIMOJI SETTING in 30-0
                 */
                if (isAnimojiSetting)
                {
                    //  fromCriticalSection = true;
                    backsideImage.enabled = false;

                    if (doInteraction)
                    {
                        // Bigger Animoji with FE
                        noInteractionFrontImage.enabled = false;
                        yesInteractionFrontImage.enabled = true;
                        yesInteractionFrontImage.transform.localScale = new Vector2(1.5f, 1.5f);
                    }
                    else
                    {
                        // Bigger Animoji without FE 
                        yesInteractionFrontImage.enabled = false;
                        noInteractionFrontImage.enabled = true;
                        noInteractionFrontImage.transform.localScale = new Vector2(1.5f, 1.5f);
                    }
                }
                /*
                 * AVATAR SETTING in 30-0
                 */
                else if (isAvatarSetting)
                {
                    // fromCriticalSection = true;
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

                        //if (bystanderYAxis <= 0)
                        //    bystanderAvatar.transform.eulerAngles = new Vector3(0, bystanderYAxis, 0);
                        //else
                        //    bystanderAvatar.transform.eulerAngles = new Vector3(0, ((bystanderYAxis * (140 / 90)) - bystanderYAxis), 0);
                    }

                    if (isSeatedAndInFOV)
                    {
                        // The bystander is inside VR user's FOV
                        if (mainCameraYAxis >= 250 && mainCameraYAxis <= 310)
                        {
                            if (!guideSuccessOnce)
                            {
                                Debug.Log("in my eyes");
                                transform.position = bystanderTracker.transform.position;
                                bystanderAvatar.transform.eulerAngles = new Vector3(0, bystanderEulerYAxis, 0);
                                // arrowImage.enabled = false;
                                isGuidingFromFOVToSeated = false;
                                // currentMovementTime = 0;
                                // timeElapsedForGuiding = 0;
                                guideSuccessOnce = true;
                            }

                        }
                        //else if(mainCameraYAxis > 310 && mainCameraYAxis <= 315) 
                        //{
                        //    bystanderAvatar.SetActive(false);
                        //    arrowImage.enabled = false;
                        //}
                        else  // The bystander is outside the FOV of the VR user ( 310 < d < 360, ....)
                        {
                            currentMovementTime += Time.deltaTime;


                            if (!isGudingFromSeatedToFov)
                            {
                                Debug.Log("guiding to FOV");
                                timeElapsedForGuidingToFOV += Time.deltaTime;

                                if (timeElapsedForGuidingToFOV < lerpDurationForAvatar)
                                {
                                    float t = timeElapsedForGuidingToFOV / lerpDurationForAvatar;
                                    t = t * t * (3f - 2f * t);
                                    transform.position = Vector3.Lerp(
                                                new Vector3(tracker.position.x, tracker.position.y, tracker.position.z),
                                                new Vector3(FOVPos.transform.position.x, bystanderTracker.transform.position.y, FOVPos.transform.position.z),
                                                t);

                                    bystanderAvatar.transform.rotation = Quaternion.Lerp(
                                        Quaternion.Euler(bystanderAvatar.transform.eulerAngles),
                                        Quaternion.Euler(new Vector3(0, bystanderEulerYAxis + ((bystanderEulerYAxis * (90 + angleinFOV) / 90) - bystanderEulerYAxis), 0)),
                                            t);
                                }
                                else
                                {
                                    transform.position = new Vector3(guidingPosForAV.transform.position.x, tracker.position.y, guidingPosForAV.transform.position.z);
                                    bystanderAvatar.transform.eulerAngles = new Vector3(0, bystanderEulerYAxis + ((bystanderEulerYAxis * (90 + angleinFOV) / 90) - bystanderEulerYAxis), 0);
                                    // new Vector3(guidingPosForAV.transform.position.x, arrowPos.transform.position.y, guidingPosForAV.transform.position.z);
                                }

                                isGudingFromSeatedToFov = true;

                                // avatar in FOV with more rotating angles
                                //   bystanderAvatar.transform.eulerAngles = new Vector3(0, bystanderEulerYAxis + ((bystanderEulerYAxis * (90 + angleinFOV) / 90) - bystanderEulerYAxis), 0);
                                //  transform.position = new Vector3(FOVPos.transform.position.x, tracker.position.y, FOVPos.transform.position.z);
                            }

                            if (!isGuidingFromFOVToSeated && isGudingFromSeatedToFov)
                            {
                                Debug.Log("guiding to SEAT");
                                timeElapsedForGuidingToSeated += Time.deltaTime;

                                if (timeElapsedForGuidingToSeated < lerpDurationForAvatar)
                                {
                                    float t = timeElapsedForGuidingToSeated / lerpDurationForAvatar;
                                    t = t * t * (3f - 2f * t);
                                    transform.position = Vector3.Lerp(
                                                new Vector3(FOVPos.transform.position.x, bystanderTracker.transform.position.y, FOVPos.transform.position.z),
                                                new Vector3(tracker.position.x, tracker.position.y, tracker.position.z),
                                                t);

                                    //  new Vector3(guidingPosForAV.transform.position.x, tracker.position.y, guidingPosForAV.transform.position.z)

                                    bystanderAvatar.transform.rotation = Quaternion.Lerp(
                                        Quaternion.Euler(bystanderAvatar.transform.eulerAngles),
                                        Quaternion.Euler(new Vector3(0, bystanderEulerYAxis + ((bystanderEulerYAxis * (90 + angleinFOV - 10) / 90) - bystanderEulerYAxis), 0)),
                                            t);
                                }
                                else
                                {
                                    transform.position = new Vector3(guidingPosForAV.transform.position.x, tracker.position.y, guidingPosForAV.transform.position.z);
                                    // new Vector3(guidingPosForAV.transform.position.x, arrowPos.transform.position.y, guidingPosForAV.transform.position.z);
                                }
                                isGuidingFromFOVToSeated = true;

                            }

                            if (currentMovementTime > 2f)
                            {
                                isGuidingFromFOVToSeated = true;
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
                else if (isMixedSetting)
                {
                    //   fromCriticalSection = true;                  
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
                        timeElapsedForGuidingToFOV += Time.deltaTime;

                        if (timeElapsedForGuidingToFOV < lerpDurationForAvatar)
                        {
                            float t = timeElapsedForGuidingToFOV / lerpDurationForAvatar;
                            t = t * t * (3f - 2f * t);
                            presenceAnimojiBoard.transform.position = Vector3.Lerp(
                                    originalPos.transform.position,
                                     guidePos.transform.position,
                                     t);
                        }
                        else
                        {
                            presenceAnimojiBoard.transform.position = guidePos.transform.position;
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

                        transform.position = tracker.position;
                        transform.localEulerAngles = new Vector3(0, bystanderEulerYAxis, 0);

                        noInteractionFrontImage.enabled = false;
                        yesInteractionFrontImage.enabled = false;
                        backsideImage.enabled = false;
                    }
                }
            }
            //  (F)
            //   //_
            //  (B) (V)
            // B: 60 >= d > 30
            else if (bystanderEulerYAxis >= 30 && bystanderEulerYAxis < 60)
            {
                timeElapsedForTransparency = 0;
                inCriticalDegreesSection = false;
                if (isAnimojiSetting)
                {
                    float timeElapsedForTransparency = 0f;
                    if (fromCriticalSection)
                    {
                        //
                        timeElapsedForTransparency += Time.deltaTime;

                        if (timeElapsedForTransparency < fadeTime)
                        {

                            float t = timeElapsedForGuidingToFOV / lerpDurationForAvatar;
                            t = t * t * (3f - 2f * t);
                            backsideImage.enabled = true;
                            Color c = backsideImage.color;
                            c.a = Mathf.Clamp01(t);
                            //presenceAnimojiBoard.transform.position = Vector3.Lerp(
                            //        originalPos.transform.position,
                            //         guidePos.transform.position,
                            //         t);
                        }
                        //
                        // backsideImage.enabled = true;
                        yesInteractionFrontImage.enabled = false;
                        noInteractionFrontImage.enabled = false;
                    }
                    else
                    {
                        timeElapsedForTransparency += Time.deltaTime;

                        if (timeElapsedForTransparency < fadeTime)
                        {
                            // Debug.Log("transparency is called2");
                            float t = timeElapsedForGuidingToFOV / lerpDurationForAvatar;
                            t = t * t * (3f - 2f * t);
                            noInteractionFrontImage.enabled = true;
                            Color c = backsideImage.color;
                            c.a = Mathf.Clamp01(t);
                            //presenceAnimojiBoard.transform.position = Vector3.Lerp(
                            //        originalPos.transform.position,
                            //         guidePos.transform.position,
                            //         t);


                        }
                        backsideImage.enabled = false;
                        yesInteractionFrontImage.enabled = false;
                        //noInteractionFrontImage.enabled = true;
                        noInteractionFrontImage.transform.localScale = new Vector2(1f, 1f);
                    }
                }

                if (isAvatarSetting)
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

                if (isMixedSetting)
                {
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
                            if (doInteraction)
                            {
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

                        transform.position = tracker.position;
                        transform.localEulerAngles = new Vector3(0, bystanderEulerYAxis, 0);

                        noInteractionFrontImage.enabled = false;
                        yesInteractionFrontImage.enabled = false;
                        backsideImage.enabled = false;
                    }
                }
            }
            //  (F)
            //   ||/
            //  (B) (V)
            // B: 80 >= d > 60
            else if (bystanderEulerYAxis < 30 && bystanderEulerYAxis >= 10)
            {
                if (isAnimojiSetting)
                {
                    fromCriticalSection = false;
                    // Backside Animoji
                    // backsideImage.enabled = true;
                    yesInteractionFrontImage.enabled = false;
                    noInteractionFrontImage.enabled = false;


                    //
                    timeElapsedForTransparency += Time.deltaTime;

                    if (timeElapsedForTransparency < fadeTime)
                    {
                        float t = timeElapsedForGuidingToFOV / fadeTime;
                        t = t * t * (3f - 2f * t);
                        backsideImage.enabled = true;
                        Color c = backsideImage.color;
                        c.a = 0.2f;
                        // c.a = 1.0f - Mathf.Clamp01(t);
                        backsideImage.color = c;
                        //presenceAnimojiBoard.transform.position = Vector3.Lerp(
                        //        originalPos.transform.position,
                        //         guidePos.transform.position,
                        //         t);

                    }

                    if (isAvatarSetting)
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

                    if (isMixedSetting)
                    {
                        fromCriticalSection = false;

                        // The bystander's avatar is outside the VR user's FOV
                        if (mainCameraYAxis >= 320 || (mainCameraYAxis > 0 && mainCameraYAxis <= 90))
                        {
                            //bystanderAvatar.SetActive(false);
                            //bystanderAnim.SetBool("isInteracting", false);
                            //presenceAnimojiBoard.transform.position = originalPos.transform.position;
                            //backsideImage.enabled = true;
                            //noInteractionFrontImage.enabled = false;
                            //yesInteractionFrontImage.enabled = false;


                            timeElapsedForTransparency += Time.deltaTime;

                            if (timeElapsedForTransparency < fadeTime)
                            {
                                float t = timeElapsedForTransparency / lerpDurationForAvatar;
                                t = t * t * (3f - 2f * t);
                                backsideImage.enabled = true;
                                Color c = backsideImage.color;
                                c.a = Mathf.Clamp01(t);
                                //presenceAnimojiBoard.transform.position = Vector3.Lerp(
                                //        originalPos.transform.position,
                                //         guidePos.transform.position,
                                //         t);


                            }
                            noInteractionFrontImage.enabled = false;
                            yesInteractionFrontImage.enabled = false;
                        }
                        else if (mainCameraYAxis < 320 && mainCameraYAxis >= 250)
                        {
                            bystanderAvatar.SetActive(true);
                            bystanderAnim.SetBool("isInteracting", false);

                            transform.position = tracker.position;
                            transform.localEulerAngles = new Vector3(0, bystanderEulerYAxis, 0);

                            noInteractionFrontImage.enabled = false;
                            yesInteractionFrontImage.enabled = false;
                            backsideImage.enabled = false;
                            // arrowImage.enabled = false;
                        }
                    }
                }
                //  (F)
                //  \|
                //  (B) (V)
                //  B: d > 90
                else
                {
                    if (isAnimojiSetting)
                    {
                        // no Animoji
                        fromCriticalSection = false;
                        backsideImage.enabled = false;
                        yesInteractionFrontImage.enabled = false;
                        noInteractionFrontImage.enabled = false;
                    }

                    if (isAvatarSetting)
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

                    if (isMixedSetting)
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





                        transform.localEulerAngles = new Vector3(0, bystanderEulerYAxis, 0);
                        // Debug.Log(bystanderRotationEulerY);
                        bystanderAvatar.SetActive(true);
                    }

                    if (isMixedSetting)
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
    }

    private void BystanderEnterCriticalSection()
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
        isGuidingFromFOVToSeated = true;
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