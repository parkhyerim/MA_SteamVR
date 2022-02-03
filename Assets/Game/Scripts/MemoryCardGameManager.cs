using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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

    private void Awake()
    {
       
        // Get all card positions and save in list
        foreach(MemoryCard card in allCards)
        {
            allPositionsOfCards.Add(card.transform.position);
            
         
           
           // Debug.Log(card.name + " " + card.transform.position + " " + card.transform.localEulerAngles);
        }

        AngleOfCards = allCards[0].transform.localEulerAngles;
        Debug.Log(AngleOfCards);
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
        Debug.Log(startToShowTimer + " " + hideTimer);

    }

    private void Update()
    {
        //Debug.Log("starttimer: " + startToShowTimer + "  time: " + Time.time);
        // check the current time to see whether it's time for hiding cards

      
        if (Time.time >= startToShowTimer && Time.time <= hideTimer )
        {
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
       
        Debug.Log("showCard called");
        Vector3 frontAngles = new Vector3(0, 0, 0);

        foreach(MemoryCard card in allCards) {
            card.transform.localEulerAngles = frontAngles;
        }
        //for (int i = 0; i < allCards.Length; i++) {
        //    //allCards[i].transform.localEulerAngles = frontAngles;

        //    allCards[i].transform.Rotate(frontAngles);

        //    Debug.Log(allCards[i].transform.localEulerAngles);


        //}

        Invoke("HideCards", 10);



        //float targetRotation = 0;
        //Quaternion rotationValue = Quaternion.Euler(0, targetRotation, 0);
        //for (int i = 0; i < allCards.Length; i++) {
        //    allCards[i].transform.rotation = Quaternion.Lerp(transform.rotation, rotationValue, 10 * Time.deltaTime);

        //    allCards[i].transform.rotation = rotationValue;
        //    Debug.Log(allCards[i].transform.rotation);
        //}

    }

    public void HideCards() {
        
        Debug.Log("HideCards is called");
        Vector3 backAngles = new Vector3(0, 180, 0);
        //for (int i = 0; i < allCards.Length; i++) {
        //    allCards[i].transform.localEulerAngles = backAngles;
            
        //}
        foreach(MemoryCard card in allCards) {
            card.IsGameStart = true;
            card.transform.localEulerAngles = backAngles;
        }
        //isFront = false;
        //mc.IsGameStart = true;
        Invoke("BystanderStart", 3f);
    }

    public void BystanderStart() {
        rt.isHeading = true;
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
            Destroy(firstSelectedCard.gameObject);
            Destroy(secondSelectedCard.gameObject);

            audioSource.PlayOneShot(clipCardMatch);
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

    }
}
