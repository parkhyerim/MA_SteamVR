using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("AUDIO")]
    public AudioSource audioSource;
    public AudioClip clipCardForward;
    public AudioClip clipCardBackward;
    public AudioClip clipCardMatch;
    public AudioClip clipCardUnmatch;
   // bool canMusicPlay;

    [Header("EFFECT")]
    public GameObject matchEffectPrefab;
    public GameObject matchEffectPrefab2;

    [Header("UI")]
    public GameObject menuUICanvas;
    public TMP_Text gameScoreText;
    public TMP_Text timeText;
    public TMP_Text instructionText;
    public TMP_Text notificationText;  
    public Image notificationBGImage;
    public List<Image> notificationCheerImages;
    public GameObject gameProcesCanvas;
    public GameObject interactionUI;

    [Header("CARDs")]
    public MemoryCard[] allCards;
    public List<Vector3> allPositionsOfCards = new List<Vector3>();
    public Vector3 AngleOfCards = new Vector3();
    public MemoryCard firstSelectedCard;
    public MemoryCard secondSelectedCard;    
    private bool canClick = true;
    private bool isFront = false;

    [Header("TIME MANAGEMENT")]
    public float memorizingTime;
    public float bufferBeforeStartingGame;
    public int totalGameTime;
    [SerializeField]
    int gameTimer;
    [SerializeField]
    private float startShowingCards, hideCardAgainInSec; // time to show Card images, time to turn backwards again
    float timer = 0f;
    float gameCountTimer = 0f;

    [SerializeField]
    private int score;
    private bool canStart;

    public RotateTracker rTracker;

    public Button pauseBtn;
    [SerializeField]
    private float pausedTime;
    bool pausedGame;
    int randomNumber;

    private void Awake()
    {       
        // Get all card positions and save in the list
        foreach(MemoryCard card in allCards)
        {
            allPositionsOfCards.Add(card.transform.position);
            // to make all cards uninteractable
            card.gameObject.GetComponent<XRSimpleInteractable>().interactionManager.enabled = false;
        }
        
        AngleOfCards = allCards[0].transform.localEulerAngles;

        // Randomize the positions of the cards
        System.Random randomNumber = new System.Random();
        allPositionsOfCards = allPositionsOfCards.OrderBy(position => randomNumber.Next()).ToList();
    
        // Assign a new position
        for(int i = 0; i < allCards.Length; i++)
        {
            allCards[i].transform.position = allPositionsOfCards[i];
        }

        // To set time to show all cards
        //startShowingCards = Time.time + warmUpInSeconds;
        //hideCardAgainInSec = startShowingCards + memorizingTime;
        gameTimer = totalGameTime;

        score = 0;
        gameScoreText.text = "";
        timeText.text = "";
        gameProcesCanvas.gameObject.SetActive(false);
        //notificationText.text = "";
       // notificationBGImage.enabled = false;
        foreach(Image img in notificationCheerImages)
        {
            img.enabled = false;
        }
       
       // canMusicPlay = true;

        interactionUI.SetActive(false);
     // pauseBtn.gameObject.SetActive(false);
    }

    private void FixedUpdate()
    {
        if (canStart)
        {
            if (Time.time >= startShowingCards && Time.time <= hideCardAgainInSec)
            {
                //if(Time.time == hideCardAgainInSec)
                //{
                //    gameProcessBackground.enabled = true;
                //    gameProcessText.text = "Match a pair of cards!";
                //}

                //gameProcessBackground.enabled = false;
                //gameProcessText.text = "";

                timer += Time.fixedDeltaTime;
                timeText.text = (memorizingTime - Math.Round(timer)).ToString();
            
                if (isFront == false)
                {
                    ShowCards();
                    isFront = true;
                }
            }
            else if (Time.time > hideCardAgainInSec && gameCountTimer <= totalGameTime)
            {
                if (!pausedGame)
                {
                    gameCountTimer += Time.fixedDeltaTime;

                    timeText.text = (gameTimer - Math.Round(gameCountTimer)).ToString(); // gameTimer - Math.Round(gameCountTimer)
                    if (Math.Round(gameCountTimer) == totalGameTime)
                    {
                        StopRayInteractoin();
                        EndGame();
                        Invoke(nameof(GoToNextLevel), 10);
                    }
                }
                else
                {
                    timeText.text = (gameTimer - Math.Round(gameCountTimer)).ToString();
                }
            }
        }
    }

    public void PauseGameTime()
    {
        if (!pausedGame)
        {
            pausedTime = gameCountTimer;
            pausedGame = true;
            StopRayInteractoin();
        }
        else
        {
            pausedGame = false;
            StartRayInteraction();
        }
    }
      
    void StopRayInteractoin()
    {
        foreach (MemoryCard card in allCards)
        {   
            if(card != null)
                card.gameObject.GetComponent<XRSimpleInteractable>().interactionManager.enabled = false;
        }
    }

    void StartRayInteraction()
    {
        foreach (MemoryCard card in allCards)
        {
            if (card != null)
                card.gameObject.GetComponent<XRSimpleInteractable>().interactionManager.enabled = true;
        }   
    }

    public void ShowCards()
    {             
        Vector3 frontAngles = new Vector3(0, 0, 0);

        foreach(MemoryCard card in allCards) {
            card.transform.localEulerAngles = frontAngles;
        }

        instructionText.text = "Match Pairs by Clicking Two Cards!";

        Invoke("HideCards", time: memorizingTime);
        Invoke(nameof(showNotification), time: memorizingTime - 0.8f);
    }

    public void showNotification()
    {
        gameProcesCanvas.SetActive(true);
        notificationBGImage.enabled = true;
        notificationText.text = "GAME START!";
    }
    public void HideCards() {
        //  timeText.text = "Game Start";
        
        notificationBGImage.enabled = false;
        notificationText.enabled = false;
        instructionText.text = "";
    
        gameScoreText.text = "0/20";
        Vector3 backAngles = new Vector3(0, 180, 0);

        foreach(MemoryCard card in allCards) {
            card.IsGameStart = true;
            card.transform.localEulerAngles = backAngles;
            card.gameObject.GetComponent<XRSimpleInteractable>().interactionManager.enabled = true;
        }
        //isFront = false;
        Invoke("BystanderStart", 5f);
        interactionUI.SetActive(true);
      //  pauseBtn.gameObject.SetActive(true);
    }

    public void BystanderStart()
    {
        timeText.text = "";
       // gameScoreText.text = "0/20";
        rTracker.isHeadingToPlayer = true;
    }
 
    public void CardClicked(MemoryCard card)
    {
        if (canClick == false || card == firstSelectedCard)
        {
            return;
        }
            
        // Always rotate card forwards to show its image
       // card.transform.localEulerAngles = new Vector3(0,0,0);
        card.targetHeight = 0.05f;
        card.targetRotation = 0;

        audioSource.PlayOneShot(clipCardForward);

        if(firstSelectedCard == null)
        {
            firstSelectedCard = card;
        }
        else
        {
            // Second card selected;
            secondSelectedCard = card;
            canClick = false;
            // 1 second later
            Invoke("CheckMatch", 1);         
        }
    }

    public void CheckMatch()
    {      
        // RESULT
        if (firstSelectedCard.identifier == secondSelectedCard.identifier)
        {
            Instantiate(matchEffectPrefab, firstSelectedCard.gameObject.transform.position, Quaternion.identity);
            Instantiate(matchEffectPrefab, secondSelectedCard.gameObject.transform.position, Quaternion.identity);
            Instantiate(matchEffectPrefab2, firstSelectedCard.gameObject.transform.position, Quaternion.identity);
            Instantiate(matchEffectPrefab2, secondSelectedCard.gameObject.transform.position, Quaternion.identity);
            Destroy(firstSelectedCard.gameObject);
            Destroy(secondSelectedCard.gameObject);
            score += 2;
            gameScoreText.text = score.ToString() + "/20";

            //if ((score % 4 == 0) && score!= 20)
            //{
                // notificationCheerImage.enabled = true;
                StartCoroutine("ShowRandomImage");
                randomNumber = UnityEngine.Random.Range(0, notificationCheerImages.Count);
                notificationCheerImages[randomNumber].enabled = true;
                notificationCheerImages[randomNumber].transform.position = firstSelectedCard.gameObject.transform.position;
            //}

            audioSource.PlayOneShot(clipCardMatch);
            if(score == 20)
            {
                StopRayInteractoin();
                Invoke(nameof(EndGame), 3);
                Invoke(nameof(GoToNextLevel), 8);
            }
        }
        else
        {
            firstSelectedCard.targetRotation = 180;
            secondSelectedCard.targetRotation = 180;
            audioSource.PlayOneShot(clipCardUnmatch);
        }

        // RESET
        firstSelectedCard = null;
        secondSelectedCard = null;

        audioSource.PlayOneShot(clipCardBackward);

        canClick = true;
    }

    public void EndGame()
    {
        gameProcesCanvas.SetActive(true);
        notificationBGImage.enabled = true;
        notificationText.enabled = true;
        
        if (score == 20)
            notificationText.text = "BRAVO!\nYOU WIN!";
        else
            notificationText.text = "GAME OVER!";
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
            notificationCheerImages[randomNumber].enabled = false;
        }
    }

    public void StartGame()
    {
        canStart = true;
        startShowingCards = Time.time + bufferBeforeStartingGame;
        hideCardAgainInSec = startShowingCards + memorizingTime;
        Destroy(menuUICanvas);
    }
}
