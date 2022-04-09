using UnityEngine;
using System;
using System.Globalization;

public class BSLogManager : MonoBehaviour
{
    BeatSaberGameManager bsGameManager;
    UserStudyManager userstudyManager;
    private string participantID;
    private string currentDateAndTime;
    private string currentTime;

    private void Awake()
    {
        userstudyManager = FindObjectOfType<UserStudyManager>();
        bsGameManager = FindObjectOfType<BeatSaberGameManager>();
    }

    void Start()
    {
        participantID = userstudyManager.GetID();
        currentDateAndTime = GetCurrentDateAndTime();
        
        WriteToLogFile("=====================================================" +
            "\nLOG STRAT: " + currentDateAndTime +
            "\nID: " + participantID);

        WriteToLogFileForHeadMovement("=====================================================" +
          "\nLOG STRAT: " + currentDateAndTime +
          "\nID: " + participantID);
    }

    public void WriteToLogFile(string message)
    {
        using (System.IO.StreamWriter logFile =
            new System.IO.StreamWriter(@"C:\Users\ru35qac\Desktop\LogFiles\LogFile_" + participantID + ".txt", append: true))
        {
            // currentDateAndTime = GetCurrentDateAndTime();
            currentTime = GetCurrentTime();
            logFile.Write("[" + currentTime + "] ");
            logFile.WriteLine(message);
        }
    }

    public void WriteToLogFileForHeadMovement(string message)
    {
        using (System.IO.StreamWriter logFile =
           new System.IO.StreamWriter(@"C:\Users\ru35qac\Desktop\LogFiles\LogFile_" + participantID + "_HeadMovement.txt", append: true))
        {
            // currentDateAndTime = GetCurrentDateAndTime();
            currentTime = GetCurrentTime();
            logFile.Write("[" + currentTime + "] ");
            logFile.WriteLine(message);
        }
    }
    private string GetCurrentDateAndTime()
    {
        DateTime localDate = DateTime.Now;
        string cultureName = "de-DE"; // de-DE  en-GB en-US
        var culture = new CultureInfo(cultureName);
        string name = localDate.ToString(culture);

        return name;
    }

    private string GetCurrentTime()
    {
        DateTime localDate = DateTime.Now;
        string name = localDate.ToString("HH:mm:ss");

        return name;
    }
}
