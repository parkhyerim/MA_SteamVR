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
    public GameObject menuUI;
    public GameObject notificationUI;
    public GameObject timeUI;
    public GameObject scoreUI;
    public GameObject instructionUI;
    public GameObject surveryUI;

    public TMP_Text gameScoreText;
    public TMP_Text gameTimeText;
    // public GameObject interactionUI;
    public TMP_Text instructionText;

    public TMP_Text notificationText;
   // public Image notificationBGImage;
   // public List<Image> notificationCheerImages;


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

    [Header("SCORE")]
    [SerializeField]
    private int score;
    private bool canStartGame;
    private bool canPauseGame;

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

    private void Awake()
    {       
        // Time Management
        gameTimer = totalGameTime;

        // Score
        score = 0;
       // gameScoreText.text = "";
       // gameTimeText.text = "";

        // Game Notification
        notificationUI.gameObject.SetActive(false);
        surveryUI.gameObject.SetActive(false);
        timeUI.gameObject.SetActive(false);
        scoreUI.gameObject.SetActive(false);
        instructionUI.gameObject.SetActive(false);

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
    }

    public void StartGame()
    {
        CanStartGame = true;
        timeFromSceneLoading = Time.time;
        startTimeForSpawingCubes = timeFromSceneLoading + getReadyTime;
        Destroy(menuUI);

        lobbyMusic.Stop();
        bgAudioSource.Play();

        scoreUI.SetActive(true);
        timeUI.SetActive(true);
        instructionUI.SetActive(true);

        currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        // currentLevelIndex = 0;
        string currentSceneName = SceneManager.GetActiveScene().name;
        logManager.WriteToLogFile("Study Order: " + currentSceneIndex + " , name: " + currentSceneName);
       // mainCameraYAxis
        logManager.WriteToLogFile("Head Rotation: ");

        //Instantiate(tunnelEffectPrefab1);
        //Instantiate(tunnelEffectPrefab2);
    }

    public void SetTimeStampForAvatarInCriticalZone()
    {
        // string curDateTime = GetCurrentTime();
        Debug.Log("Bystander Interaction (Enter 30-0 d Zone): " + (float)Math.Round(gameTimerIgnoringPause));
        logManager.WriteToLogFile("Bystander Interaction (Enter 30-0 d Zone): " + (float)Math.Round(gameTimerIgnoringPause));
    }

    public void SetTimeStampForAvatarInCriticalZone(string state)
    {
        // string curDateTime = GetCurrentTime();
        logManager.WriteToLogFile("Bystander " + state + ": " + (float)Math.Round(gameTimerIgnoringPause));
    }

    private void SetCameraAxisAtBeginning()
    {
        logManager.WriteToLogFile("Start Y-Axis: " + mainCameraYAxis + " X-Axis: " + mainCameraXAxis + " (" + maincameraAxis + ")");
    }

    //public void ShowCards()
    //{            
    //    Invoke("SpawnCubes", time: getReadyTime);
    //    Invoke(nameof(showNotification), time: getReadyTime - 0.8f);
    //}

    //public void showNotification()
    //{
    //    notificationUI.SetActive(true);
    //  //  notificationBGImage.enabled = true;
    //    notificationText.text = "GAME START!";
    //}
    public void SpawnCubes() {

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
        //  pauseBtn.gameObject.SetActive(true);
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
            audioSource.PlayOneShot(cheerSound);
            Instantiate(cheerEffect, cube.transform.position, Quaternion.identity);
        }
        else
        {
            audioSource.PlayOneShot(rightSlice);
        }
      
        if (cube.name.Contains("Blue"))
        {
            audioSource.PlayOneShot(sliceSound);
          
            Instantiate(blueEffect, cube.transform.position, Quaternion.identity);
           // Destroy(cube);
            score += 1;
            gameScoreText.text = score.ToString();
        } 
        else if (cube.name.Contains("GreenCube"))
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
            gamePaused = true;
            pausedTime = (float)Math.Round(GameCountTimer);
            identificationTime = (float)Math.Round(gameTimerIgnoringPause);
            logManager.WriteToLogFile("Identification (Paused) Time: " + identificationTime);
            cubeSpawner.CanSpawn = false;
            cubeSpawner.StopMoving = true;
            cubes = GameObject.FindGameObjectsWithTag("Cube");
            foreach(GameObject cube in cubes)
            {
               // Debug.Log(cube.name);
                cube.GetComponent<Cube>().StopMove();
            }
            StopRayInteractoin();
        }
        else
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
    }

    public void EndGame()
    {
        notificationUI.SetActive(true);
       // notificationBGImage.enabled = true;
        notificationText.enabled = true;
        bystanderInteract = false;
        CanPauseGame = false;

        cubeSpawner.CanSpawn = false;

        notificationText.text = "BRAVO!\nYOUR SCORE IS " + score +"!";

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
       
        // tunnelEffectPrefab2.SetActive(false);
        // tunnelEffectPrefab1.SetActive(false);

         Invoke(nameof(GoToNextLevel), 2f);
        // Invoke(nameof(DoSurvey), 1f);
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

        //foreach (MemoryCard card in allCards)
        //{
        //    if (card != null)
        //        card.gameObject.SetActive(false);
        //}
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

    //public void WriteToLogFile(string message)
    //{
    //    using (System.IO.StreamWriter logFile = 
    //        new System.IO.StreamWriter(@"C:\Users\ru35qac\Desktop\LogFiles\LogFile_" + participantID +".txt", append:true))
    //    {
    //        logFile.WriteLine(DateTime.Now + message);       
    //    }  
    //}

    void StopRayInteractoin()
    {
        //foreach (MemoryCard card in allCards)
        //{
        //    if (card != null)
        //    {
        //        card.gameObject.GetComponent<XRSimpleInteractable>().interactionManager.enabled = false;
        //      //  card.gameObject.SetActive(false);
        //    }
        //}

         lineVisual.enabled = false;
    }

    void StartRayInteraction()
    {
        //foreach (MemoryCard card in allCards)
        //{
        //    if (card != null)
        //        card.gameObject.GetComponent<XRSimpleInteractable>().interactionManager.enabled = true;
        //}
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
}
