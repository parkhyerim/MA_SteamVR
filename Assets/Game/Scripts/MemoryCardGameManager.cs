using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemoryCardGameManager : MonoBehaviour
{
    public MemoryCard firstSelectedCard;
    public MemoryCard secondSelectedCard;

    private bool canClick = true;

    public void CardClicked(MemoryCard card)
    {
        if (canClick == false || card == firstSelectedCard)
        {
            return;
        }
            
        // Always rotate card front to show its image
       // card.transform.localEulerAngles = new Vector3(0,0,0);
        card.targetHeight = 0.05f;
        card.targetRotation = 0;

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
        }
        else
        {
            firstSelectedCard.targetRotation = 180;
            secondSelectedCard.targetRotation = 180;


        }

        // RESET
        firstSelectedCard = null;
        secondSelectedCard = null;

        canClick = true;
    }
}
