using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;

public class MemoryCardGameManager : MonoBehaviour
{
    public AudioSource audioSource;

    public AudioClip clipCardForward;
    public AudioClip clipCardBackward;
    public AudioClip clipCardMatch;
    public AudioClip clipCardUnmatch;

    public GameObject uiCavans;

    public MemoryCard[] allCards;
    public List<Vector3> allPositionsOfCards = new List<Vector3>();
    public Vector3 AngleOfCards = new Vector3();

    public MemoryCard firstSelectedCard;
    public MemoryCard secondSelectedCard;
    //private MemoryCard mc = new MemoryCard();
    
    private bool canClick = true;
    [SerializeField]
    private bool isFront = false;

    public float memorizingTime;
    public float warmUpInSeconds;
    public int gameTimes;

    [SerializeField]
    private float startShowingCards, hideCardAgainInSec; // time to show Card images, time to turn backwards again
    [SerializeField]
    int gameTimer;
    public RotateTracker rt;

    public TMP_Text gameScoreText;
    public TMP_Text timeText;

    [SerializeField]
    private int score;

    public GameObject matchEffectPrefab;
    public GameObject matchEffectPrefab2;
    float timer = 0f;
    float gameCountTimer = 0f;

    [SerializeField]
    private bool canStart;

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
        gameTimer = gameTimes;

        score = 0;
        gameScoreText.text = "";
        timeText.text = "";
    }

    private void FixedUpdate()
    {
        if (canStart)
        {
            if (Time.time >= startShowingCards && Time.time <= hideCardAgainInSec)
            {
                timer += Time.fixedDeltaTime;
                timeText.text = "TIME REMANING: " + (memorizingTime - Math.Round(timer)) + " ";
                // *** Debug.Log(timer);
                //  Debug.Log(Time.time + " startToShow:" + startToShowTimer + " hideTimer: "+ hideTimer);
                // ShowCards();
                if (isFront == false)
                {
                    ShowCards();
                    isFront = true;
                }
            }
            else if (Time.time > hideCardAgainInSec && gameCountTimer <= gameTimes)
            {
                gameCountTimer += Time.fixedDeltaTime;

              
                
                timeText.text = "Time: " + (gameTimer - Math.Round(gameCountTimer)); // gameTimer - Math.Round(gameCountTimer)
                if (Math.Round(gameCountTimer) == gameTimes)
                {
                    StopRayInteractoin();
                    Invoke(nameof(GoToNextLevel), 4);
                }
            }
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
       
        // Debug.Log("showCard called");
        
        Vector3 frontAngles = new Vector3(0, 0, 0);

        foreach(MemoryCard card in allCards) {
            card.transform.localEulerAngles = frontAngles;
        }
        //for (int i = 0; i < allCards.Length; i++) {
        //    //allCards[i].transform.localEulerAngles = frontAngles;

        //    allCards[i].transform.Rotate(frontAngles);

        //    Debug.Log(allCards[i].transform.localEulerAngles);


        //}

        Invoke("HideCards", time: memorizingTime);



        //float targetRotation = 0;
        //Quaternion rotationValue = Quaternion.Euler(0, targetRotation, 0);
        //for (int i = 0; i < allCards.Length; i++) {
        //    allCards[i].transform.rotation = Quaternion.Lerp(transform.rotation, rotationValue, 10 * Time.deltaTime);

        //    allCards[i].transform.rotation = rotationValue;
        //    Debug.Log(allCards[i].transform.rotation);
        //}

    }

    public void HideCards() {
        timeText.text = "Game Start";
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
        Invoke("BystanderStart", 3f);
      //  Invoke("StartGame", 30f);
    }

    public void BystanderStart()
    {
        timeText.text = "";
        rt.isHeadingToPlayer = true;
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
            gameScoreText.text = "SCORE: " + score.ToString() + "/20";

            audioSource.PlayOneShot(clipCardMatch);
            if(score == 20)
            {
                Invoke(nameof(GoToNextLevel), 5);
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

    public void GoToNextLevel() {
        LevelManager levelManager = FindObjectOfType<LevelManager>();
        levelManager.LoadNextLevel();
    }

    public void StartGame()
    {
        canStart = true;
        startShowingCards = Time.time + warmUpInSeconds;
        hideCardAgainInSec = startShowingCards + memorizingTime;
        
        Destroy(uiCavans);
    }


}
