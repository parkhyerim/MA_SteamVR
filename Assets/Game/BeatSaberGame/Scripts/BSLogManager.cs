using UnityEngine;
using System;
using System.Globalization;

public class BSLogManager : MonoBehaviour
{
    public BeatSaberGameManager bsGameManager;
    private string participantID;
    private string currentDateTime;
    private string currentTime;
    void Start()
    {
        if (bsGameManager.participantID != "" || bsGameManager.participantID != null)
            participantID = bsGameManager.participantID;
        else
            participantID = "not assigned";

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
            logFile.Write(message);
           // logFile.WriteLine("              [" + currentDateTime + "] "); 
            logFile.WriteLine("              [" + currentTime + "] ");
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
