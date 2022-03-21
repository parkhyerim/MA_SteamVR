using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BSLogManager : MonoBehaviour
{
    private string participantID;

    public BeatSaberGameManager bsGameManager;

    // Start is called before the first frame update
    void Start()
    {
        if (bsGameManager.participantID != null)
            participantID = bsGameManager.participantID;

        //if (participantID == null || participantID == "")
        //    participantID = "not assinged";
        WriteToLogFile("ID: " + participantID);
    }
    public void WriteToLogFile(string message)
    {
        using (System.IO.StreamWriter logFile =
            new System.IO.StreamWriter(@"C:\Users\ru35qac\Desktop\LogFiles\LogFile_" + participantID + ".txt", append: true))
        {
            // logFile.WriteLine(DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + ": \n");
            logFile.WriteLine(message);
        }
    }
}
