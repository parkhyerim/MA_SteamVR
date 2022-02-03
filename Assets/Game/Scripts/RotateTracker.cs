using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateTracker : MonoBehaviour
{

    public float speed = 2;
    public bool isForInteraction;
    public bool isGoing;
    public bool isGoingOrigin;
    public bool isHeading;

    // Update is called once per frame
    void Update()
    {
        if (isForInteraction && (transform.eulerAngles.y <= 90) && !isGoing && !isGoingOrigin && isHeading) {
            transform.Rotate(0, 1 * speed * Time.deltaTime, 0);
            // RotateForInteraction();
            //  isForInteraction = !isForInteraction;
            if(Mathf.Round(transform.eulerAngles.y) == 90) {
                Invoke("GoingBack", 3.0f);
               
            }
        }

        if (isGoing && !isHeading) {
           // Debug.Log("isGoing is called"); 
            if(transform.eulerAngles.y > 60) {
                transform.Rotate(0, -1 * speed * Time.deltaTime, 0);
                if (Mathf.Round(transform.eulerAngles.y) == 60) {
                    Invoke("GoingBacktoOrigin", 10.0f);
                }
            }
        }

        if (isGoingOrigin && !isHeading) {
           // Debug.Log("isGoingOrigin is called");
            if(transform.eulerAngles.y > 0) {
                transform.Rotate(0, -1 * speed * Time.deltaTime, 0);
                if (Mathf.Round(transform.eulerAngles.y) == 0) {
                    isGoingOrigin = false;
                }
            }
        }

        

        
      //  transform.Rotate(0, Input.GetAxis("Horizontal") * speed * Time.deltaTime, 0);
    }

    public void GoingBack() {
        isGoing = true;
        isHeading = false;
    }

    public void GoingBacktoOrigin() {
        isGoing = false;
        isGoingOrigin = true;
        isHeading = false;
    }

    public void RotateForNoInteraction() {
       
    }

    public void RotateForInteraction() {
        Debug.Log("Rotate For interaction is called");
        transform.Rotate(0, 10 * speed * Time.deltaTime, 0);
    }
}
