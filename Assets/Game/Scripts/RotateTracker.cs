using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This is a temporary script to rotate the Bystander's tracker with programming
public class RotateTracker : MonoBehaviour
{
    public bool doInteraction;
    public bool useTracker;

    public float rotateSpeed = 2;
    [SerializeField]
    private bool isHeadingTo30, isHeadingToFrontSeat;
    // TODO: why the gameManager needs to call this variable?
    public bool isHeadingToPlayer;
    public Animator anim;

    private void Start()
    {
        anim.SetBool("isInteracting", false);
    }
    void Update()
    {
        if (!useTracker)
        {
            // Bystander: 90 -> 0 degrees (towards the VR Player)
            if ((transform.eulerAngles.y <= 90)
                && !isHeadingTo30 && !isHeadingToFrontSeat && isHeadingToPlayer)
            {
                transform.Rotate(Vector3.up * Time.deltaTime * rotateSpeed);  // Rotate the object around its local Y axis at 1 * speed degrees per second
                                                                              // transform.Rotate(0, 1 * speed * Time.deltaTime, 0); // previous code
                                                                              // RotateForInteraction();

                anim.SetBool("isInteracting", false);


                if (Mathf.Round(transform.eulerAngles.y) == 90)
                { // Heading towards the VR-Player
                    if (doInteraction)
                    {
                        if (anim)
                        {
                            //   Debug.Log("anim interaction is called");
                            anim.SetBool("isInteracting", true);
                        }

                        // Debug.Log("Invoke: " + System.DateTime.Now);
                        Invoke("HeadingBackTo30Degrees", 13f); // Stay in 13 seconds
                    }
                    else
                    {


                        // Debug.Log("Invoke: " + System.DateTime.Now);
                        Invoke("HeadingBackTo30Degrees", 0.5f);   // Stay in 0.5 seconds
                    }
                }
            }

            // Bystander: 0 -> 30 degrees (towards the front seat)
            if (isHeadingTo30 && !isHeadingToPlayer)
            {
                if (transform.eulerAngles.y > 60)
                {
                    transform.Rotate(Vector3.up * Time.deltaTime * rotateSpeed * -1);
                    // transform.Rotate(0, -1 * speed * Time.deltaTime, 0);
                    anim.SetBool("isInteracting", false);
                    if (Mathf.Round(transform.eulerAngles.y) == 60)
                    {
                        if (doInteraction)
                        {


                            Invoke("HeadingBacktoFrontSeat", 0f);
                        }
                        else
                        {

                            Invoke("HeadingBacktoFrontSeat", 12.5f);
                        }
                    }
                }
            }

            // Bystander: 30 -> 90 degrees (towards the front seat)
            if (isHeadingToFrontSeat && !isHeadingToPlayer)
            {
                anim.SetBool("isInteracting", false);
                if (transform.eulerAngles.y > 0)
                {
                    transform.Rotate(Vector3.up * Time.deltaTime * rotateSpeed * -1);

                    if (Mathf.Round(transform.eulerAngles.y) == 0)
                    {
                        isHeadingToFrontSeat = false;
                    }
                }
            }

            // Controll the tracker with arrow keys 
            // transform.Rotate(0, Input.GetAxis("Horizontal") * speed * Time.deltaTime, 0);
        }
    }

    private void HeadingBackTo30Degrees() {
       // Debug.Log(" Called: " + System.DateTime.Now + doInteraction);
        isHeadingTo30 = true;
        isHeadingToPlayer = false;
        anim.SetBool("isInteracting", false);
    }

    private void HeadingBacktoFrontSeat() {
        isHeadingTo30 = false;
        isHeadingToFrontSeat = true;
        isHeadingToPlayer = false;
        anim.SetBool("isInteracting", false);
    }

    public void SetisHeadingToPlayer()
    {

    }
}
