using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeLog : MonoBehaviour
{
    HeadMovement headMovement;
    BSLogManager logManager;
    BSGameManager gameManager;

    private float period = 0.2f;
    private float checkTimer = 0.0f;
    private int count;

    bool gameStart;

    float headUpMaxDegrees, headDownDegrees, headLeftDegrees, headRightDegrees;
    float curEulerY, curEulerX, curEulerZ;
    float curPosX, curPosY, curPosZ;
    bool paused, resumed;
    public bool GameStart { get => gameStart; set => gameStart = value; }

    private void Awake()
    {
        headMovement = FindObjectOfType<HeadMovement>();
        logManager = FindObjectOfType<BSLogManager>();
        gameManager = FindObjectOfType<BSGameManager>();
    }

    private void Start()
    {
        logManager.WriteLogForExcel(
            "Timestamp" 
            + " HeadUp" + " HeadDown" + " HeadLeft" + " HeadRight"
            + " RotX" + " RotY" + " RotZ" + " PosX" + " PosY" + " PosZ"
            + " GazeX" + " GazeY" + " GazeZ" 
            + " Pause" + " Resume", true);
    }

    private void FixedUpdate()
    {
        checkTimer += Time.fixedDeltaTime;

        if (!GameStart)
        {
            
        }
        else
        {
            checkTimer += Time.fixedDeltaTime;
            if (checkTimer >= period)
            {
                headUpMaxDegrees = gameManager.MaxUpAxis;
                headDownDegrees = gameManager.MaxDownAxis;
                headLeftDegrees = gameManager.MaxLeftAxis;
                headRightDegrees = gameManager.MaxRightAxis;
                curEulerY = headMovement.Conv_curEulerY;
                curEulerX = headMovement.Conv_curEulerX;
                curEulerZ = headMovement.Conv_curEulerZ;
                curPosX = headMovement.HeadsetPosX;
                curPosY = headMovement.HeadsetPosY;
                curPosZ = headMovement.HeadsetPosZ;
                paused = gameManager.GamePaused;

                // logManager.WriteLogForExcel(checkTimer + " check", false);
                logManager.WriteLogForExcel(headUpMaxDegrees + ", " + headDownDegrees + ", " + headLeftDegrees + ", " + headRightDegrees + ", "
                    + curEulerX + ", " + curEulerY + ", " + curEulerZ + ", "
                    + curPosX + ", " + curPosY + ", " + curPosZ + ", "
                    + "GAZE" + ", "
                    + paused + ", "
                    , false);

                checkTimer = 0f;
            }
        }
    }

}
