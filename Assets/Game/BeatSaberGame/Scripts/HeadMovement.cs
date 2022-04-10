using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class HeadMovement : MonoBehaviour
{
    public float period = 0.2f;
    [SerializeField]
    private float checkTimer = 0.0f;
    [SerializeField]
    private Vector3 camEulerAngles;
    [SerializeField]
    private Quaternion camRotation;
    [SerializeField]
    private float curEulerY, prevEulerY, diffEulerY, avgEulerY, sumEulerY;
    [SerializeField]
    private float conv_curEulerY, conv_prevEulerY, conv_diffEulerY, conv_avgEulerY, conv_sumEulerY;
    [SerializeField]
    private float curRotY, prevRotY, diffRotY, avgRotY, sumRotY;
    [SerializeField]
    private int num;

    bool canMeasure;
    bool inGame;
    BSLogManager logManager;

    public bool CanMeasure { get => canMeasure; set => canMeasure = value; }
    public bool InGame { get => inGame; set => inGame = value; }

    private void Awake()
    {
        logManager = FindObjectOfType<BSLogManager>();
    }
    // Start is called before the first frame update
    void Start()
    {
        num = -1;

        // Euler Angles
        camEulerAngles = Camera.main.transform.eulerAngles;
       // prevEulerY = camEulerAngles.y;
        curEulerY = camEulerAngles.y;

        // Rotation Angles
        camRotation = Camera.main.transform.rotation;
        curRotY = camRotation.y;       
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // logManager.WriteToLogFileForHeadMovement("test");

        // Measure Head Movement
        // 1.Euler
        camEulerAngles = Camera.main.transform.eulerAngles;
        curEulerY = camEulerAngles.y;

        // 2. Rotation
        camRotation = Camera.main.transform.rotation;
        curRotY = camRotation.y;

        // 3. converted euler
        if (curEulerY >= 180 && curEulerY <= 360) // 360-270-180 => 0-90-180
        {
            conv_curEulerY = 360f - curEulerY;
        }
        else if (curEulerY > 0 && curEulerY < 180) // 1-90-179 => -1 -90 -179
        {
            conv_curEulerY = curEulerY * (-1f);
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
                num += 1;
                if (num > 0)
                {
                    
                    diffEulerY = Mathf.Abs(prevEulerY - curEulerY);
                    // Converted
                    conv_diffEulerY = Mathf.Abs(conv_prevEulerY - conv_curEulerY);

                    sumEulerY += diffEulerY;
                    conv_sumEulerY += conv_diffEulerY;
            

                    avgEulerY = sumEulerY / num;
                    conv_avgEulerY = conv_sumEulerY / num;

                    string logMsg = "Time:" + Time.time + 
                        " period: " + checkTimer +
                        " prev:" + conv_prevEulerY + "("+prevEulerY+") " +
                        "cur: " + conv_curEulerY +"("+ curEulerY+ ") " +
                        "diff: " + conv_diffEulerY + "("+ diffEulerY+") " +
                        "num: " + num + 
                        " sum: " + conv_sumEulerY +"("+ sumEulerY + ") " + 
                        " avg: " + conv_avgEulerY + "("+avgEulerY + ")";

                    Debug.Log("Time:" + Time.time + " period: " + checkTimer +
                        " prev:" + conv_prevEulerY + " cur: " + conv_curEulerY + " diffYAxis: " + conv_diffEulerY +
                        " num: " + num + " sum: " + conv_sumEulerY + " avg: " + conv_avgEulerY);
                    // previousYAxis = currentYAxis;
                    conv_prevEulerY = conv_curEulerY;
                    prevEulerY = curEulerY;
                    logManager.WriteLogForHeadMovement(logMsg);
                }

                if (num > 0)
                {
                    //conv_prevEulerY = conv_curEulerY;
                    //prevEulerY = curEulerY;
                }
                checkTimer = 0f;
            }
          //  prevEulerY = curEulerY;
        }
    }

    public float GetResultHeadMovement()
    {
       // string value = avgYAxis.ToString();
        return conv_avgEulerY;
    }
}
