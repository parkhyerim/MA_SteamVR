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
    float gazeXAni, gazeYAni, gazeZAni, gazeXAva, gazeYAva, gazeZava;
    bool paused, resumed;
    bool gazeAnimoji, gazeAvatar, gazeUI, gazeScore, gazeTimer, gazeCubearea;
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
         "Timestamp(s:ms),"
         + "HeadUpMax(-(deg.))," + " HeadDownMax(deg.)," + " HeadLeft(-(deg.))," + " HeadRight(deg.),"
         + "RotationX(deg.)," + "RotationY(deg.)," + "RotationZ(deg.),"
         + "PositionX," + " PosítionY," + " PositionZ,"
         + "GazePosX_Animoji," + "GazePosY_Animoji," + "GazePosZ_Animoji,"
         + "GazePosX_Avatar," + "GazePosY_Avatar," + "GazePosZ_Avatar,"
         + "GazeAnimoji(bool)," + "GazeAvatar(bool)," + "GazeCubeArea(bool)," + "GazeUI(bool)," + "GazeScore(bool)," + "GazeTimer(bool),"
         + "Pause(bool)," + "Resume(bool)", true);
    }

    private void FixedUpdate()
    {
        checkTimer += Time.fixedDeltaTime;

        if (!GameStart)
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
            gazeAnimoji = gameManager.gazeAnimoji;
            gazeAvatar = gameManager.gazeAvatar;
            gazeCubearea = gameManager.gazeCubes;
            gazeScore = gameManager.gazeScore;
            gazeTimer = gameManager.gazeTimer;
            gazeUI = gameManager.gazeUI;
            gazeXAni = gameManager.AnimojiGazeTransform.position.x;
            gazeYAni = gameManager.AnimojiGazeTransform.position.y;
            gazeZAni = gameManager.AnimojiGazeTransform.position.z;
            gazeXAva = gameManager.AvatarGazeTransform.position.x;
            gazeYAva = gameManager.AvatarGazeTransform.position.y;
            gazeZava = gameManager.AvatarGazeTransform.position.z;
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
                gazeAnimoji = gameManager.gazeAnimoji;
                gazeAvatar = gameManager.gazeAvatar;
                gazeCubearea = gameManager.gazeCubes;
                gazeScore = gameManager.gazeScore;
                gazeTimer = gameManager.gazeTimer;
                gazeUI = gameManager.gazeUI;
                gazeXAni = gameManager.AnimojiGazeTransform.position.x;
                gazeYAni = gameManager.AnimojiGazeTransform.position.y;
                gazeZAni = gameManager.AnimojiGazeTransform.position.z;
                gazeXAva = gameManager.AvatarGazeTransform.position.x;
                gazeYAva = gameManager.AvatarGazeTransform.position.y;
                gazeZava = gameManager.AvatarGazeTransform.position.z;


                // logManager.WriteLogForExcel(checkTimer + " check", false);
                logManager.WriteLogForExcel(
                    headUpMaxDegrees + "," + headDownDegrees + "," + headLeftDegrees + "," + headRightDegrees + ","
                    + curEulerX + "," + curEulerY + "," + curEulerZ + ","
                    + curPosX + "," + curPosY + "," + curPosZ + ","
                    + gazeXAni + "," + gazeYAni + "," + gazeZAni + ","
                     + gazeXAva + "," + gazeYAva + "," + gazeZava + ","
                    + gazeAnimoji +","+ gazeAvatar +"," + gazeCubearea +"," + gazeUI+"," + gazeScore +"," + gazeTimer +","
                    + paused + ", "
                    , false);

                checkTimer = 0f;
            }
        }
    }

}
