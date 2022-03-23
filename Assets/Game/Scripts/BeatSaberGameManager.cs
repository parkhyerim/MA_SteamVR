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
   
    //public GameObject matchEffectPrefab;
    //public GameObject matchEffectPrefab2;
    //public GameObject tunnelEffectPrefab1;
    //public GameObject tunnelEffectPrefab2;

    [Header("GAME UI")]
    public GameObject menuUICanvas;
    public GameObject timeCanvas;
    public GameObject scoreCanvas;
    public GameObject instructionCanvas;
    public TMP_Text gameScoreText;
    public TMP_Text gameTimeText;
    // public GameObject interactionUI;
    public TMP_Text instructionText;
    public GameObject notificationCanvas;
    public TMP_Text notificationText;
    public Image notificationBGImage;
   // public List<Image> notificationCheerImages;
    public GameObject surveryUICanvas;

    [Header("TIME MANAGEMENT")]
    public float getReadyTime;
    public float bufferBeforeStartingGame = 2f;
    public int totalGameTime;
    [SerializeField]
    float gameCountTimer;
    public float gameCountTimerIgnoringPause;
    [SerializeField]
    int gameTimer;
    private float readyTime, startSpawingCubes; // time to show Card images, time to turn backwards again
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
    public string participantID;

    private int randomNumForEffect;
    private bool bystanderInteract;
    public BSLogManager logManager;
    public bool isPracticeGame;
    public bool isEndScene;
    private bool recordScore;
    int currentLevelIndex;

    public CubeSpawner cubeSpawner;
    // [Header("Participant")]

    //public string participantID = null;

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
        gameScoreText.text = "";
        gameTimeText.text = "";

        // Game Notification
        notificationCanvas.gameObject.SetActive(false);
        surveryUICanvas.gameObject.SetActive(false);
        timeCanvas.gameObject.SetActive(false);
        scoreCanvas.gameObject.SetActive(false);
        instructionCanvas.gameObject.SetActive(false);

        //foreach(Image img in notificationCheerImages)
        //{
        //    img.enabled = false;
        //}

        // canMusicPlay = true;
        //  interactionUI.SetActive(false);
        // pauseBtn.gameObject.SetActive(false);

       

        if (participantID == "" || participantID == null)
        {
            DateTime localDate = DateTime.Now;
            string cultureName = "de-DE"; // de-DE  en-GB en-US
            var culture = new CultureInfo(cultureName);
            string name = localDate.ToString(culture);
            participantID = "not assigned";
            // Debug.Log("participant name: " + participantID);
        }
    }

    private void FixedUpdate()
    {
        if (CanStartGame)
        {
            if (Time.time >= readyTime && Time.time <= startSpawingCubes) // Showing Time
            {
                Debug.Log("Update is called");
                //if(Time.time == hideCardAgainInSec)
                //{
                //    gameProcessBackground.enabled = true;
                //    gameProcessText.text = "Match a pair of cards!";
                //}

                //gameProcessBackground.enabled = false;
                //gameProcessText.text = "";

                beforeGameTimer += Time.fixedDeltaTime;
                gameTimeText.text = Math.Round(getReadyTime - beforeGameTimer).ToString();
            
                //if (isFrontCard == false)
                //{
                //    ShowCards();
                //    isFrontCard = true;
                //}
            }
            else if (Time.time > startSpawingCubes && GameCountTimer <= totalGameTime) // During the Game
            {
                gameCountTimerIgnoringPause += Time.fixedDeltaTime;
                Debug.Log("Game Timer is running");
              
               // cubeSpawner.SetSpawner();

                if (!gameIsPaused)
                {
                    GameCountTimer += Time.fixedDeltaTime; 
                    gameTimeText.text = Math.Round(gameTimer - GameCountTimer).ToString(); // gameTimer - Math.Round(gameCountTimer)
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
                    gameTimeText.text = Math.Round(gameTimer - GameCountTimer).ToString();
                }
            }
        }
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
        notificationCanvas.SetActive(true);
        notificationBGImage.enabled = true;
        notificationText.text = "GAME START!";
    }
    public void SpawnCubes() {

        cubeSpawner.CanSpawn = true;

        notificationBGImage.enabled = false;
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
        notificationCanvas.SetActive(true);
        notificationBGImage.enabled = true;
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
        surveryUICanvas.SetActive(true);
    }
    public void DoSurvey()
    { 
        surveryUICanvas.SetActive(true);
        lineVisual.enabled = true;
        
        //lineVisual.gameObject.SetActive(true);
        notificationCanvas.SetActive(false);
        notificationBGImage.enabled = false;
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

    public void StartGame()
    {
        Debug.Log("Start Game Is Called");
        CanStartGame = true;
        readyTime = Time.time + bufferBeforeStartingGame;
        startSpawingCubes = readyTime + getReadyTime;
        Destroy(menuUICanvas);

        scoreCanvas.SetActive(true);
        timeCanvas.SetActive(true);
        instructionCanvas.SetActive(true);
     

        currentLevelIndex = SceneManager.GetActiveScene().buildIndex;
        // currentLevelIndex = 0;
        string currentSceneName = SceneManager.GetActiveScene().name;
        logManager.WriteToLogFile("Study Order: " + currentLevelIndex + " , name: " + currentSceneName);

        //Instantiate(tunnelEffectPrefab1);
        //Instantiate(tunnelEffectPrefab2);
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
}
