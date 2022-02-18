using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;
using UnityEngine.XR.Interaction.Toolkit;

public class MemoryCardGameManager : MonoBehaviour
{
    public AudioSource audioSource;

    public AudioClip clipCardForward;
    public AudioClip clipCardBackward;
    public AudioClip clipCardMatch;
    public AudioClip clipCardUnmatch;

    public MemoryCard[] allCards;
    public List<Vector3> allPositionsOfCards = new List<Vector3>();
    public Vector3 AngleOfCards = new Vector3();

    public MemoryCard firstSelectedCard;
    public MemoryCard secondSelectedCard;
    //private MemoryCard mc = new MemoryCard();
    
    private bool canClick = true;
    [SerializeField]
    private bool isFront = false;

    public float showCardsInSeconds = 10f;
    public float warmUpInSeconds = 5f;
    private float startToShowTimer; // time to show Card images
    private float hideTimer; // time to turn backwards again

    public RotateTracker rt;

    public Text gameScoreText;
    public Text timeText;

    [SerializeField]
    private int score;

    public GameObject matchEffectPrefab;
    public GameObject matchEffectPrefab2;
    float timer = 0.0f;

  

    private void Awake()
    {


       // GetComponent<MemoryCard>().gameObject.layer = LayerMask.NameToLayer("Default");
       
        // Get all card positions and save in list
        foreach(MemoryCard card in allCards)
        {
            allPositionsOfCards.Add(card.transform.position);
           // card.gameObject.layer = LayerMask.NameToLayer("Default");
           
            Debug.Log(card.gameObject.GetComponent<XRSimpleInteractable>().interactionLayers);
            card.gameObject.GetComponent<XRSimpleInteractable>().interactionManager.enabled = false;
          
            // Debug.Log(card.name + " " + card.transform.position + " " + card.transform.localEulerAngles);
        }

        AngleOfCards = allCards[0].transform.localEulerAngles;
       // ** Debug.Log(AngleOfCards);
        // Randomize the positions of the cards
        System.Random randomNumber = new System.Random();
        allPositionsOfCards = allPositionsOfCards.OrderBy(position => randomNumber.Next()).ToList();
    
        // Assign a new position
        for(int i = 0; i < allCards.Length; i++)
        {
            allCards[i].transform.position = allPositionsOfCards[i];
        }

        //
        startToShowTimer = Time.time + warmUpInSeconds;
        hideTimer = startToShowTimer + showCardsInSeconds;
      //  Debug.Log(startToShowTimer + " " + hideTimer);
        score = 0;
        gameScoreText.text = "SCORE: "+ score.ToString() + "/20";
        timeText.text = "";

    }

    private void Update()
    {
        //Debug.Log("starttimer: " + startToShowTimer + "  time: " + Time.time);
        // check the current time to see whether it's time for hiding cards

      
        if (Time.time >= startToShowTimer && Time.time <= hideTimer)
        {
            timer += Time.deltaTime;
            timeText.text = "TIME REMANING: " + (showCardsInSeconds-Math.Round(timer));
            // *** Debug.Log(timer);
            //  Debug.Log(Time.time + " startToShow:" + startToShowTimer + " hideTimer: "+ hideTimer);
            // ShowCards();
            if (isFront == false) {
               
                ShowCards();
                isFront = true;
            }
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

        Invoke("HideCards", time: showCardsInSeconds);



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
                LevelManager levelManager = FindObjectOfType<LevelManager>();
                levelManager.LoadNextLevel();
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

    public void StartGame() {
        LevelManager levelManager = FindObjectOfType<LevelManager>();
        levelManager.LoadNextLevel();
    }
}
