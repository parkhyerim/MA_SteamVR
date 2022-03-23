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
    public AudioClip rightSlice;
    public AudioClip wrongSlice;
    public AudioClip sliceSound;

    [Header("EFFECT")]
    public GameObject rightSliceEffectPrefab;
    public GameObject wrongSliceEffectPrefab;
    public GameObject blueCubeSliceEffetPrefab;
    public GameObject greenCubeSliceEffectPrefab;
    public GameObject yellowCubeSliceEffectPrefab;

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
    public float gameCountTimerIgnoringPause;

    float gameCountTimer;
    int gameTimer;
    private float timeFromSceneLoading, startTimeForSpawingCubes; // time to show Card images, time to turn backwards again
    float beforeGameTimer = 0f;
    public float BystanderStartTime = 25f;

    // public Button pauseBtn;
    [SerializeField]
    private float pausedTime, identificationTime, eyeFocusedTime ;
    bool gameIsPaused;

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
    int currentSceneIndex;

    public CubeSpawner cubeSpawner;

    public bool CanStartGame { get => canStartGame; set => canStartGame = value; }
    public bool BystanderInteract { get => bystanderInteract; set => bystanderInteract = value; }
    public bool CanPauseGame { get => canPauseGame; set => canPauseGame = value; }
    public float GameCountTimer { get => gameCountTimer; set => gameCountTimer = value; }

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

        //foreach(Image img in notificationCheerImages)
        //{
        //    img.enabled = false;
        //}

        // canMusicPlay = true;
        //  interactionUI.SetActive(false);
        // pauseBtn.gameObject.SetActive(false);

        if (participantID == "" || participantID == null)
        {
            participantID = "ID not assigned";
        }
    }

    private void FixedUpdate()
    {
        if (CanStartGame)
        {
            if (Time.time >= timeFromSceneLoading && Time.time <= startTimeForSpawingCubes) // Showing Time
            {
                beforeGameTimer += Time.fixedDeltaTime;
                // gameTimeText.text = Math.Round(getReadyTime - beforeGameTimer).ToString();
                gameTimeText.text = ConvertToMinAndSeconds(getReadyTime - beforeGameTimer);

                //if (isFrontCard == false)
                //{
                //    ShowCards();
                //    isFrontCard = true;
                //}
            }
            else if (Time.time > startTimeForSpawingCubes && GameCountTimer <= totalGameTime) // During the Game
            {
                gameCountTimerIgnoringPause += Time.fixedDeltaTime;
                Debug.Log("Game Timer is running");
              
               // cubeSpawner.SetSpawner();

                if (!gameIsPaused)
                {
                    GameCountTimer += Time.fixedDeltaTime; 
                   // gameTimeText.text = Math.Round(gameTimer - GameCountTimer).ToString(); // gameTimer - Math.Round(gameCountTimer)
                    gameTimeText.text = ConvertToMinAndSeconds(gameTimer - GameCountTimer);
                    cubeSpawner.CanSpawn = true;
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

        scoreUI.SetActive(true);
        timeUI.SetActive(true);
        instructionUI.SetActive(true);

        currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        // currentLevelIndex = 0;
        string currentSceneName = SceneManager.GetActiveScene().name;
        logManager.WriteToLogFile("Study Order: " + currentSceneIndex + " , name: " + currentSceneName);

        //Instantiate(tunnelEffectPrefab1);
        //Instantiate(tunnelEffectPrefab2);
    }

    public void SetAvatarTimeStamp()
    {
        string curDateTime = GetCurrentTime();
        logManager.WriteToLogFile("Bystander wants to interact: " + (float)Math.Round(gameCountTimerIgnoringPause) + " [" + curDateTime +"]");
    }

    public void ShowCards()
    {             
       // Vector3 frontAngles = new Vector3(0, 0, 0);

        //foreach(MemoryCard card in allCards) {
        //    card.transform.localEulerAngles = frontAngles;
        //}

       // instructionText.text = "Match Pairs by Clicking Two Cards!";

        Invoke("SpawnCubes", time: getReadyTime);
        Invoke(nameof(showNotification), time: getReadyTime - 0.8f);
    }

    public void showNotification()
    {
        notificationUI.SetActive(true);
      //  notificationBGImage.enabled = true;
        notificationText.text = "GAME START!";
    }
    public void SpawnCubes() {

        cubeSpawner.CanSpawn = true;

     //   notificationBGImage.enabled = false;
        notificationText.enabled = false;
        instructionText.text = "";
    
        gameScoreText.text = "0";
       // Vector3 backAngles = new Vector3(0, 180, 0);

        //foreach(MemoryCard card in allCards) {
        //    card.IsGameStart = true;
        //    card.transform.localEulerAngles = backAngles;
        //    card.gameObject.GetComponent<XRSimpleInteractable>().interactionManager.enabled = true;
        //}

        CanPauseGame = true;
        //isFront = false;
        Invoke(nameof(BystanderStart), time: BystanderStartTime);
       // interactionUI.SetActive(true);
      //  pauseBtn.gameObject.SetActive(true);
    }

    public void BystanderStart()
    {
        gameTimeText.text = "";
        bysTracker.IsHeadingToPlayer = true;
        BystanderInteract = true;
        // logManager
    }
 
    public void CubeSliced(GameObject cube)
    {
      //  audioSource.PlayOneShot(sliceSound);

        Debug.Log(cube.name + " called the Method");

        if(cube.tag == "BlueCube")
        {
            Debug.Log("Blue Cube Right Slice");
            audioSource.PlayOneShot(rightSlice);
            Instantiate(blueCubeSliceEffetPrefab, cube.transform.position, Quaternion.identity);
            Destroy(cube);
            score += 1;
            gameScoreText.text = score.ToString();
        }
        
        if(cube.tag == "GreenCube")
        {
            Debug.Log("Green Cube Slice");
            audioSource.PlayOneShot(rightSlice);
            Instantiate(greenCubeSliceEffectPrefab, cube.transform.position, Quaternion.identity);
            Destroy(cube);
            if (score > 0)
            {
                score += 1;
                gameScoreText.text = score.ToString();
            }
        }
        
        if(cube.tag == "YellowCube")
        {
            Debug.Log("YEllow Cube Slice");
            audioSource.PlayOneShot(wrongSlice);
            Instantiate(yellowCubeSliceEffectPrefab, cube.transform.position, Quaternion.identity);
            Destroy(cube);
            if (score > 0)
            {
                score += 1;
                gameScoreText.text = score.ToString();
            }
        }
    }

    public void PauseGame()
    {
        if (!gameIsPaused)
        {
            gameIsPaused = true;
            pausedTime = (float)Math.Round(GameCountTimer);
            identificationTime = (float)Math.Round(gameCountTimerIgnoringPause);
            logManager.WriteToLogFile("Identification (Paused) Time: " + identificationTime);
            StopRayInteractoin();
        }
        else
        {
            gameIsPaused = false;
            logManager.WriteToLogFile("Resume Time: " + (float)Math.Round(gameCountTimerIgnoringPause));
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

        notificationText.text = "BRAVO!\nYOUR SCORE IS " + score +"!";

        if (!recordScore)
        {
            logManager.WriteToLogFile("Score: " + score);
            logManager.WriteToLogFile("==============================");
            recordScore = true;
        }

       // tunnelEffectPrefab2.SetActive(false);
       // tunnelEffectPrefab1.SetActive(false);

       // Invoke(nameof(GoToNextLevel), 2f);
        Invoke(nameof(DoSurvey), 1f);
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
        LevelManager levelManager = FindObjectOfType<LevelManager>();
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

    public void EyeFocused()
    {
        DateTime localDate = DateTime.Now;
        string cultureName = "de-DE"; // de-DE  en-GB en-US
        var culture = new CultureInfo(cultureName);
        string name = localDate.ToString(culture);

        Debug.Log("Eye focused: " + name);
        eyeFocusedTime = (float)Math.Round(gameCountTimerIgnoringPause);
        logManager.WriteToLogFile("Eye Focused Time: " + eyeFocusedTime);
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

}
