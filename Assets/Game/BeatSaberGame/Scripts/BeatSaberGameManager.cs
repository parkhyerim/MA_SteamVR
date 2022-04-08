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
    public GameObject practiceLobbyMenuUI;
    public GameObject notificationUI;
    public GameObject timeUI;
    public GameObject scoreUI;
    public GameObject instructionUI;
    // public GameObject surveryUI;

    public TMP_Text gameScoreText;
    public TMP_Text gameTimeText;
    public TMP_Text instructionText;
    public TMP_Text trialInstructionText;

    public TMP_Text notificationText;

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

    [Header("TRIAL TIME MNG)")]
    public float getReadyTimeForTrial;

    [Header("SCORE")]
    [SerializeField]
    private int score;

    [Header("BOOLEADN")]
    private bool canStartGame;
    [SerializeField]
    private bool canPauseGame, canStartTrial, gamePaused;

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
    public float GameCountTimer { get => gameCountTimer; set => gameCountTimer = value; }
    public bool BystanderCanHearAnswer { get => bystanderCanHearAnswer; set => bystanderCanHearAnswer = value; }
    public bool CanStartTrial { get => canStartTrial; set => canStartTrial = value; }

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
        notificationUI.gameObject.SetActive(false);
        // surveryUI.gameObject.SetActive(false);
        timeUI.gameObject.SetActive(false);
        scoreUI.gameObject.SetActive(false);
        if (!isPracticeGame)
        {
            instructionUI.gameObject.SetActive(false);
            practiceLobbyMenuUI.gameObject.SetActive(false);
        }
        else // Practice game
        {
            lobbyMenuUI.gameObject.SetActive(false);
            // TrialStartBtn.enabled = false;
        }
        //  instructionUI.gameObject.SetActive(false);

        saberObject.SetActive(false);
        participantID = userstudyManager.GetID();
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

        // Debug.Log("ID: " + participantID);

        //cameraAxis = Camera.main.transform.eulerAngles;
        //previousYAxis = cameraAxis.y;
        //currentYAxis = cameraAxis.y;

        //Debug.Log("cameraAxis: " + cameraAxis);
        //Debug.Log("prev: " + previousYAxis);
        //Debug.Log("curr: " + currentYAxis);
    }

    private void FixedUpdate()
    {
        //checkPointTime += Time.deltaTime;
        //cameraAxis = Camera.main.transform.eulerAngles;

        //if (checkPointTime >= period)
        //{
        //    currentYAxis = cameraAxis.y;
        //    diffYAxis = previousYAxis - currentYAxis;
        //    num += 1;
        //    //  Debug.Log("Time passed" + " " + Time.time + " checkPointTime: " + checkPointTime);
        //    //  Debug.Log("checked Point Time: " + checkPointTime);
        //    Debug.Log("Time:" + Time.time + " period: " + checkPointTime + " prev:" + previousYAxis + " cur: " + currentYAxis + " diffYAxis: " + diffYAxis + " num: " + num);

        //    checkPointTime = 0f;
        //}

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
                    gameTimeText.text = ConvertToMinAndSeconds(gameTimerToZero - GameCountTimer);

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
                    gameTimeText.text = ConvertToMinAndSeconds(gameTimerToZero - GameCountTimer);
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

        timeUI.SetActive(true);
    }

    public void StartGame()
    {
        CanStartGame = true;
        timeFromSceneLoading = Time.time; // Time.time returns the amount of time in seconds since the project started playing
        startTimeForSpawingCubes = timeFromSceneLoading + getReadyTime;
        Destroy(lobbyMenuUI);
        headMovement.CanMeasure = true;
        headMovement.InGame = true;

        // Music 
        lobbyMusicAudioSource.Stop();
        bgMusicAudioSource.Play();

        // GameObject 
        saberObject.SetActive(true);
        scoreUI.SetActive(true);
        timeUI.SetActive(true);

        // Scene Management
        currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        string currentSceneName = SceneManager.GetActiveScene().name;
        logManager.WriteToLogFile("Condition : " + currentSceneName + " , Order: " + (currentSceneIndex + 1));
       
        // mainCameraYAxis
       // logManager.WriteToLogFile("Head Movement (Rotation): X: ");
    }

    public void SetTimeStampForAvatarInCriticalZone()
    {
        // string curDateTime = GetCurrentTime();
        Debug.Log("Bystander Interaction (Enter 30-0 d Zone): " + (float)Math.Round(gameTimerIgnoringPause));
        logManager.WriteToLogFile("Bystander Interaction (Enter 30-0 d Zone): " + (float)Math.Round(gameTimerIgnoringPause));
    }

    public void SetTimeStampForAvatarInCriticalZoneWithMessage(string state)
    {
        Debug.Log("Bystander Interaction: " + state + " " + (float)Math.Round(gameTimerIgnoringPause) + "sec");
        logManager.WriteToLogFile("Bystander " + state + ": " + (float)Math.Round(gameTimerIgnoringPause) + "sec");
    }

    private void SetCameraAxisAtBeginning()
    {
        logManager.WriteToLogFile("Head Movement: [START] " + "Y-Axis: " + mainCameraYAxis + " X-Axis: " + mainCameraXAxis + " Vector:" + maincameraAxis);
    }

    public void SpawnCubes()
    {
        if (!isPracticeGame)
        {
            cubeSpawner.CanSpawn = true;
            CanPauseGame = true;

            notificationText.enabled = false;
            gameScoreText.text = "0";

            Invoke(nameof(BystanderStart), time: BystanderStartTime);

            if (!oneInteruption)
            {
                Invoke(nameof(BystanderStart), time: BystanderStartTime2);
                Invoke(nameof(BystanderStart), time: BystanderStartTime3);
                Invoke(nameof(BystanderStart), time: BystanderStartTime4);
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
        // gameTimeText.text = "";
        bysTracker.IsHeadingToPlayer = true;
        BystanderInteract = true;
        pauseController.OncePausedInSession = false;
        // logManager
        logManager.WriteToLogFile("Bystander starts to rotate towards VR user: " +(float)Math.Round(gameTimerIgnoringPause));
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
                    instructionMsg = "You paused the game.\n Try to resume the game!";
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
            headMovement.CanMeasure = false;
            headMovement.InGame = false;
            float avgValue = headMovement.GetResultHeadMovement();
            logManager.WriteToLogFile("Head Movement Avg. (every 0.20s): " + avgValue);

            cubeSpawner.CanSpawn = false;
            cubes = GameObject.FindGameObjectsWithTag("Cube");
            foreach (GameObject cube in cubes)
            {
                // Debug.Log(cube.name + " Destrpy");
                Destroy(cube);
            }
            saberObject.SetActive(false);
            notificationUI.GetComponentsInChildren<RawImage>()[0].enabled = false;
            //
            scoreUI.SetActive(false);
            timeUI.SetActive(false);

            notificationText.text = "BRAVO!\nYOUR SCORE IS " + score + "!";

            gameTimeText.text = ConvertToMinAndSeconds(0);

            if (!recordScore)
            {
                logManager.WriteToLogFile("Score: " + score);
                recordScore = true;
            }

            if (!recordMaxMin)
            {
                logManager.WriteToLogFile("Max Y Axis (Toward Bystander): " + maxYAxis + " Vector: " + maxYVectorAxis);
                logManager.WriteToLogFile("Min Y Axis (Against Bystander): " + minYAxis + " Vector: " + minYVecotorAxis);
                logManager.WriteToLogFile("Max X Axis: " + maxXAxis + " Vector: " + maxXVectorAxis);
                logManager.WriteToLogFile("Min X Axis: " + minXAxis + " Vector: " + minXVectorAxis);
                logManager.WriteToLogFile("================================\n=================");
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
            trialCubeSpawner.CanSpawn = false;
            saberObject.SetActive(false);
            notificationText.text = "Your Trial Game is finised!";
            gameTimeText.text = ConvertToMinAndSeconds(0);
            Invoke(nameof(GoToNextLevel), 5f);
        }
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
        notificationUI.SetActive(false);
        //  notificationBGImage.enabled = false;
        notificationText.enabled = false;
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
                logManager.WriteToLogFile("Bystander ask the question " + audioOrder[questionCounter - 1] + ": " + (float)Math.Round(gameTimerIgnoringPause));

            }
            questionCounter++;
        }
    }
}