using System;
using System.Collections;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class BeatSaberGameManager : MonoBehaviour
{
    [Header("AUDIO")]
    public AudioSource audioSource;
    public AudioSource bgAudioSource;
    public AudioClip gameMusic;
    public AudioSource lobbyMusic;
    public AudioClip rightSlice;
    public AudioClip missSound;
    public AudioClip sliceSound;
    public AudioClip cheerSound;

    [Header("EFFECT")]
    public GameObject rightSliceEffectPrefab;
    public GameObject wrongSliceEffectPrefab;
    public GameObject cheerEffect;
    public GameObject blueEffect;
    public GameObject greenEffect;
    public GameObject yellowEffect;

    [Header("GAME UI")]
    public GameObject lobbymenuUI;
    public GameObject practiceLobbyMenuUI;
    public GameObject notificationUI;
    public GameObject timeUI;
    public GameObject scoreUI;
    public GameObject instructionUI;
    public GameObject surveryUI;

    public TMP_Text gameScoreText;
    public TMP_Text gameTimeText;
    // public GameObject interactionUI;
    public TMP_Text instructionText;
    public TMP_Text trialInstructionText;

    public TMP_Text notificationText;
    // public Image notificationBGImage;
    // public List<Image> notificationCheerImages;
   // public Button TrialStartBtn;

    public GameObject saberObject;

    [Header("TIME MANAGEMENT")]
    public float getReadyTime;
   // private float bufferingTimeBeforeStartGame = 1f;
    public int totalGameTime;
    public float gameTimerIgnoringPause;

    float gameCountTimer;
    int gameTimer;
    private float timeFromSceneLoading, startTimeForSpawingCubes; // time to show Card images, time to turn backwards again
    float beforeGameTimer = 0f;
    public float BystanderStartTime = 25f;
    public float bystanderInterval = 60f;
    [SerializeField]
    private float BystanderStartTime2, BystanderStartTime3;

    // public Button pauseBtn;
    [SerializeField]
    private float pausedTime, identificationTime, eyeFocusTime ;
    bool gamePaused;

    [Header("TRIAL TIME MNG)")]
    public float getReadyTimeForTrial;

    [Header("SCORE")]
    [SerializeField]
    private int score;
    private bool canStartGame;
    private bool canPauseGame;
    private bool canStartTrial;

    [Header("TRACKER")]
    public BSRotateTracker bysTracker;
    public XRInteractorLineVisual lineVisual;
    [Header("PARTICIPANT")]
    public string participantID;

    private int randomNumForEffect;
    private bool bystanderInteract;
    public BSLogManager logManager;
    public bool isPracticeGame;
    public bool isEndScene;
    private bool recordScore;
    private bool recordMaxMin;
    private bool recordStartAxis;
    int currentSceneIndex;
    private bool askSpawnCubes;

    public CubeSpawner cubeSpawner;
    public BSPauseController pauseController;
    public GameObject[] cubes;
    public GameObject[] trialCubes;
    public CubeSpawner trialCubeSpawner;

    private bool trialOncePaused;
    private bool trialOnceResumed;
    private string instructionMsg;

    [SerializeField]
    private Vector3 maincameraAxis;
    [SerializeField]
    private float mainCameraYAxis, mainCameraXAxis, minYAxis, maxYAxis, minXAxis, maxXAxis;
    public bool oneInteruption;
    private bool bystanderCanHearAnswer;

    public bool CanStartGame { get => canStartGame; set => canStartGame = value; }
    public bool BystanderInteract { get => bystanderInteract; set => bystanderInteract = value; }
    public bool CanPauseGame { get => canPauseGame; set => canPauseGame = value; }
    public float GameCountTimer { get => gameCountTimer; set => gameCountTimer = value; }
    public bool BystanderCanHearAnswer { get => bystanderCanHearAnswer; set => bystanderCanHearAnswer = value; }
    public bool CanStartTrial { get => canStartTrial; set => canStartTrial = value; }

    private void Awake()
    {       
        gameTimer = totalGameTime;
        score = 0;

        // Game Notification
        notificationUI.gameObject.SetActive(false);
        surveryUI.gameObject.SetActive(false);
        timeUI.gameObject.SetActive(false);
        scoreUI.gameObject.SetActive(false);
        if (!isPracticeGame)
        {
            instructionUI.gameObject.SetActive(false);
            practiceLobbyMenuUI.gameObject.SetActive(false);
        }
        else // Practice game
        {
            lobbymenuUI.gameObject.SetActive(false);
           // TrialStartBtn.enabled = false;
        }
      //  instructionUI.gameObject.SetActive(false);

        // canMusicPlay = true;
        //  interactionUI.SetActive(false);
        // pauseBtn.gameObject.SetActive(false);

        if (participantID == "" || participantID == null)
        {
            participantID = "ID not assigned";
        }
    }

    private void Start()
    {
        mainCameraYAxis = Camera.main.transform.eulerAngles.y;
        mainCameraXAxis = Camera.main.transform.eulerAngles.x;
        maincameraAxis = Camera.main.transform.eulerAngles;
        maxXAxis = mainCameraXAxis;
        maxYAxis = mainCameraYAxis;
        minYAxis = mainCameraXAxis;
        BystanderStartTime2 = BystanderStartTime + bystanderInterval;
        BystanderStartTime3 = BystanderStartTime2 + bystanderInterval;
        saberObject.SetActive(false);

        if (isPracticeGame)
        {
            instructionText.text = "";
            StartCoroutine(Instructions());
        }
    }

    private void FixedUpdate()
    {
        if (CanStartGame)
        {
            if (!recordStartAxis)
            {
                if (Camera.main.transform.localEulerAngles.y > 180 && Camera.main.transform.localEulerAngles.y <= 360)
                {
                    mainCameraYAxis = 360f - Camera.main.transform.localEulerAngles.y;
                }
                if (Camera.main.transform.localEulerAngles.y >= 0 && Camera.main.transform.localEulerAngles.y < 180)
                {
                    mainCameraYAxis = Camera.main.transform.localEulerAngles.y * -1f;
                }
                if (Camera.main.transform.localEulerAngles.x > 180 && Camera.main.transform.localEulerAngles.x <= 360)
                {
                    mainCameraXAxis = 360f - Camera.main.transform.localEulerAngles.x;
                }
                if (Camera.main.transform.localEulerAngles.x >= 0 && Camera.main.transform.localEulerAngles.x < 180)
                {
                    mainCameraXAxis = Camera.main.transform.localEulerAngles.x * -1f;
                }

                maincameraAxis = Camera.main.transform.localEulerAngles;
                SetCameraAxisAtBeginning();
                recordStartAxis = true;
            }
            
            // Time Before Game Start
            if (Time.time >= timeFromSceneLoading && Time.time <= startTimeForSpawingCubes) // Showing Time
            {
                beforeGameTimer += Time.fixedDeltaTime;
                // gameTimeText.text = Math.Round(getReadyTime - beforeGameTimer).ToString();
                gameTimeText.text = ConvertToMinAndSeconds(getReadyTime - beforeGameTimer);
            }
            // GAME TIME
            else if (Time.time > startTimeForSpawingCubes && GameCountTimer <= totalGameTime) // During the Game
            {
                if (Camera.main.transform.localEulerAngles.y > 180 && Camera.main.transform.localEulerAngles.y <= 360)
                {
                    mainCameraYAxis = 360f - Camera.main.transform.localEulerAngles.y;
                }
                if (Camera.main.transform.localEulerAngles.y >= 0 && Camera.main.transform.localEulerAngles.y < 180)
                {
                    mainCameraYAxis = Camera.main.transform.localEulerAngles.y * -1f;
                }

                if (minYAxis > mainCameraYAxis)
                    minYAxis = mainCameraYAxis;
                if (maxYAxis < mainCameraYAxis)
                    maxYAxis = mainCameraYAxis;


                if (Camera.main.transform.localEulerAngles.x > 180 && Camera.main.transform.localEulerAngles.x <= 360)
                {
                    mainCameraXAxis = 360f - Camera.main.transform.localEulerAngles.x;
                }
                if (Camera.main.transform.localEulerAngles.x >= 0 && Camera.main.transform.localEulerAngles.x < 180)
                {
                    mainCameraXAxis = Camera.main.transform.localEulerAngles.x * -1f;
                }

                if (minXAxis > mainCameraXAxis)
                    minXAxis = mainCameraXAxis;
                if (maxXAxis < mainCameraXAxis)
                    maxXAxis = mainCameraXAxis;

                gameTimerIgnoringPause += Time.fixedDeltaTime;

                if (!gamePaused)
                {
                    GameCountTimer += Time.fixedDeltaTime; 
                   // gameTimeText.text = Math.Round(gameTimer - GameCountTimer).ToString(); // gameTimer - Math.Round(gameCountTimer)
                    gameTimeText.text = ConvertToMinAndSeconds(gameTimer - GameCountTimer);
                   
                    if(askSpawnCubes == false)
                    {
                        SpawnCubes();
                        askSpawnCubes = true;
                    }
                    
                    if (Math.Round(GameCountTimer) == totalGameTime)
                    {
                        cubeSpawner.CanSpawn = false;
                        StopRayInteractoin();
                        EndGame();
                    }
                }
                else
                {
                   // gameTimeText.text = Math.Round(gameTimer - GameCountTimer).ToString();
                    gameTimeText.text = ConvertToMinAndSeconds(gameTimer - GameCountTimer);
                }
            }
        }

        if (isPracticeGame && canStartTrial)
        {
            // Time Before Game Start
            if (Time.time >= timeFromSceneLoading && Time.time <= startTimeForSpawingCubes) // Showing Time
            {
                Debug.Log("Practice is called " + startTimeForSpawingCubes);
                beforeGameTimer += Time.fixedDeltaTime;
                // gameTimeText.text = Math.Round(getReadyTime - beforeGameTimer).ToString();
                gameTimeText.text = ConvertToMinAndSeconds(getReadyTimeForTrial - beforeGameTimer);
            }
            // GAME TIME
            else if (Time.time > startTimeForSpawingCubes && GameCountTimer <= totalGameTime) // During the Game
            {
                Debug.Log("Practice is called 2");
                gameTimerIgnoringPause += Time.fixedDeltaTime;

                if (!gamePaused)
                {
                    GameCountTimer += Time.fixedDeltaTime;
                    gameTimeText.text = ConvertToMinAndSeconds(gameTimer - GameCountTimer);

                    if (askSpawnCubes == false)
                    {
                        SpawnCubes();
                        askSpawnCubes = true;
                    }

                    if (Math.Round(GameCountTimer) == totalGameTime)
                    {
                        trialCubeSpawner.CanSpawn = false;
                        StopRayInteractoin();
                        EndGame();
                    }
                }
                else
                {
                    // gameTimeText.text = Math.Round(gameTimer - GameCountTimer).ToString();
                    gameTimeText.text = ConvertToMinAndSeconds(gameTimer - GameCountTimer);
                }
            }
        }
    }

    public void StartTrial()
    {
        Debug.Log("Click Trial");
      //  CanStartTrial = true;
      //  timeFromSceneLoading = Time.time;
      //  startTimeForSpawingCubes = timeFromSceneLoading + getReadyTimeForTrial;
        practiceLobbyMenuUI.SetActive(false);
        notificationUI.SetActive(true);
        saberObject.SetActive(true);
        StartCoroutine(InstructionsForCubeSlice());
        timeUI.SetActive(true);
    }

    public void StartGame()
    {
        CanStartGame = true;
        timeFromSceneLoading = Time.time;
        startTimeForSpawingCubes = timeFromSceneLoading + getReadyTime;
        Destroy(lobbymenuUI);

        lobbyMusic.Stop();
        bgAudioSource.Play();

        saberObject.SetActive(true);

        scoreUI.SetActive(true);
        timeUI.SetActive(true);
        instructionUI.SetActive(true);

        currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        // currentLevelIndex = 0;
        string currentSceneName = SceneManager.GetActiveScene().name;
        logManager.WriteToLogFile("Study Order: " + currentSceneIndex + " , name: " + currentSceneName);
       // mainCameraYAxis
        logManager.WriteToLogFile("Head Rotation: ");
    }

    public void SetTimeStampForAvatarInCriticalZone()
    {
        // string curDateTime = GetCurrentTime();
        Debug.Log("Bystander Interaction (Enter 30-0 d Zone): " + (float)Math.Round(gameTimerIgnoringPause));
        logManager.WriteToLogFile("Bystander Interaction (Enter 30-0 d Zone): " + (float)Math.Round(gameTimerIgnoringPause));
    }

    public void SetTimeStampForAvatarInCriticalZoneWithMessage(string state)
    {
        Debug.Log("Bystander Interaction: " + state + " " + (float)Math.Round(gameTimerIgnoringPause));
        logManager.WriteToLogFile("Bystander " + state + ": " + (float)Math.Round(gameTimerIgnoringPause));
    }

    private void SetCameraAxisAtBeginning()
    {
        logManager.WriteToLogFile("Start Y-Axis: " + mainCameraYAxis + " X-Axis: " + mainCameraXAxis + " (" + maincameraAxis + ")");
    }

    public void SpawnCubes() {
        if (!isPracticeGame)
        {
            cubeSpawner.CanSpawn = true;

            //   notificationBGImage.enabled = false;
            notificationText.enabled = false;
            instructionText.text = "";

            gameScoreText.text = "0";
            // Vector3 backAngles = new Vector3(0, 180, 0);

            CanPauseGame = true;
            //isFront = false;
            Invoke(nameof(BystanderStart), time: BystanderStartTime);
            if (!oneInteruption)
            {
                Invoke(nameof(BystanderStart), time: BystanderStartTime2);
                Invoke(nameof(BystanderStart), time: BystanderStartTime3);
            }
            // interactionUI.SetActive(true);
        }
        else
        {
            trialCubeSpawner.CanSpawn = true;

            //   notificationBGImage.enabled = false;
          //  notificationText.enabled = false;
           // instructionText.text = "";

            gameScoreText.text = "0";
            // Vector3 backAngles = new Vector3(0, 180, 0);

            CanPauseGame = true;
        }
    }

    public void BystanderStart()
    {
        gameTimeText.text = "";
        bysTracker.IsHeadingToPlayer = true;
        BystanderInteract = true;
        pauseController.OncePausedInSession = false;
        // logManager
        bystanderCanHearAnswer = true;
    }

    public void BystanderEnd()
    {
        BystanderInteract = false;   
    }
 
    public void CubeSliced(GameObject cube)
    {
      //  Debug.Log(cube.name + " called the Method");
        if(score % 8 == 0 && score > 0)
        {
            //audioSource.PlayOneShot(cheerSound);
            //Instantiate(cheerEffect, cube.transform.position, Quaternion.identity);
        }
        else
        {
            audioSource.PlayOneShot(rightSlice);
        }
      
        if (cube.name.Contains("Blue"))
        {
            audioSource.PlayOneShot(sliceSound);
          
            Instantiate(blueEffect, cube.transform.position, Quaternion.identity);
            score += 1;
            gameScoreText.text = score.ToString();
        } 
        else if (cube.name.Contains("Green"))
        {
            audioSource.PlayOneShot(sliceSound);
            
            Instantiate(greenEffect, cube.transform.position, Quaternion.identity);
            score += 1;
            gameScoreText.text = score.ToString();
        }
       else if (cube.name.Contains("YellowCube"))
        {
            audioSource.PlayOneShot(sliceSound);
            Instantiate(yellowEffect, cube.transform.position, Quaternion.identity);
            score += 1;
            gameScoreText.text = score.ToString();            
        }
    }

    public void PauseGame()
    {
        if (!gamePaused)
        {
            if (!isPracticeGame)
            {
                gamePaused = true;
                pausedTime = (float)Math.Round(GameCountTimer);
                identificationTime = (float)Math.Round(gameTimerIgnoringPause);
                logManager.WriteToLogFile("Identification (Paused) Time: " + identificationTime);
                cubeSpawner.CanSpawn = false;
                cubeSpawner.StopMoving = true;
                cubes = GameObject.FindGameObjectsWithTag("Cube");
                foreach (GameObject cube in cubes)
                {
                    // Debug.Log(cube.name);
                    cube.GetComponent<Cube>().StopMove();
                }
                StopRayInteractoin();
            }
            // Practice Game
            else
            {
                gamePaused = true;
               // pausedTime = (float)Math.Round(GameCountTimer);
               // identificationTime = (float)Math.Round(gameTimerIgnoringPause);
               // logManager.WriteToLogFile("Identification (Paused) Time: " + identificationTime);
                trialCubeSpawner.CanSpawn = false;
                trialCubeSpawner.StopMoving = true;
                trialCubes = GameObject.FindGameObjectsWithTag("Cube");
                foreach (GameObject cube in trialCubes)
                {
                    cube.GetComponent<Cube>().StopMove();
                }
                StopRayInteractoin();
                if (!trialOncePaused)
                {
                    instructionMsg = "You paused the game.\n You can resume the game by pressing the touchpad again!";
                    trialOncePaused = true;
                }
                else
                {
                    instructionMsg = "You paused the game.";
                }

                notificationText.text = instructionMsg;
              //  StartCoroutine(InstructionForPause());
                // notificationUI.GetComponentsInChildren<RawImage>()[0].enabled = true;
                //StartCoroutine(InstructionForPause());              
            }        
        }
        else
        {
            if (!isPracticeGame)
            {
                gamePaused = false;
                logManager.WriteToLogFile("Resume Time: " + (float)Math.Round(gameTimerIgnoringPause));
                cubeSpawner.CanSpawn = true;
                cubeSpawner.StopMoving = false;
                cubes = GameObject.FindGameObjectsWithTag("Cube");
                foreach (GameObject cube in cubes)
                {
                    //  Debug.Log(cube.name);
                    cube.GetComponent<Cube>().StartMove();
                }
                StartRayInteraction();
            }
            else
            {
                gamePaused = false;
                logManager.WriteToLogFile("Resume Time: " + (float)Math.Round(gameTimerIgnoringPause));
                trialCubeSpawner.CanSpawn = true;
                trialCubeSpawner.StopMoving = false;
                trialCubes = GameObject.FindGameObjectsWithTag("Cube");
                foreach (GameObject cube in trialCubes)
                {
                    //  Debug.Log(cube.name);
                    cube.GetComponent<Cube>().StartMove();
                }
                if (!trialOnceResumed)
                {
                    instructionMsg = "You resumed the game! \n You can play the game and try to pause and resume game again!";
                    trialOnceResumed = true;
                }
                else
                {
                    instructionMsg = "You resumed the game.";
                }

                instructionMsg = 
                notificationText.text = instructionMsg;
                notificationUI.GetComponentsInChildren<RawImage>()[0].enabled = false;
                // notificationUI.GetComponentsInChildren<RawImage>()[0].enabled = false;
                StartCoroutine(InstructionForPause());
                
                StartRayInteraction();
            }        
        }
    }

    public void EndGame()
    {
        if (!isPracticeGame)
        {
            notificationUI.SetActive(true);
            notificationText.enabled = true;
            bystanderInteract = false;
            CanPauseGame = false;

            cubeSpawner.CanSpawn = false;
            saberObject.SetActive(false);
            notificationUI.GetComponentsInChildren<RawImage>()[0].enabled = false;
            //
            scoreUI.SetActive(false);

            notificationText.text = "BRAVO!\nYOUR SCORE IS " + score + "!";

            gameTimeText.text = ConvertToMinAndSeconds(0);

            if (!recordScore)
            {
                logManager.WriteToLogFile("Score: " + score);
                logManager.WriteToLogFile("==============================");
                recordScore = true;
            }

            if (!recordMaxMin)
            {
                logManager.WriteToLogFile("Max Y Axis (Toward Bystander): " + maxYAxis);
                logManager.WriteToLogFile("Min Y Axis (Against Bystander): " + minYAxis);
                recordMaxMin = true;
            }

            Invoke(nameof(GoToNextLevel), 4f);
            // Invoke(nameof(DoSurvey), 1f);
        }
        else
        {
            CanPauseGame = false;
            trialCubeSpawner.CanSpawn = false;
            saberObject.SetActive(false);
            notificationText.text = "Your Trial Game is finised!";
            gameTimeText.text = ConvertToMinAndSeconds(0);
            Invoke(nameof(GoToNextLevel), 4f);
        }
    }

    public void GoSurvey()
    {
        surveryUI.SetActive(true);
    }
    public void DoSurvey()
    { 
        surveryUI.SetActive(true);
        lineVisual.enabled = true;
        
        //lineVisual.gameObject.SetActive(true);
        notificationUI.SetActive(false);
      //  notificationBGImage.enabled = false;
        notificationText.enabled = false;
        // menuUICanvas.SetActive(false);
    }

    public void GoToNextLevel() {
        BSLevelManager levelManager = FindObjectOfType<BSLevelManager>();
        levelManager.LoadNextLevel();
    }

    public IEnumerator ShowRandomImage()
    {
        while (true)
        {   
            yield return new WaitForSeconds(1);
           // notificationCheerImages[randomNumForEffect].enabled = false;
        }
    }

    void StopRayInteractoin()
    {
         lineVisual.enabled = false;
    }

    void StartRayInteraction()
    {
        lineVisual.enabled = true;
    }

    public void SubmitSurvey()
    {
        Debug.Log("submit survey");
    }

    public void EyeFocused(bool focus)
    {
        eyeFocusTime = (float)Math.Round(gameTimerIgnoringPause);
        if (focus)
        { 
            logManager.WriteToLogFile("Receive FOCUS: " + eyeFocusTime);
        }
        else
        {
            logManager.WriteToLogFile("LOST FOCUS: " + eyeFocusTime);
        }  
    }

    public string GetCurrentTime()
    {
        DateTime localDate = DateTime.Now;
        string cultureName = "de-DE"; // de-DE  en-GB en-US
        var culture = new CultureInfo(cultureName);
        string name = localDate.ToString(culture);

        return name;
    }

    private string ConvertToMinAndSeconds(float totalTimeInSeconds)
    {
        string timeText = Mathf.Floor(totalTimeInSeconds / 60).ToString("00") + ":" + Mathf.FloorToInt(totalTimeInSeconds % 60).ToString("00");
        return timeText;
    }

    public void MissCube()
    {
       // Debug.Log("missed ball");
        audioSource.PlayOneShot(missSound);
    }

    IEnumerator Instructions()
    {
        // Debug.Log(practiceLobbyMenuUI.GetComponentsInChildren<RawImage>()[0].name);
        practiceLobbyMenuUI.GetComponentsInChildren<RawImage>()[0].enabled = false;
        instructionMsg = "Welcome to our user study!";
        trialInstructionText.text = instructionMsg;
        yield return new WaitForSeconds(7);
        instructionMsg = "Please point and click the Start button below when you're ready.\n You can press the trigger button of the controller.";
        trialInstructionText.text = instructionMsg;
        practiceLobbyMenuUI.GetComponentsInChildren<RawImage>()[0].enabled = true;
       // TrialStartBtn.enabled = true;
        yield return new WaitForSeconds(5);
    }

    IEnumerator InstructionsForCubeSlice()
    {
        instructionMsg = "You hold a lightsaber now! \n Slash the blocks by hitting the colored part with your saber! \n You don't have to click any button on the controller";
        notificationText.text = instructionMsg;
        notificationUI.GetComponentsInChildren<RawImage>()[0].enabled = false;
        yield return new WaitForSeconds(9);
        CanStartTrial = true;
        timeFromSceneLoading = Time.time;
        startTimeForSpawingCubes = timeFromSceneLoading + getReadyTimeForTrial;
        notificationText.text = "";
        yield return new WaitForSeconds(20);
        notificationText.text = "Try to Pause the game.\n You can press the touchpad on the controller";
        notificationUI.GetComponentsInChildren<RawImage>()[0].enabled = true;
        yield return new WaitForSeconds(5);
    }

    IEnumerator InstructionForPause()
    {
        instructionMsg = "";
        notificationText.text = instructionMsg;
        notificationUI.GetComponentsInChildren<RawImage>()[0].enabled = false;
        yield return new WaitForSeconds(10);
        notificationText.text = "Try to Pause the game.\n You can press the touchpad on the controller";
        notificationUI.GetComponentsInChildren<RawImage>()[0].enabled = true;
        yield return new WaitForSeconds(3);
    }
}