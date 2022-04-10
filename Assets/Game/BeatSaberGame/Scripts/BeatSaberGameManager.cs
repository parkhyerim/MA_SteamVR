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
    public AudioSource gameEffectAudioSource;
    public AudioSource bgMusicAudioSource;
    public AudioSource lobbyMusicAudioSource;
    public AudioSource quesitionAudioSource;
    public AudioClip rightSlice;
    public AudioClip missCubeSound;
    public AudioClip sliceSound;
    public AudioClip cheerSound;
    public AudioClip[] questionAudios;

    [Header("Particle EFFECT")]
    public GameObject cheerEffect;
    public GameObject blueEffect;
    public GameObject greenEffect;
    public GameObject yellowEffect;

    [Header("GAME UI")]
    public GameObject lobbyMenuUI;
    public GameObject instructionUI;
    public GameObject timeUI;
    public GameObject scoreUI;
    private TMP_Text gameScoreText;
    private TMP_Text gameTimeText;
    private TMP_Text instructionText;

    [Header("TRIAL_UI")]
    public GameObject trialLobbyMenuUI;
    public GameObject trialInstructionUI;
   // public GameObject instructionUI;
    // public GameObject surveryUI;
    public GameObject trialProcessUI;
    private TMP_Text trialInstructionText;
    private TMP_Text trialLobbyText;
    public GameObject trialStartButton;

    public GameObject[] stopCubes;

    [Header("GAME COMPONENTS")]
    public GameObject saberObject;

    [Header("TIME INPUT")]
    public int totalGameTime;
    public float getReadyTime;
    public float BystanderStartTime = 25f;
    public float bystanderInterval = 60f;
    [Header("TIME INFO.")]
    [SerializeField]
    private float gameTimerIgnoringPause, gameCountTimer;
    int gameTimerToZero;
    private float timeFromSceneLoading, startTimeForSpawingCubes; // time to show Card images, time to turn backwards again
    float beforeGameTimer = 0f;
    [SerializeField]
    private float BystanderStartTime2, BystanderStartTime3, BystanderStartTime4;
    [SerializeField]
    private float pausedTime, identificationTime, eyeFocusTime;

    [Header("TRIAL TIME MNG.")]
    public float getReadyTimeForTrial;

    [Header("SCORE")]
    [SerializeField]
    private int score;

    [Header("BOOLEADN")]
    private bool canStartGame;
    [SerializeField]
    private bool canPauseGame, gamePaused;
    [SerializeField]
    private bool canStartTrial, canPauseTrial;

    [Header("TRACKER")]
    BSRotateTracker bysTracker;
    public XRInteractorLineVisual lineVisual;
    [Header("PARTICIPANT")]
    [SerializeField]
    private string participantID;

    private int randomNumForEffect;
    private bool bystanderInteract;

    public bool isPracticeGame;
    public bool isEndScene;
    private bool recordScore;
    private bool recordMaxMin;
    private bool recordStartAxis;
    int currentSceneIndex;
    private bool askSpawnCubes;
    //Trial
    private bool pauseInstructed;
    private bool askSpawnCubesForTrial;
    private GameObject[] cubes;
    private GameObject[] trialCubes;
    public CubeSpawner cubeSpawner;
    public CubeSpawner trialCubeSpawner;

    private bool trialOncePaused;
    private bool trialOnceResumed;
    private string instructionMsg;
    BSBystanderAvatar bystanderAvatar;
    BSLevelManager levelManager;
    UserStudyManager userstudyManager;
    BSLogManager logManager;
    BSPauseController pauseController;
    HeadMovement headMovement;

    float testTime;
    int checkCounter;
    [SerializeField]
    private float currentYAxis, diffYAxis, previousYAxis;
    private int num;
    private float checkPointTime = 0.0f;
    public float period = 0.2f;
    [SerializeField]
    private Vector3 cameraAxis;

    [SerializeField]
    private Vector3 maincameraAxis, maxYVectorAxis, minYVecotorAxis, maxXVectorAxis, minXVectorAxis;
    [SerializeField]
    private float mainCameraYAxis, mainCameraXAxis, minYAxis, maxYAxis, minXAxis, maxXAxis;
    public bool oneInteruption;
    private bool bystanderCanHearAnswer;
    public int[] audioOrder = { 1, 2, 3 };
    int questionCounter;

    public bool CanStartGame { get => canStartGame; set => canStartGame = value; }
    public bool BystanderInteract { get => bystanderInteract; set => bystanderInteract = value; }
    public bool CanPauseGame { get => canPauseGame; set => canPauseGame = value; }
    public bool CanPauseTrial { get => canPauseTrial; set => canPauseTrial = value; }
    public float GameCountTimer { get => gameCountTimer; set => gameCountTimer = value; }
    public bool BystanderCanHearAnswer { get => bystanderCanHearAnswer; set => bystanderCanHearAnswer = value; }
    public bool CanStartTrial { get => canStartTrial; set => canStartTrial = value; }
    public int Score { get => score; set => score = value; }

    private void Awake()
    {
        // Create references to other game objects and components
        levelManager = FindObjectOfType<BSLevelManager>();
        userstudyManager = FindObjectOfType<UserStudyManager>();
        logManager = FindObjectOfType<BSLogManager>();
        pauseController = FindObjectOfType<BSPauseController>();
        bystanderAvatar = FindObjectOfType<BSBystanderAvatar>();
        bysTracker = FindObjectOfType<BSRotateTracker>();
        headMovement = FindObjectOfType<HeadMovement>();

        // Game Notification
        instructionUI.SetActive(false);
        trialInstructionUI.SetActive(false);
        // surveryUI.gameObject.SetActive(false);
        timeUI.SetActive(false);
        scoreUI.SetActive(false);
        trialProcessUI.SetActive(false);

        if (!isPracticeGame)
        {
            trialLobbyMenuUI.SetActive(false);         
        }
        else // Practice game
        {
            lobbyMenuUI.SetActive(false);
            trialStartButton.SetActive(false);
        }

        saberObject.SetActive(false);
        participantID = userstudyManager.GetID();

        // GAME
        instructionText = instructionUI.GetComponentInChildren<TMP_Text>();
        gameScoreText = scoreUI.GetComponentsInChildren<Image>()[1].GetComponentInChildren<TMP_Text>();
        gameTimeText = timeUI.GetComponentsInChildren<Image>()[1].GetComponentInChildren<TMP_Text>();
        //TRIAL
        trialLobbyText = trialLobbyMenuUI.GetComponentInChildren<TMP_Text>();
        trialInstructionText = trialInstructionUI.GetComponentInChildren<TMP_Text>();
        // TRIAL

        foreach (GameObject cube in stopCubes)
        {
            cube.SetActive(false);
        }
    }

    private void Start()
    {
        gameTimerToZero = totalGameTime; // set time for the game e.g., 150
        score = 0;

        // Interruption Time of Bystander
        BystanderStartTime2 = BystanderStartTime + bystanderInterval;
        BystanderStartTime3 = BystanderStartTime2 + bystanderInterval;
        BystanderStartTime4 = BystanderStartTime3 + bystanderInterval;

        // X, Y Yxis of VR User
        maincameraAxis = Camera.main.transform.eulerAngles;
        mainCameraYAxis = Camera.main.transform.eulerAngles.y;
        mainCameraXAxis = Camera.main.transform.eulerAngles.x;
       
        maxXAxis = mainCameraXAxis;
        minXAxis = mainCameraXAxis;
        maxYAxis = mainCameraYAxis;
        minYAxis = mainCameraYAxis;

        if (participantID == "" || participantID == null)
        {
            participantID = "ID not assigned";
        }

        // TRIAL_GAME
        if (isPracticeGame)
        {
            trialLobbyText.text = "";
            StartCoroutine(WelcomeInstruction());
        }      
    }

    private void FixedUpdate()
    {
        if (CanStartGame)
        {      
            maincameraAxis = Camera.main.transform.localEulerAngles;

            // Head Movement
            if (!recordStartAxis)
            {
                if (maincameraAxis.y > 180 && maincameraAxis.y <= 360)
                {
                    mainCameraYAxis = 360f - maincameraAxis.y;
                }
                if (maincameraAxis.y >= 0 && maincameraAxis.y < 180)
                {
                    mainCameraYAxis = maincameraAxis.y * -1f;
                }
                if (maincameraAxis.x > 180 && maincameraAxis.x <= 360)
                {
                    mainCameraXAxis = 360f - maincameraAxis.x;
                }
                if (maincameraAxis.x >= 0 && maincameraAxis.x < 180)
                {
                    mainCameraXAxis = maincameraAxis.x * -1f;
                }

                LogCameraAxisAtStart();
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
                if (maincameraAxis.y > 180 && maincameraAxis.y <= 360)
                {
                    mainCameraYAxis = 360f - maincameraAxis.y;
                }
                if (maincameraAxis.y >= 0 && maincameraAxis.y < 180)
                {
                    mainCameraYAxis = maincameraAxis.y * -1f;
                }

                // Set Max. & Min. Value
                if (minYAxis > mainCameraYAxis)
                {
                    minYAxis = mainCameraYAxis;
                    minYVecotorAxis = maincameraAxis;
                }
                   
                if (maxYAxis < mainCameraYAxis)
                {
                    maxYAxis = mainCameraYAxis;
                    maxYVectorAxis = maincameraAxis;
                }
                   


                if (maincameraAxis.x > 180 && maincameraAxis.x <= 360)
                {
                    mainCameraXAxis = 360f - maincameraAxis.x;
                }
                if (maincameraAxis.x >= 0 && maincameraAxis.x < 180)
                {
                    mainCameraXAxis = maincameraAxis.x * -1f;
                }

                if (minXAxis > mainCameraXAxis)
                {
                    minXAxis = mainCameraXAxis;
                    minXVectorAxis = maincameraAxis;               
                }
                   
                if (maxXAxis < mainCameraXAxis)
                {
                    maxXAxis = mainCameraXAxis;
                    maxXVectorAxis = maincameraAxis;
                }

                gameTimerIgnoringPause += Time.fixedDeltaTime;

                if (!gamePaused)
                {
                    GameCountTimer += Time.fixedDeltaTime;
                    // gameTimeText.text = Math.Round(gameTimer - GameCountTimer).ToString(); // gameTimer - Math.Round(gameCountTimer)
                    gameTimeText.text = ConvertToMinAndSeconds(gameTimerToZero - GameCountTimer);

                    if (!askSpawnCubes)
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
                    gameTimeText.text = ConvertToMinAndSeconds(gameTimerToZero - GameCountTimer);
                }
            }
        }

        if (isPracticeGame && canStartTrial)
        {
            // Time Before Game Start
            if (Time.time >= timeFromSceneLoading && Time.time <= startTimeForSpawingCubes) // Showing Time
            {
              //  Debug.Log("Practice is called " + startTimeForSpawingCubes);
                beforeGameTimer += Time.fixedDeltaTime;
                // gameTimeText.text = Math.Round(getReadyTime - beforeGameTimer).ToString();
                gameTimeText.text = ConvertToMinAndSeconds(getReadyTimeForTrial - beforeGameTimer);
            }
            // GAME TIME
            else if (Time.time > startTimeForSpawingCubes && GameCountTimer <= totalGameTime) // During the Game
            {
                gameTimerIgnoringPause += Time.fixedDeltaTime;

                if (!gamePaused)
                {
                    GameCountTimer += Time.fixedDeltaTime;
                    gameTimeText.text = ConvertToMinAndSeconds(gameTimerToZero - GameCountTimer);

                    if (askSpawnCubesForTrial == false)
                    {
                        SpawnCubesForTrial();
                        askSpawnCubesForTrial = true;
                    }

                    if (Math.Round(GameCountTimer) == totalGameTime)
                    {
                        trialCubeSpawner.CanSpawn = false;
                        StopRayInteractoin();
                        //EndGame();
                        EndTrial();
                    }
                }
                else
                {
                    // gameTimeText.text = Math.Round(gameTimer - GameCountTimer).ToString();
                    gameTimeText.text = ConvertToMinAndSeconds(gameTimerToZero - GameCountTimer);
                }
            }

           
        }

    }


    public void StartGame()
    {
        CanStartGame = true;
        timeFromSceneLoading = Time.time; // Time.time returns the amount of time in seconds since the project started playing
        startTimeForSpawingCubes = timeFromSceneLoading + getReadyTime;

        headMovement.CanMeasure = true;
        headMovement.InGame = true;

        // Music 
        lobbyMusicAudioSource.Stop();
        bgMusicAudioSource.Play();

        // UI GameObject 
        saberObject.SetActive(true);
        scoreUI.SetActive(true);
        timeUI.SetActive(true);
        Destroy(lobbyMenuUI);

        // Scene Management
        currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        string currentSceneName = SceneManager.GetActiveScene().name;

        // Logging Game-Start
        logManager.WriteLogFile("Game Start");
        logManager.WriteLogForHeadMovement("Game Start");
        logManager.WriteLogForVRUserHead("Game Start");
        logManager.WriteLogFile("Condition: " + currentSceneName + ", Order: " + (currentSceneIndex + 1));
        logManager.WriteLogForHeadMovement("Condition: " + currentSceneName + ", Order: " + (currentSceneIndex + 1));
        logManager.WriteLogForVRUserHead("Condition: " + currentSceneName + ", Order: " + (currentSceneIndex + 1));
       
        // mainCameraYAxis
        // logManager.WriteToLogFile("Head Movement (Rotation): X: ");
    }

    public void SetTimeStampForAvatarInCriticalZone()
    {
        // string curDateTime = GetCurrentTime();
        Debug.Log("Bystander Interaction (Enter 30-0 d Zone): " + (float)Math.Round(gameTimerIgnoringPause));
        logManager.WriteLogFile("Bystander Interaction (Enter 30-0 d Zone): " + (float)Math.Round(gameTimerIgnoringPause));
        logManager.WriteLogForHeadMovement("Bystander Interaction (Enter 30-0 d Zone): " + (float)Math.Round(gameTimerIgnoringPause));
        logManager.WriteLogForVRUserHead("Bystander Interaction (Enter 30-0 d Zone): " + (float)Math.Round(gameTimerIgnoringPause));
    }

    public void SetTimeStampForAvatarInCriticalZoneWithMessage(string state)
    {
        Debug.Log("Bystander Interaction: " + state + " " + (float)Math.Round(gameTimerIgnoringPause) + "sec");
        logManager.WriteLogFile("Bystander " + state + ": " + (float)Math.Round(gameTimerIgnoringPause) + "sec");
        logManager.WriteLogForHeadMovement("Bystander " + state + ": " + (float)Math.Round(gameTimerIgnoringPause) + "sec");
        logManager.WriteLogForVRUserHead("Bystander " + state + ": " + (float)Math.Round(gameTimerIgnoringPause) + "sec");
    }

    private void LogCameraAxisAtStart()
    {
        logManager.WriteLogFile("Head Movement [START]: " + "Y-Axis: " + mainCameraYAxis + ", X-Axis: " + mainCameraXAxis + ", Vector:" + maincameraAxis);
        logManager.WriteLogForVRUserHead("Head Movement [START]: " + "Y-Axis: " + mainCameraYAxis + ", X-Axis: " + mainCameraXAxis + ", Vector:" + maincameraAxis);
        // TODO: Head Movement Start Value
        logManager.WriteLogForHeadMovement("Head Movement [START]: " + "Y-Axis: " + mainCameraYAxis + ", X-Axis: " + mainCameraXAxis + ", Vector:" + maincameraAxis);
    }

    public void SpawnCubes()
{
   
        cubeSpawner.CanSpawn = true;
        CanPauseGame = true;

        instructionText.enabled = false;
        gameScoreText.text = "0";

        Invoke(nameof(BystanderStartTurningToVRPlayer), time: BystanderStartTime);

        if (!oneInteruption)
        {
            Invoke(nameof(BystanderStartTurningToVRPlayer), time: BystanderStartTime2);
            Invoke(nameof(BystanderStartTurningToVRPlayer), time: BystanderStartTime3);
            Invoke(nameof(BystanderStartTurningToVRPlayer), time: BystanderStartTime4);
        }
        // interactionUI.SetActive(true);
    }



    public void BystanderStartTurningToVRPlayer()
    {
        bysTracker.IsHeadingToPlayer = true;
        BystanderInteract = true;
        pauseController.OncePausedInSession = false;
        // logManager
        logManager.WriteLogFile("Bystander starts turning towards VR user: " + (float)Math.Round(gameTimerIgnoringPause));

        bystanderCanHearAnswer = true;
        bystanderAvatar.LookedOnceSeatedPosition = false;
        bystanderAvatar.IsGuidingFOVToSeatedExceed = false;
    }

    public void BystanderEnd()
    {
        BystanderInteract = false;
    }

    public void SliceCube(GameObject cube)
    {
        //  Debug.Log(cube.name + " called the Method");
        if (score % 8 == 0 && score > 0)
        {
            //ToDo: Short Effect
            gameEffectAudioSource.PlayOneShot(cheerSound);
            Instantiate(cheerEffect, cube.transform.position, Quaternion.identity);
        }
        else
        {
            gameEffectAudioSource.PlayOneShot(rightSlice);
        }

    
       
   

        if (cube.name.Contains("Blue"))
        {
            gameEffectAudioSource.PlayOneShot(sliceSound);

            Instantiate(blueEffect, cube.transform.position, Quaternion.identity);
            score += 1;
            gameScoreText.text = score.ToString();
        }
        else if (cube.name.Contains("Green"))
        {
            gameEffectAudioSource.PlayOneShot(sliceSound);

            Instantiate(greenEffect, cube.transform.position, Quaternion.identity);
            score += 1;
            gameScoreText.text = score.ToString();
        }
        else if (cube.name.Contains("Yellow"))
        {
            gameEffectAudioSource.PlayOneShot(sliceSound);
            Instantiate(yellowEffect, cube.transform.position, Quaternion.identity);
            score += 1;
            gameScoreText.text = score.ToString();
        }

        if (score % 10 == 0 && score > 0 && score < 30)
        {
            if(isPracticeGame)
                ShowPauseHint();
        }

        
        //if(score % 5 == 0 && score > 30)
        //{
        //    Debug.Log("speed is higher");
        //    trialCubeSpawner.beat = 0.8f;
        //}
        //if(score > 20 && score % 25 == 0 && score < 30)
        //{
        //    ShowFinishHint();
        //}

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
                logManager.WriteLogFile("Identification (Paused) Time: " + identificationTime);
                logManager.WriteLogForHeadMovement("Identification (Paused) Time: " + identificationTime);
                logManager.WriteLogForVRUserHead("Identification (Paused) Time: " + identificationTime);
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
                    instructionMsg = "You paused the game.\n Try to resume the game by pressing the trackpad again!";
                    //trialInstructionText.text = instructionMsg;
                    trialInstructionText.text = instructionMsg;
                    trialOncePaused = true;
                }
                else
                {
                    instructionMsg = "You paused the game.\n Try to resume the game!";
                    trialInstructionText.text = instructionMsg;
                }

               // trialInstructionText.text = instructionMsg;
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
                logManager.WriteLogFile("Resume Time: " + (float)Math.Round(gameTimerIgnoringPause));
                logManager.WriteLogForHeadMovement("Resume Time: " + (float)Math.Round(gameTimerIgnoringPause));
                logManager.WriteLogForVRUserHead("Resume Time: " + (float)Math.Round(gameTimerIgnoringPause));
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
               // logManager.WriteLogFile("Resume Time: " + (float)Math.Round(gameTimerIgnoringPause));
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
                    instructionMsg = "You resumed the game! \n You can play the game again!";
                    trialInstructionText.text = instructionMsg;
                    trialOnceResumed = true;
                    StartCoroutine(CleanInstructionBoard());
                }
                else
                {
                    instructionMsg = "You resumed the game.";
                    trialInstructionText.text = instructionMsg;
                    StartCoroutine(CleanInstructionBoard());
                }

               // trialInstructionText.text = instructionMsg;
                trialInstructionUI.GetComponentsInChildren<RawImage>()[0].enabled = false;
                // notificationUI.GetComponentsInChildren<RawImage>()[0].enabled = false;
               // StartCoroutine(InstructionForPause());
                StartRayInteraction();
            }
        }
    }

    public void EndGame()
    {
        trialInstructionUI.SetActive(false);

        if (!isPracticeGame)
        {
            instructionUI.SetActive(true);
            instructionText.enabled = true;
            bystanderInteract = false;
            CanPauseGame = false;
            CanPauseTrial = false;
            headMovement.CanMeasure = false;
            headMovement.InGame = false;
            

            float avgValue = headMovement.GetResultHeadMovement();
           

            cubeSpawner.CanSpawn = false;
            cubes = GameObject.FindGameObjectsWithTag("Cube");
            foreach (GameObject cube in cubes)
            {
                // Debug.Log(cube.name + " Destrpy");
                Destroy(cube);
            }
            saberObject.SetActive(false);
            trialInstructionUI.GetComponentsInChildren<RawImage>()[0].enabled = false;
            //
            scoreUI.SetActive(false);
            timeUI.SetActive(false);

            instructionText.text = "BRAVO!\nYOUR SCORE IS " + score + "!";
         
            

            gameTimeText.text = ConvertToMinAndSeconds(0);

            if (!recordScore)
            {
                logManager.WriteLogFile("Score: " + score);
                recordScore = true;
            }

            if (!recordMaxMin)
            {
                //logManager.WriteLogFile("Max Y Axis (Toward Bystander): " + maxYAxis + " Vector: " + maxYVectorAxis);
                //logManager.WriteLogFile("Min Y Axis (Against Bystander): " + minYAxis + " Vector: " + minYVecotorAxis);
                //logManager.WriteLogFile("Max X Axis: " + maxXAxis + " Vector: " + maxXVectorAxis);
                //logManager.WriteLogFile("Min X Axis: " + minXAxis + " Vector: " + minXVectorAxis);
                //logManager.WriteLogFile("================================\n=================");
                LogVRHeadsetAxis();
                logManager.WriteLogFile("END GAME");
                logManager.WriteLogForHeadMovement("END GAME");
                logManager.WriteLogForVRUserHead("END GAME");
                logManager.WriteLogFile("Head Movement Avg. (every 0.20s): " + avgValue);
                logManager.WriteLogForHeadMovement("Head Movement Avg. (every 0.20s): " + avgValue);
                logManager.WriteLogForVRUserHead("Head Movement Avg. (every 0.20s): " + avgValue);
                recordMaxMin = true;
            }

            Invoke(nameof(GoToNextLevel), 5f);
            // Invoke(nameof(DoSurvey), 1f);

            //TODO:
            // levelManager.LoadGameOver();
        }
        else
        {
            CanPauseGame = false;
            CanPauseTrial = false;
            trialCubeSpawner.CanSpawn = false;
            saberObject.SetActive(false);
            instructionText.text = "Your Trial Game is finised!";
            gameTimeText.text = ConvertToMinAndSeconds(0);
            Invoke(nameof(GoToNextLevel), 5f);
        }
    }

    private void LogVRHeadsetAxis()
    {
        logManager.WriteLogFile("Head Movement [END]: Max Y-Axis (Toward Bystander): " + maxYAxis + " Vector: " + maxYVectorAxis);
        logManager.WriteLogFile("Head Movement [END]:  Min Y-Axis (Against Bystander): " + minYAxis + " Vector: " + minYVecotorAxis);
        logManager.WriteLogFile("Head Movement [END]:  Max X-Axis: " + maxXAxis + " Vector: " + maxXVectorAxis);
        logManager.WriteLogFile("Head Movement [END]:  Min X-Axis: " + minXAxis + " Vector: " + minXVectorAxis);
        logManager.WriteLogFile("================================\n=================");
    }
    public void GoSurvey()
    {
        // surveryUI.SetActive(true);
    }
    public void DoSurvey()
    {
        // surveryUI.SetActive(true);
        lineVisual.enabled = true;

        //lineVisual.gameObject.SetActive(true);
        trialInstructionUI.SetActive(false);
        //  notificationBGImage.enabled = false;
        instructionText.enabled = false;
        // menuUICanvas.SetActive(false);
    }

    public void GoToNextLevel()
    {
        //BSLevelManager levelManager = FindObjectOfType<BSLevelManager>();
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
            logManager.WriteLogFile("Receive FOCUS: " + eyeFocusTime);
        }
        else
        {
            logManager.WriteLogFile("LOST FOCUS: " + eyeFocusTime);
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
        gameEffectAudioSource.PlayOneShot(missCubeSound);
    }

    public void AskQuestion()
    {
        Invoke(nameof(PlayQuestionAudio), 3f);
    }

    public void PlayQuestionAudio()
    {

        if (questionCounter < 4)
        {
            if (questionCounter == 0)
            {

            }
            else
            {
                int index = audioOrder[questionCounter - 1] - 1;
                quesitionAudioSource.PlayOneShot(questionAudios[index]);
                logManager.WriteLogFile("Bystander ask the question " + audioOrder[questionCounter - 1] + ": " + (float)Math.Round(gameTimerIgnoringPause));


            }
            questionCounter++;
        }
    }

    private void ShowPauseHint()
    {
        trialInstructionUI.SetActive(true);
        trialInstructionText.text = "Try to Pause the game.\n You can press the trackpad on the controller";
        trialInstructionUI.GetComponentsInChildren<RawImage>()[0].enabled = true;
        Debug.Log("pause instruction is called");
    }

    private void ShowFinishHint()
    {
        Debug.Log("Called Finish hint");
        trialInstructionUI.SetActive(true);
        trialInstructionText.text = "You can practice this game until you feel confident! \n You can click the Finish button below whenever you want to stop this tutorial.";
        StartCoroutine(CleanInstructionBoard());
    }


    /****************************************************************************************************
     * *********************************** TRIAL *******************************************************
     ****************************************************************************************************/
    //  TRIAL_GAME:2
    public void StartTrialIntroduction()
    {
        trialLobbyMenuUI.SetActive(false);

        trialInstructionUI.SetActive(true);
       
        trialInstructionUI.GetComponentsInChildren<RawImage>()[0].enabled = false;

        saberObject.SetActive(true);

        //foreach (GameObject cube in stopCubes)
        //{
        //    cube.SetActive(true);
        //}
        trialProcessUI.SetActive(true);

        CanStartTrial = true;
        timeFromSceneLoading = Time.time;
        startTimeForSpawingCubes = timeFromSceneLoading + getReadyTimeForTrial; 


        StartCoroutine(InstructionsForCubeSlice());
    }

    public void StartTrialGame()
    {
        CanStartTrial = true;
        //score = 0;
        //foreach (GameObject cube in stopCubes)
        //{
        //    if (cube != null)
        //    {
        //        Destroy(cube);
        //    }
        //}

        timeFromSceneLoading = Time.time; 
        startTimeForSpawingCubes = timeFromSceneLoading + getReadyTimeForTrial; // getReadyTimeForTrial: longer than the main game
     

        //timeFromSceneLoading = Time.time;
        //startTimeForSpawingCubes = timeFromSceneLoading + getReadyTimeForTrial;
        trialInstructionText.text = "";


  
        //trialLobbyMenuUI.SetActive(false);
        //trialInstructionUI.SetActive(true);

        //saberObject.SetActive(true);
        //StartCoroutine(InstructionsForCubeSlice());
    }

    public void SpawnCubesForTrial()
    {
        trialCubeSpawner.CanSpawn = true;

        gameScoreText.text = "0";

      //  CanPauseGame = true;
        CanPauseTrial = true;
    }

    public void EndTrial()
    {
        CanPauseGame = false;
        CanPauseTrial = false;
        trialCubeSpawner.CanSpawn = false;
        saberObject.SetActive(false);
        trialInstructionText.text = "Your Trial Game is finised!";
       // instructionText.text = "Your Trial Game is finised!";
        gameTimeText.text = ConvertToMinAndSeconds(0);
        Invoke(nameof(GoToNextLevel), 5f);
    }

    // TRIAL_GAME: 1
    IEnumerator WelcomeInstruction()
    {
        instructionMsg = "Welcome to our user study!";
        trialLobbyText.text = instructionMsg;
        trialLobbyMenuUI.GetComponentsInChildren<RawImage>()[0].enabled = false; // trackpad image
        yield return new WaitForSeconds(6);

        instructionMsg = "Please click the Start button below when you're ready." +
            "\n\n You can press the trigger button of the controller.";
        trialLobbyText.text = instructionMsg;
        trialLobbyMenuUI.GetComponentsInChildren<RawImage>()[0].enabled = true; // trackpad image
        trialStartButton.SetActive(true);
        yield return new WaitForSeconds(0);
    }

    IEnumerator InstructionsForCubeSlice()
    {
        //instructionMsg = "You now hold a lightsaber in your right hand! \n\n " +
        //    "Slash the colored parts of the blocks in front of you with your saber! \n\n You don't need to press any button on the controller";
        //trialInstructionText.text = instructionMsg;
        //trialInstructionUI.GetComponentsInChildren<RawImage>()[0].enabled = false;
        instructionMsg = "You now hold a lightsaber in your right hand! \n\n " +
           "Slash the colored parts of the blocks with your saber! \n\n You don't need to press any button on the controller";
        trialInstructionText.text = instructionMsg;
        yield return new WaitForSeconds(10);
        timeUI.SetActive(true);
        scoreUI.SetActive(true);
       // trialInstructionText.text = "";
        trialInstructionUI.SetActive(false);
 
        //instructionMsg = "If you slash all boxes, you can press the NEXT button below.";
        //trialInstructionText.text = instructionMsg;


        yield return new WaitForSeconds(0); //20
    }

    IEnumerator InstructionForPause()
    {
        instructionMsg = ""; 
        trialInstructionText.text = instructionMsg;
        trialInstructionUI.GetComponentsInChildren<RawImage>()[0].enabled = false;
        yield return new WaitForSeconds(10);
        trialInstructionText.text = "Try to Pause the game.\n You can press the touchpad on the controller";
        trialInstructionUI.GetComponentsInChildren<RawImage>()[0].enabled = true;
        yield return new WaitForSeconds(3);
    }

    IEnumerator CleanInstructionBoard()
    {
        trialInstructionUI.SetActive(false);
        yield return new WaitForSeconds(3);
    }
}