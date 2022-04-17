using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeLog : MonoBehaviour
{
    HeadMovement headMovement;
    BSLogManager logManager;

    private float period = 0.2f;
    private float checkTimer = 0.0f;
    private int count;

    bool gameStart;

    public bool GameStart { get => gameStart; set => gameStart = value; }

    private void Awake()
    {
        headMovement = FindObjectOfType<HeadMovement>();
        logManager = FindObjectOfType<BSLogManager>();
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
        if (checkTimer >= period)
        {
            logManager.WriteLogForExcel(checkTimer + " check", false);
            checkTimer = 0f;
        }
        //if (!GameStart)
        //{
        //    logManager.WriteLogForExcel("check");
        //}
        //else
        //{
        //    checkTimer += Time.fixedDeltaTime;
        //    if(checkTimer >= period)
        //    {
        //        logManager.WriteLogForExcel(checkTimer+ " check");
        //        checkTimer = 0f;
        //    }

        //}
    }

}
