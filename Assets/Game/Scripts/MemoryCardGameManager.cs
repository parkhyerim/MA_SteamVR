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

    public GameObject[] allCards;
    public List<Vector3> allPositionsOfCards = new List<Vector3>(); 

    public MemoryCard firstSelectedCard;
    public MemoryCard secondSelectedCard;

    private bool canClick = true;
    private bool isFront;

    public float showCardsInSeconds = 5f;
    public float warmUpInSeconds = 5f;
    private float startToShowTimer; // time to show Card images
    private float hideTimer; // time to turn backwards again

    private void Awake()
    {
        // Get all card positions and save in list
        foreach(GameObject card in allCards)
        {
            allPositionsOfCards.Add(card.transform.position);
        }

        // Randomize positions
        System.Random randomNumber = new System.Random();
        allPositionsOfCards = allPositionsOfCards.OrderBy(position => randomNumber.Next()).ToList();
    
        // Assign new position
        for(int i = 0; i < allCards.Length; i++)
        {
            allCards[i].transform.position = allPositionsOfCards[i];
        }

        startToShowTimer = Time.time + warmUpInSeconds;
        hideTimer = Time.time + showCardsInSeconds;
    }

    private void Update()
    {
        //Debug.Log("starttimer: " + startToShowTimer + "  time: " + Time.time);
        // check the current time to see whether it's time for hiding cards
        if(Time.time >= startToShowTimer)
        {
            if (!isFront)
            {
               // ShowCards();
            }
          
        }

        if (Time.time >= hideTimer && isFront == true)
        {
           // ShowCards();
        }
    }

    public void ShowCards()
    {
            float targetRotation = 0;
            Quaternion rotationValue = Quaternion.Euler(0, targetRotation, 0);
            for (int i = 0; i < allCards.Length; i++)
            {
                allCards[i].transform.rotation = Quaternion.Lerp(transform.rotation, rotationValue, 10 * Time.deltaTime);
                
              allCards[i].transform.rotation = rotationValue;
                Debug.Log(allCards[i].transform.rotation);
            }
        isFront = true;
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
}
