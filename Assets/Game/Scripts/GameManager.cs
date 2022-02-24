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
    public AudioClip bgMusic;
    public AudioSource bgMusicAudioSource;
    bool canMusicPlay;

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
    public GameObject gameProcesCanvas;

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

    private void Awake()
    {       
        // Get all card positions and save in the list
        foreach(MemoryCard card in allCards)
        {
            allPositionsOfCards.Add(card.transform.position);
           // card.gameObject.layer = LayerMask.NameToLayer("Default");
           
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
        notificationText.text = "";
        notificationBGImage.enabled = false;
        canMusicPlay = true;
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
                // *** Debug.Log(timer);
                //  Debug.Log(Time.time + " startToShow:" + startToShowTimer + " hideTimer: "+ hideTimer);
                // ShowCards();
               
                if (isFront == false)
                {
                    ShowCards();
                    isFront = true;
                }
            }
            else if (Time.time > hideCardAgainInSec && gameCountTimer <= totalGameTime)
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


            //if (canMusicPlay)
            //{
            //    bgMusicAudioSource.Play(0);
            //   // audioSource.Play();
            //    Debug.Log("music can be played");
            //}
            //else
            //{
            //    bgMusicAudioSource.Stop();
            //}


        }


    }
    //private void Update()
    //{
    //    //Debug.Log("starttimer: " + startToShowTimer + "  time: " + Time.time);
    //    // check the current time to see whether it's time for hiding cards

    //    if (canStart)
    //    {
    //        if (Time.time >= startShowingCards && Time.time <= hideCardAgainInSec)
    //        {
    //            timer += Time.deltaTime;
    //            timeText.text = "TIME REMANING: " + (memorizingTime - Math.Round(timer));
    //            // *** Debug.Log(timer);
    //            //  Debug.Log(Time.time + " startToShow:" + startToShowTimer + " hideTimer: "+ hideTimer);
    //            // ShowCards();
    //            if (isFront == false)
    //            {
    //                ShowCards();
    //                isFront = true;
    //            }
    //        }
    //        else if(Time.time > hideCardAgainInSec)
    //        {
    //            gameCountTimer += Time.deltaTime;

    //            timeText.text = "Time: " + (gameTimer - Math.Round(gameCountTimer));
    //        }
    //    }
      
    //}


    void StopRayInteractoin()
    {
        foreach (MemoryCard card in allCards)
        {
   
            if(card != null)
                card.gameObject.GetComponent<XRSimpleInteractable>().interactionManager.enabled = false;
        }
    }

    public void ShowCards()
    {             
        Vector3 frontAngles = new Vector3(0, 0, 0);

        foreach(MemoryCard card in allCards) {
            card.transform.localEulerAngles = frontAngles;
        }
        //for (int i = 0; i < allCards.Length; i++) {
        //    //allCards[i].transform.localEulerAngles = frontAngles;

        //    allCards[i].transform.Rotate(frontAngles);

        //    Debug.Log(allCards[i].transform.localEulerAngles);


        //}
        instructionText.text = "Match Pairs by Clicking Two Cards!";

        Invoke("HideCards", time: memorizingTime);
        Invoke(nameof(showNotification), time: memorizingTime - 1f);



        //float targetRotation = 0;
        //Quaternion rotationValue = Quaternion.Euler(0, targetRotation, 0);
        //for (int i = 0; i < allCards.Length; i++) {
        //    allCards[i].transform.rotation = Quaternion.Lerp(transform.rotation, rotationValue, 10 * Time.deltaTime);

        //    allCards[i].transform.rotation = rotationValue;
        //    Debug.Log(allCards[i].transform.rotation);
        //}

    }


    public void showNotification()
    {
        notificationBGImage.enabled = true;
        notificationText.text = "GAME START!";
    }
    public void HideCards() {
        //  timeText.text = "Game Start";

        notificationBGImage.enabled = false;
        notificationText.enabled = false;
        instructionText.text = "";
    
    gameScoreText.text = "0/20";
        // Debug.Log("HideCards is called");
        Vector3 backAngles = new Vector3(0, 180, 0);
        //for (int i = 0; i < allCards.Length; i++) {
        //    allCards[i].transform.localEulerAngles = backAngles;
            
        //}
        foreach(MemoryCard card in allCards) {
            card.IsGameStart = true;
            card.transform.localEulerAngles = backAngles;
            card.gameObject.GetComponent<XRSimpleInteractable>().interactionManager.enabled = true;
        }
        //isFront = false;
        //mc.IsGameStart = true;
        Invoke("BystanderStart", 15f);
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
       // Debug.Log(card.name + " " + card.identifier);
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

            audioSource.PlayOneShot(clipCardMatch);
            if(score == 20)
            {
                EndGame();
                Invoke(nameof(GoToNextLevel), 10);
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
        notificationBGImage.enabled = true;
        notificationText.enabled = true;
        notificationText.text = "FINISH";
        
    }

    public void GoToNextLevel() {
        LevelManager levelManager = FindObjectOfType<LevelManager>();
        levelManager.LoadNextLevel();
    }

    public void StartGame()
    {
        canStart = true;
        startShowingCards = Time.time + bufferBeforeStartingGame;
        hideCardAgainInSec = startShowingCards + memorizingTime;
        Destroy(menuUICanvas);
    }


}
