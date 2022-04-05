using UnityEngine;
using System;
using System.Globalization;

public class BSLogManager : MonoBehaviour
{
    BeatSaberGameManager bsGameManager;
    UserStudyManager userstudyManager;
    private string participantID;
    private string currentDateTime;
    private string currentTime;

    private void Awake()
    {
        userstudyManager = FindObjectOfType<UserStudyManager>();
        bsGameManager = FindObjectOfType<BeatSaberGameManager>();
    }

    void Start()
    {
        //if (bsGameManager.participantID != "" || bsGameManager.participantID != null)
        //    participantID = bsGameManager.participantID;
        //else
        //    participantID = "not assigned";

        participantID = userstudyManager.GetID();
        currentDateTime = GetCurrentDateTime();
        
        WriteToLogFile("=================================\n" +
            "LOG STRAT: "+ currentDateTime +
            "\nID: " + participantID);
    }

    public void WriteToLogFile(string message)
    {
        using (System.IO.StreamWriter logFile =
            new System.IO.StreamWriter(@"C:\Users\ru35qac\Desktop\LogFiles\LogFile_" + participantID + ".txt", append: true))
        {
            currentDateTime = GetCurrentDateTime();
            currentTime = GetCurrentTime();
            logFile.Write("[" + currentTime + "] ");
            logFile.WriteLine(message);
           // logFile.WriteLine("              [" + currentDateTime + "] "); 
        }
    }

    private string GetCurrentDateTime()
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
       // string cultureName = "de-DE"; // de-DE  en-GB en-US
      //  var culture = new CultureInfo(cultureName);
        string name = localDate.ToString("HH:mm:ss");

        return name;
    }
}
