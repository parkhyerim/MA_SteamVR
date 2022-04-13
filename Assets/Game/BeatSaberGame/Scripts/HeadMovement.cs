using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class HeadMovement : MonoBehaviour
{
    BSLogManager logManager;

    public float period = 0.2f;
    [SerializeField]
    private float checkTimer = 0.0f;

    private Vector3 camEulerAngles;
    private Quaternion camRotation;

    private float curEulerY, prevEulerY, diffEulerY, avgEulerY, sumEulerY;
    private float conv_curEulerY, conv_prevEulerY, conv_diffEulerY, conv_avgEulerY, conv_sumEulerY;
    private float curRotY, prevRotY, diffRotY, avgRotY, sumRotY;

    private float curEulerX, prevEulerX, diffEulerX, avgEulerX, sumEulerX;
    private float conv_curEulerX, conv_prevEulerX, conv_diffEulerX, conv_avgEulerX, conv_sumEulerX;
    private float curRotX, prevRotX, diffRotX, avgRotX, sumRox;

    private int count;

    bool canMeasure;
    bool inGame;

    public bool CanMeasure { get => canMeasure; set => canMeasure = value; }
    public bool InGame { get => inGame; set => inGame = value; }

    private void Awake()
    {
        logManager = FindObjectOfType<BSLogManager>();
    }
    // Start is called before the first frame update
    void Start()
    {
        count = -1;

        // Euler Angles
        camEulerAngles = Camera.main.transform.eulerAngles;
        
       // prevEulerY = camEulerAngles.y;
        curEulerY = camEulerAngles.y;

        // Rotation Angles
        camRotation = Camera.main.transform.rotation;
        curRotY = camRotation.y;
        curRotX = camRotation.x;
        //Debug.Log(camRotation + " " + curRotX + " " + curRotY);       
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // logManager.WriteToLogFileForHeadMovement("test");

        // Measure Head Movement
        // 1.Euler
        camEulerAngles = Camera.main.transform.eulerAngles;
        curEulerY = camEulerAngles.y;
        curEulerX = camEulerAngles.x;

        // 2. Rotation
        camRotation = Camera.main.transform.rotation;
        curRotY = camRotation.y;
        curRotX = camRotation.x;

        // 3. converted euler
        if (curEulerY >= 180 && curEulerY <= 360) // 360-270-180 => 0-90-180
        {
            conv_curEulerY = 360f - curEulerY;
        }
        else if (curEulerY > 0 && curEulerY < 180) // 1-90-179 => -1 -90 -179
        {
            conv_curEulerY = curEulerY * (-1f);
        }

        if(curEulerX >= 180 && curEulerX <= 360)
        {
            conv_curEulerX = 360f - curEulerX;
        }
        else if (curEulerX > 0 && curEulerX < 180) // 1-90-179 => -1 -90 -179
        {
            conv_curEulerX = curEulerX * (-1f);
        }

        if (!canMeasure)
        {
            if (!inGame)
            {
                prevEulerY = curEulerY;
                prevRotY = curRotY;
                // 360-270-180 => 0-90-180
                if (prevEulerY >= 180 && prevEulerY <= 360)
                    conv_prevEulerY = 360f - prevEulerY;
                else if (prevEulerY > 0 && prevEulerY < 180) // 1-90-179 => -1 -90 -179
                    conv_prevEulerY = prevEulerY * (-1f);
                //Debug.Log("Before Starting Game: " 
                //    + " prevEuler:" + prevEulerY 
                //    + " prevRot: " + prevRotY 
                //    + " conv_Y: " + conv_prevEulerY);
            }
        }
        else 
        {
            // Time
            checkTimer += Time.fixedDeltaTime; // 0f - 0.2f
             
            if (checkTimer >= period) // every 0.2 secs
            {
                count += 1;
                if (count > 0)
                {
                    
                    diffEulerY = Mathf.Abs(prevEulerY - curEulerY);
                    diffEulerX = Mathf.Abs(prevEulerX - curEulerX);
                    // Converted
                    conv_diffEulerY = Mathf.Abs(conv_prevEulerY - conv_curEulerY);
                    conv_diffEulerX = Mathf.Abs(conv_prevEulerX - conv_curEulerX);

                    sumEulerY += diffEulerY;
                    sumEulerX += diffEulerX;
                    conv_sumEulerY += conv_diffEulerY;
                    conv_sumEulerX += conv_diffEulerX;
            
                    avgEulerY = sumEulerY / count;
                    avgEulerX = sumEulerX / count;
                    conv_avgEulerY = conv_sumEulerY / count;
                    conv_avgEulerX = conv_sumEulerX / count;

                    string logMsgForHorizontalHM = "Time:" + Time.time + 
                        " period: " + checkTimer +
                        " prev:" + conv_prevEulerY + "("+prevEulerY+") " +
                        "cur: " + conv_curEulerY +"("+ curEulerY+ ") " +
                        "diff: " + conv_diffEulerY + "("+ diffEulerY+") " +
                        "count: " + count + 
                        " sum: " + conv_sumEulerY +"("+ sumEulerY + ") " + 
                        " avg: " + conv_avgEulerY + "("+avgEulerY + ")";

                    string logMsgForVerticalHM = "Time:" + Time.time +
                        " period: " + checkTimer +
                        " prev:" + conv_prevEulerX + "(" + prevEulerX + ") " +
                        "cur: " + conv_curEulerX + "(" + curEulerX + ") " +
                        "diff: " + conv_diffEulerX + "(" + diffEulerX + ") " +
                        "count: " + count +
                        " sum: " + conv_sumEulerX + "(" + sumEulerX + ") " +
                        " avg: " + conv_avgEulerX + "(" + avgEulerX + ")";

                    //Debug.Log(logMsgForHorizontalHM + "\n" + logMsgForVerticalHM);

                    conv_prevEulerY = conv_curEulerY;
                    conv_prevEulerX = conv_curEulerX;
                    prevEulerY = curEulerY;
                    prevEulerX = curEulerX;
                    logManager.WriteLogForHorizontalHeadMovement(logMsgForHorizontalHM);
                    logManager.WriteLogForVerticalHeadMovement(logMsgForVerticalHM);
                }

                if (count > 0)
                {
                    //conv_prevEulerY = conv_curEulerY;
                    //prevEulerY = curEulerY;
                }
                checkTimer = 0f;
            }
          //  prevEulerY = curEulerY;
        }
    }

    public float GetHorizontalHeadMovement()
    {
       // string value = avgYAxis.ToString();
        return conv_avgEulerY;
    }

    public float GetVerticalHeadMovement()
    {
        return conv_avgEulerX;
    }
}
