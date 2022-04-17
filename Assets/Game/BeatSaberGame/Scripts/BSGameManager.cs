using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Sockets;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class BSGameManager : MonoBehaviour
{
    [Header("AUDIO")]
    public AudioSource gameEffectAudioSource;
    public AudioSource bgMusicAudioSource;
    public AudioSource lobbyMusicAudioSource;
    // TODO: Question Audio
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
    public float bystanderInterval = 40f;

    [Header("TIME INFO.")]
    [SerializeField]
    private float gameTimerIgnoringPause, gameCountTimer;
    int gameTimerToZero;
    private float timeFromSceneLoading, startTimeForSpawningCubes; // time to show Card images, time to turn backwards again
    float beforeGameTimer = 0f;
    [SerializeField]
    private float BystanderStartTime2, BystanderStartTime3, BystanderStartTime4;
    private float pausedTime, identificationTime, eyeFocusTime;

    [Header("TRIAL TIME MNG.")]
    public float getReadyTimeForTrial;

    [Header("SCORE")]
    [SerializeField]
    private int score;

    [Header("BOOLEADN FOR GAME")]
    [SerializeField]
    private bool canStartGame, canPauseGame, gamePaused, gameResumed;
    [Header("BOOLEADN FOR TRIAL")]
    [SerializeField]
    private bool canStartTrial, canPauseTrial;

    [Header("TRACKER")]
    BSRotateTracker bysTracker;
    public XRInteractorLineVisual lineVisual;
    [Header("PARTICIPANT")]
    [SerializeField]
    private string participantID;

    private bool bystanderInteract;

    public bool isPracticeGame;
    public bool isEndScene;
    private bool recordScore, recordMaxMin, recordStartAxis;
    int currentSceneIndex;
    private bool askSpawnCubes;
    //Trial
    private bool pauseInstructed;
    private bool askSpawnCubesForTrial;
    private GameObject[] cubes;
    private GameObject[] trialCubes;
    public CubeSpawner cubeSpawner;
    public CubeSpawner trialCubeSpawner;

    private bool trialOncePaused, trialOnceResumed;
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
    private Vector3 maincameraAxisVector, maxLeftVectorAxis, maxRightVecotorAxis, maxUpVectorAxis, maxDownVectorAxis;
    [SerializeField]
    private float mainCameraYAxis, mainCameraXAxis, mainCameraZAxis, maxRightAxis, maxLeftAxis, maxDownAxis, maxUpAxis;
    public bool oneInteruption;
    private bool bystanderCanHearAnswer;
    public int[] audioOrder = { 1, 2, 3 };
    int questionCounter;
    bool allQuestionAsked, reduceGameTime, calledPushEnd;

    public bool CanStartGame { get => canStartGame; set => canStartGame = value; }
    public bool BystanderInteract { get => bystanderInteract; set => bystanderInteract = value; }
    public bool CanPauseGame { get => canPauseGame; set => canPauseGame = value; }
    public bool CanPauseTrial { get => canPauseTrial; set => canPauseTrial = value; }
    public float GameCountTimer { get => gameCountTimer; set => gameCountTimer = value; }
    public bool BystanderCanHearAnswer { get => bystanderCanHearAnswer; set => bystanderCanHearAnswer = value; }
    public bool CanStartTrial { get => canStartTrial; set => canStartTrial = value; }
    public int Score { get => score; set => score = value; }
    public float MaxRightAxis { get => maxRightAxis; set => maxRightAxis = value; }
    public float MaxLeftAxis { get => maxLeftAxis; set => maxLeftAxis = value; }
    public float MaxUpAxis { get => maxUpAxis; set => maxUpAxis = value; }
    public float MaxDownAxis { get => maxDownAxis; set => maxDownAxis = value; }
    public bool GamePaused { get => gamePaused; set => gamePaused = value; }



    /**************************************************************
     * socket code
     **************************************************************/

    TcpClient mySocket;
    NetworkStream theStream;
    StreamWriter theWriter;
    StreamReader theReader;

    public bool socketReady = false; //a true/false variable for connection status
    //try to initiate connection
    public void setupSocket()
    {
        try
        {
            mySocket = new TcpClient("localhost", 25001);
            theStream = mySocket.GetStream();
            theWriter = new StreamWriter(theStream);
            theReader = new StreamReader(theStream);
            socketReady = true;
        }
        catch (Exception e)
        {
            Debug.Log("Socket error:" + e);
        }
    }
    //send message to server
    public void writeSocket(string theLine)
    {
        if (!socketReady)
            return;
        String tmpString = theLine + "\r\n";
        theWriter.Write(tmpString);
        theWriter.Flush();
    }
    //disconnect from the socket
    public void closeSocket()
    {
        if (!socketReady)
            return;
        theWriter.Close();
        theReader.Close();
        mySocket.Close();
        socketReady = false;
    }
    /**************************************************************
     * socket code ende
     **************************************************************/



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

        // GAME
        instructionText = instructionUI.GetComponentInChildren<TMP_Text>();
        gameScoreText = scoreUI.GetComponentsInChildren<Image>()[1].GetComponentInChildren<TMP_Text>();
        gameTimeText = timeUI.GetComponentsInChildren<Image>()[1].GetComponentInChildren<TMP_Text>();
        //TRIAL
        trialLobbyText = trialLobbyMenuUI.GetComponentInChildren<TMP_Text>();
        trialInstructionText = trialInstructionUI.GetComponentInChildren<TMP_Text>();

        // Game Notification
        instructionUI.SetActive(false);
        trialInstructionUI.SetActive(false);
        trialProcessUI.SetActive(false);
        // surveryUI.gameObject.SetActive(false);
        timeUI.SetActive(false);
        scoreUI.SetActive(false);

        if (!isPracticeGame)
        {
            trialLobbyMenuUI.SetActive(false);         
        }
        else // Practice game
        {
            lobbyMenuUI.SetActive(false);
            // trialStartButton.SetActive(false);
        }

        saberObject.SetActive(false);

        participantID = userstudyManager.GetParticipantID();

        setupSocket();
    }

    private void Start()
    {
        
        gameTimerToZero = totalGameTime; // set time for the game e.g., 150
        score = 0;

        // Interruption Time of Bystander
        BystanderStartTime2 = BystanderStartTime + bystanderInterval;
        BystanderStartTime3 = BystanderStartTime2 + bystanderInterval;
        BystanderStartTime4 = BystanderStartTime3 + bystanderInterval;

        // X, Y Y-Axis of VR User
        maincameraAxisVector = Camera.main.transform.eulerAngles;
        mainCameraYAxis = maincameraAxisVector.y;
        mainCameraXAxis = maincameraAxisVector.x;
        mainCameraZAxis = maincameraAxisVector.z;

        // basic values
        MaxUpAxis = mainCameraXAxis;
        MaxDownAxis = mainCameraXAxis;
        MaxLeftAxis = mainCameraYAxis;
        MaxRightAxis = mainCameraYAxis;

        if (participantID == "" || participantID == null)
            participantID = "IDNotAssigned";

        // TRIAL_GAME
        if (isPracticeGame)
        {
            trialLobbyText.text = "";
            StartCoroutine(WelcomeInstruction());
        }
        //else
        //{
        //    setupSocket();
        //}
    }

    private void FixedUpdate()
    {
        if (CanStartGame)
        {
           // Debug.Log("gametimeingnoringpause: " + gameTimerIgnoringPause);
            //maincameraAxisVector = Camera.main.transform.localEulerAngles;
            maincameraAxisVector = Camera.main.transform.eulerAngles;
            // Debug.Log("local: " + Camera.main.transform.localEulerAngles);
            // Debug.Log("no: " + Camera.main.transform.eulerAngles);
            mainCameraZAxis = maincameraAxisVector.z;

            if (maincameraAxisVector.y > 180 && maincameraAxisVector.y <= 360) // 360-> 270-> 179 => 0-> -90 -> -179
            {
                mainCameraYAxis = maincameraAxisVector.y - 360f;
            }
            if (maincameraAxisVector.y > 0 && maincameraAxisVector.y <= 180) // 1-> 90-> 180 => 1 -> 90 -> 180
            {
                mainCameraYAxis = maincameraAxisVector.y;
            }
            if (maincameraAxisVector.x > 180 && maincameraAxisVector.x <= 360)
            {
                mainCameraXAxis = maincameraAxisVector.x - 360f;
            }
            if (maincameraAxisVector.x > 0 && maincameraAxisVector.x <= 180)
            {
                mainCameraXAxis = maincameraAxisVector.x;
            }

            // Head Movement
            if (!recordStartAxis)
            {
                LogCameraAxisAtStart();
                recordStartAxis = true;
            }

            // Time Before Game Start
            if (Time.time >= timeFromSceneLoading && Time.time <= startTimeForSpawningCubes) // Showing Time
            {
                beforeGameTimer += Time.fixedDeltaTime;
                // gameTimeText.text = Math.Round(getReadyTime - beforeGameTimer).ToString();
                gameTimeText.text = ConvertToMinAndSeconds(getReadyTime - beforeGameTimer);
            }
            // GAME TIME
            else if (Time.time > startTimeForSpawningCubes && GameCountTimer <= totalGameTime) // During the Game
            {
                gameTimerIgnoringPause += Time.fixedDeltaTime;

                // Set Max. & Min. Value
                if (MaxRightAxis < mainCameraYAxis) // against bystander: 0 <-> 90
                {
                    MaxRightAxis = mainCameraYAxis;
                    maxRightVecotorAxis = maincameraAxisVector;
                }
                   
                if (MaxLeftAxis > mainCameraYAxis) // towards bystander: -90 <-> 0
                {
                    MaxLeftAxis = mainCameraYAxis;
                    maxLeftVectorAxis = maincameraAxisVector;
                }                

                if (MaxDownAxis < mainCameraXAxis) // head down: 0 <-> 90
                {
                    MaxDownAxis = mainCameraXAxis;
                    maxDownVectorAxis = maincameraAxisVector;               
                }
                   
                if (MaxUpAxis > mainCameraXAxis) // head up: 0 <-> -90
                {
                    MaxUpAxis = mainCameraXAxis;
                    maxUpVectorAxis = maincameraAxisVector;
                }
            
                if (!gamePaused)
                {
                    GameCountTimer += Time.fixedDeltaTime;
                    // gameTimeText.text = Math.Round(gameTimer - GameCountTimer).ToString(); // gameTimer - Math.Round(gameCountTimer)
                    gameTimeText.text = ConvertToMinAndSeconds(gameTimerToZero - GameCountTimer);
                   // gameTimeText.text = ConvertToMinAndSeconds(GameCountTimer);

                    if (!askSpawnCubes)
                    {
                        SpawnCubes();
                        askSpawnCubes = true;
                    }

                    if (Math.Round(GameCountTimer) == totalGameTime)
                    {
                        //cubeSpawner.CanSpawn = false;
                       // StopRayInteractoin();
                        EndGame();
                    }

                    //if (reduceGameTime && !calledPushEnd)
                    //{
                    //    cubeSpawner.CanSpawn = false;
                    //    StopRayInteractoin();
                    //    EndGame();
                    //    calledPushEnd = true;
                    //}
                }
                else
                {
                    // gameTimeText.text = Math.Round(gameTimer - GameCountTimer).ToString();
                    gameTimeText.text = ConvertToMinAndSeconds(gameTimerToZero - GameCountTimer);
                   // gameTimeText.text = ConvertToMinAndSeconds(GameCountTimer);
                }
            }
        }

        if (isPracticeGame && canStartTrial)
        {
            // Time Before Game Start
            if (Time.time >= timeFromSceneLoading && Time.time <= startTimeForSpawningCubes) // Showing Time
            {
                beforeGameTimer += Time.fixedDeltaTime;
                // gameTimeText.text = Math.Round(getReadyTime - beforeGameTimer).ToString();
                gameTimeText.text = ConvertToMinAndSeconds(getReadyTimeForTrial - beforeGameTimer);
            }
            // GAME TIME
            else if (Time.time > startTimeForSpawningCubes && GameCountTimer <= totalGameTime) // During the Game
            {
                gameTimerIgnoringPause += Time.fixedDeltaTime;

                if (!gamePaused)
                {
                    GameCountTimer += Time.fixedDeltaTime;
                     gameTimeText.text = ConvertToMinAndSeconds(gameTimerToZero - GameCountTimer);
                  //  gameTimeText.text = ConvertToMinAndSeconds(GameCountTimer);

                    if (askSpawnCubesForTrial == false)
                    {
                        SpawnCubesForTrial();
                        askSpawnCubesForTrial = true;
                    }

                    if (Math.Round(GameCountTimer) == totalGameTime)
                    {
                       // trialCubeSpawner.CanSpawn = false;
                        //StopRayInteractoin();
                        //EndGame();
                        EndTrial();
                    }
                }
                else
                {
                    // gameTimeText.text = Math.Round(gameTimer - GameCountTimer).ToString();
                     gameTimeText.text = ConvertToMinAndSeconds(gameTimerToZero - GameCountTimer);
                    //gameTimeText.text = ConvertToMinAndSeconds(GameCountTimer);
                }
            }       
        }
    }


    public void StartGame()
    {
        CanStartGame = true;
        timeFromSceneLoading = Time.time; // Time.time returns the amount of time in seconds since the project started playing
        startTimeForSpawningCubes = timeFromSceneLoading + getReadyTime;

        headMovement.GameStart = true;
       // headMovement.InGame = true;

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
        logManager.WriteLogFile("GAME START");
        logManager.WriteLogForYawHeadMovement("GAME START");
        logManager.WriteLogForPitchHeadMovement("GAME START");
        logManager.WriteLogForRollHeadMovement("GAME START");
        logManager.WriteLogForEyeGaze("GAME START");
        logManager.WriteLogForHeadPosition("GAME START");

        logManager.WriteLogFile("CONDITION: " + currentSceneName + ", ORDER: " + (currentSceneIndex));
        logManager.WriteLogForYawHeadMovement("CONDITION: " + currentSceneName + ", ORDER: " + (currentSceneIndex));        
        logManager.WriteLogForPitchHeadMovement("CONDITION: " + currentSceneName + ", ORDER: " + (currentSceneIndex));
        logManager.WriteLogForRollHeadMovement("CONDITION: " + currentSceneName + ", ORDER: " + (currentSceneIndex));
        logManager.WriteLogForEyeGaze("CONDITION: " + currentSceneName + ", ORDER: " + (currentSceneIndex));
        logManager.WriteLogForHeadPosition("CONDITION: " + currentSceneName + ", ORDER: " + (currentSceneIndex));
    }

    public void SetTimeStampForAvatarInCriticalZoneWithMessage(string state)
    {
        Debug.Log("BYSTANDER: " + state + " " + (float)Math.Round(gameTimerIgnoringPause) + " (" + gameTimerIgnoringPause + ")");
        logManager.WriteLogFile("BYSTANDER " + state + ": " + (float)Math.Round(gameTimerIgnoringPause) + " (" + gameTimerIgnoringPause + ")");
        logManager.WriteLogForYawHeadMovement("BYSTANDER " + state + ": " + (float)Math.Round(gameTimerIgnoringPause) +" (" + gameTimerIgnoringPause + ")");
        logManager.WriteLogForPitchHeadMovement("BYSTANDER " + state + ": " + (float)Math.Round(gameTimerIgnoringPause) + " (" + gameTimerIgnoringPause + ")");
        logManager.WriteLogForRollHeadMovement("BYSTANDER " + state + ": " + (float)Math.Round(gameTimerIgnoringPause) + " (" + gameTimerIgnoringPause + ")");
        logManager.WriteLogForEyeGaze("BYSTANDER " + state + ": " + (float)Math.Round(gameTimerIgnoringPause) + " (" + gameTimerIgnoringPause + ")");
        logManager.WriteLogForHeadPosition("BYSTANDER " + state + ": " + (float)Math.Round(gameTimerIgnoringPause) +" (" + gameTimerIgnoringPause + ")");
    }

    private void LogCameraAxisAtStart()
    {
        Vector3 cameraLocalPos = Camera.main.transform.localPosition;
        logManager.WriteLogFile("HEAD INFORMATION [ORIGIN]: " + "Yaw(Y): " + mainCameraYAxis + ", Pitch(X): " + mainCameraXAxis + ", Roll(Z)" + mainCameraZAxis+ ", Vector3:" + maincameraAxisVector);
        logManager.WriteLogForHeadPosition("Head Movement [ORIGIN]: " + "X: " + cameraLocalPos.x + ", Y: " + cameraLocalPos.y + ", Z: " + cameraLocalPos.z  + ", Vector3:" + cameraLocalPos);
        // TODO: Head Movement Start Value
        logManager.WriteLogForYawHeadMovement("HEAD INFORMATION [ORIGIN]: " + "Yaw(Y): " + mainCameraYAxis + ", Pitch(X): " + mainCameraXAxis + ", Roll(Z)" + mainCameraZAxis + ", Vector3:" + maincameraAxisVector);
        logManager.WriteLogForPitchHeadMovement("HEAD INFORMATION [ORIGIN]: " + "Yaw(Y): " + mainCameraYAxis + ", Pitch(X): " + mainCameraXAxis + ", Roll(Z)" + mainCameraZAxis + ", Vector3:" + maincameraAxisVector);
        logManager.WriteLogForRollHeadMovement("HEAD INFORMATION [ORIGIN]: " + "Yaw(Y): " + mainCameraYAxis + ", Pitch(X): " + mainCameraXAxis + ", Roll(Z)" + mainCameraZAxis + ", Vector3:" + maincameraAxisVector);
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
        logManager.WriteLogFile("BYSTANDER starts turning towards VR user: " + (float)Math.Round(gameTimerIgnoringPause) + " (" + gameTimerIgnoringPause + ")");
        logManager.WriteLogForEyeGaze("BYSTANDER starts turning towards VR user: " + (float)Math.Round(gameTimerIgnoringPause) + " (" + gameTimerIgnoringPause + ")");
        logManager.WriteLogForYawHeadMovement("BYSTANDER starts turning towards VR user: " + (float)Math.Round(gameTimerIgnoringPause) + " (" + gameTimerIgnoringPause + ")");
        logManager.WriteLogForPitchHeadMovement("BYSTANDER starts turning towards VR user: " + (float)Math.Round(gameTimerIgnoringPause) + " (" + gameTimerIgnoringPause + ")");
        logManager.WriteLogForRollHeadMovement("BYSTANDER starts turning towards VR user: " + (float)Math.Round(gameTimerIgnoringPause) + " (" + gameTimerIgnoringPause + ")");
        logManager.WriteLogForHeadPosition("BYSTANDER FROM starts turning towards VR user: " + (float)Math.Round(gameTimerIgnoringPause) + " (" + gameTimerIgnoringPause + ")");
        bystanderCanHearAnswer = true;
        bystanderAvatar.LookedOnceSeatedPosition = false;
        bystanderAvatar.IsGuidingFOVToSeatedExceed = false;
    }

    public void BystanderEnd()
    {
        BystanderInteract = false;
        if (questionCounter == 4)
        {
            Debug.Log("all question is asked");
            allQuestionAsked = true;
            Invoke(nameof(EndGame), 25f);
            //ChangeTotalTime();
        }
    }

    //private void ChangeTotalTime()
    //{
    //    float countTime = gameCountTimer;
    //    float remainTime = totalGameTime - countTime;
    //    Debug.Log("counttime:" + countTime);
    //    Debug.Log("remain time:" + remainTime);
    //    if (remainTime > 30)
    //    {
    //        //totalGameTime = 30;
    //        //Debug.Log(totalGameTime);
    //        reduceGameTime = true;
    //    }
    //}

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
                logManager.WriteLogFile("IDENTIFICATION (Paused) TIME: " + identificationTime + " (" + gameTimerIgnoringPause + ")");
                logManager.WriteLogForYawHeadMovement("IDENTIFICATION (Paused) TIME: " + identificationTime + " (" + gameTimerIgnoringPause + ")");
                logManager.WriteLogForPitchHeadMovement("IDENTIFICATION (Paused) TIME: " + identificationTime + " (" + gameTimerIgnoringPause + ")");
                logManager.WriteLogForRollHeadMovement("IDENTIFICATION (Paused) TIME: " + identificationTime + " (" + gameTimerIgnoringPause + ")");
                logManager.WriteLogForEyeGaze("IDENTIFICATION (Paused) TIME: " + identificationTime + " (" + gameTimerIgnoringPause + ")");
                logManager.WriteLogForHeadPosition("IDENTIFICATION (Paused) TIME: " + identificationTime + " (" + gameTimerIgnoringPause + ")");
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
                logManager.WriteLogFile("RESUME TIME: " + (float)Math.Round(gameTimerIgnoringPause) + " (" + gameTimerIgnoringPause +")");
                logManager.WriteLogForYawHeadMovement("RESUME TIME: " + (float)Math.Round(gameTimerIgnoringPause) + " (" + gameTimerIgnoringPause + ")");
                logManager.WriteLogForPitchHeadMovement("RESUME TIME: " + (float)Math.Round(gameTimerIgnoringPause) + " (" + gameTimerIgnoringPause + ")");
                logManager.WriteLogForRollHeadMovement("RESUME TIME: " + (float)Math.Round(gameTimerIgnoringPause) + " (" + gameTimerIgnoringPause + ")");
                logManager.WriteLogForEyeGaze("RESUME TIME: " + (float)Math.Round(gameTimerIgnoringPause) + " (" + gameTimerIgnoringPause + ")");
                logManager.WriteLogForHeadPosition("RESUME TIME: " + (float)Math.Round(gameTimerIgnoringPause) + " (" + gameTimerIgnoringPause + ")");
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
        StopRayInteractoin();
        cubeSpawner.CanSpawn = false;
        float totalPlayTime = gameTimerIgnoringPause;
       // Debug.Log("END gametimeingnoringpause: " + totalPlayTime);

        logManager.WriteLogFile("Total Game Time: " + totalPlayTime);
        if (!isPracticeGame)
        {
            //writeSocket("endscript");
            //closeSocket();

            instructionUI.SetActive(true);
            instructionText.enabled = true;
            bystanderInteract = false;
            CanPauseGame = false;
            CanPauseTrial = false;
            headMovement.GameStart = false;
            headMovement.GameEnd = true;
            // headMovement.InGame = false;
            

            float avgYawHMValue = headMovement.GetYawHeadMovement();
            float avgPitchHMValue = headMovement.GetPitchHeadMovement();
            float avgRollValue = headMovement.GetRollHeadMovement();

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

            int totalScore = cubeSpawner.GetCountCubes();

            if (!recordScore)
            {
                logManager.WriteLogFile("Score: " + score + " /" + totalScore);
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
                logManager.WriteLogFile("Yaw(Y) Head Movement Avg. (every 0.20s): " + avgYawHMValue);
                logManager.WriteLogFile("Pitch(X) Head Movement Avg. (every 0.20s): " + avgPitchHMValue);
                logManager.WriteLogFile("Roll(Z) Head Movement Avg. (every 0.20s): " + avgRollValue);
                logManager.WriteLogForYawHeadMovement("Yaw(Y) Head Movement Avg. (every 0.20s): " + avgYawHMValue);
                logManager.WriteLogForPitchHeadMovement("Pitch(X) Head Movement Avg. (every 0.20s): " + avgPitchHMValue);
                logManager.WriteLogForRollHeadMovement("Roll(Z) Head Movement Avg. (every 0.20s): " + avgRollValue);

                logManager.WriteLogFile("END GAME");
                logManager.WriteLogForYawHeadMovement("END GAME");
                logManager.WriteLogForPitchHeadMovement("END GAME");
                logManager.WriteLogForRollHeadMovement("END GAME");
                logManager.WriteLogForHeadPosition("END GAME");
                logManager.WriteLogForEyeGaze("END GAME");
                logManager.WriteLogFile("=================================\n");
                logManager.WriteLogForYawHeadMovement("=================================\n");
                logManager.WriteLogForPitchHeadMovement("=================================\n");
                logManager.WriteLogForRollHeadMovement("=================================\n");
                logManager.WriteLogForHeadPosition("=================================\n");
                logManager.WriteLogForEyeGaze("=================================\n");
                recordMaxMin = true;
            }


            GoToNextLevel();
            //Invoke(nameof(GoToNextLevel), 5f);
            // Invoke(nameof(DoSurvey), 1f);

            //TODO:
            // levelManager.LoadGameOver();
        }
        //else
        //{
        //    CanPauseGame = false;
        //    CanPauseTrial = false;
        //    trialCubeSpawner.CanSpawn = false;
        //    saberObject.SetActive(false);
        //    instructionText.text = "Your Trial Game is finised!";
        //    gameTimeText.text = ConvertToMinAndSeconds(0);
        //    GoToNextLevel();
        //   // Invoke(nameof(GoToNextLevel), 5f);
        //}

        Debug.Log("EndGame");
        Debug.Log("Closing socket connection to python");
        writeSocket("endscript");
        closeSocket();
    }

    private void LogVRHeadsetAxis()
    {
        logManager.WriteLogFile("Head Movement [END]: Max Left(Y) (Toward Bystander): " + MaxLeftAxis + " Vector: " + maxLeftVectorAxis);
        logManager.WriteLogFile("Head Movement [END]: Max Right(Y) (Against Bystander): " + MaxRightAxis + " Vector: " + maxRightVecotorAxis);
        logManager.WriteLogFile("Head Movement [END]: Max Up(X): " + MaxUpAxis + " Vector: " + maxUpVectorAxis);
        logManager.WriteLogFile("Head Movement [END]: Max Down(X): " + MaxDownAxis + " Vector: " + maxDownVectorAxis);
        logManager.WriteLogFile("==========================================================");
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
        levelManager.LoadNextLevel();
    }

    //public IEnumerator ShowRandomImage()
    //{
    //    while (true)
    //    {
    //        yield return new WaitForSeconds(1);
    //        // notificationCheerImages[randomNumForEffect].enabled = false;
    //    }
    //}

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

    public void EyeFocused(bool focus, string visType)
    {
        if (BystanderInteract)
        {
            eyeFocusTime = (float)Math.Round(gameTimerIgnoringPause);
            if (focus)
            {
               // logManager.WriteLogFile("[" + visType +"] Receive FOCUS: " + eyeFocusTime + " (" + gameTimerIgnoringPause + ")");
                logManager.WriteLogForEyeGaze("[" + visType + "] Receive FOCUS: " + eyeFocusTime + " (" + gameTimerIgnoringPause + ")");
            }
            else
            {
               // logManager.WriteLogFile("[" + visType + "] LOST FOCUS: " + eyeFocusTime + " (" + gameTimerIgnoringPause + ")");
                logManager.WriteLogForEyeGaze("[" + visType + "] LOST FOCUS: " + eyeFocusTime + " (" + gameTimerIgnoringPause + ")");
            }
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
        Invoke(nameof(PlayQuestionAudio), 0f);
    }

    public void PlayQuestionAudio()
    {
        // default. counter = 0
        if (questionCounter < 4)
        {
            if (questionCounter == 0)
            {
                // no question(1st visualisation)
            }
            else
            {
                int index = audioOrder[questionCounter - 1] - 1;  // 0,1,2
                //quesitionAudioSource.PlayOneShot(questionAudios[index]);

                // socket
                Debug.Log(index+ "question is called");
                writeSocket("question" + index);
                Debug.Log(index + "question is called: " + (float)Math.Round(gameTimerIgnoringPause));
                logManager.WriteLogFile("ASK A QUESTIOM " + audioOrder[questionCounter - 1] + ": " + (float)Math.Round(gameTimerIgnoringPause) + "(" + gameTimerIgnoringPause + ")");
                logManager.WriteLogForEyeGaze("ASK A QUESTIOM " + audioOrder[questionCounter - 1] + ": " + (float)Math.Round(gameTimerIgnoringPause) + "(" + gameTimerIgnoringPause + ")");
                logManager.WriteLogForYawHeadMovement("ASK A QUESTIOM " + audioOrder[questionCounter - 1] + ": " + (float)Math.Round(gameTimerIgnoringPause) + "(" + gameTimerIgnoringPause + ")");
                logManager.WriteLogForPitchHeadMovement("ASK A QUESTIOM " + audioOrder[questionCounter - 1] + ": " + (float)Math.Round(gameTimerIgnoringPause) + "(" + gameTimerIgnoringPause + ")");
                logManager.WriteLogForRollHeadMovement("ASK A QUESTIOM " + audioOrder[questionCounter - 1] + ": " + (float)Math.Round(gameTimerIgnoringPause) + "(" + gameTimerIgnoringPause + ")");
                logManager.WriteLogForHeadPosition("ASK A QUESTIOM " + audioOrder[questionCounter - 1] + ": " + (float)Math.Round(gameTimerIgnoringPause) + "(" + gameTimerIgnoringPause + ")");
            }
            questionCounter++; // 1, 2, 3, 4  end
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
        startTimeForSpawningCubes = timeFromSceneLoading + getReadyTimeForTrial; 


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
        startTimeForSpawningCubes = timeFromSceneLoading + getReadyTimeForTrial; // getReadyTimeForTrial: longer than the main game
     

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
        StopRayInteractoin();
        trialCubeSpawner.CanSpawn = false;
        CanPauseGame = false;
        CanPauseTrial = false;
        trialCubeSpawner.CanSpawn = false;
        saberObject.SetActive(false);
        trialInstructionText.text = "Your Trial Game is finised!";
       // instructionText.text = "Your Trial Game is finised!";
        gameTimeText.text = ConvertToMinAndSeconds(0);
        GoToNextLevel();
       // Invoke(nameof(GoToNextLevel), 5f);
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