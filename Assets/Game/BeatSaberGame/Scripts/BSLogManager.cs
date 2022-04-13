using UnityEngine;
using System;
using System.Globalization;

public class BSLogManager : MonoBehaviour
{
    UserStudyManager userstudyManager;
    private string participantID;
    private string currentDateAndTime;
    private string currentTime;
    private string currentTimeinMilliseconds;

    private void Awake()
    {
        userstudyManager = FindObjectOfType<UserStudyManager>();

        participantID = userstudyManager.GetParticipantID();
        currentDateAndTime = GetCurrentDateAndTime();
    }

    void Start()
    {
        //participantID = userstudyManager.GetID();
        //currentDateAndTime = GetCurrentDateAndTime();
        
        WriteLogFile("==============GAME LOG=======================================" +
           "\nID: " + participantID+
           "\nLOG STRAT: " + currentDateAndTime);

        WriteLogForHorizontalHeadMovement("===============HEAD MOVEMENT (Horizontal)========================" +
          "\nID: " + participantID +
          "\nLOG STRAT: " + currentDateAndTime);

        WriteLogForVerticalHeadMovement("===============HEAD MOVEMENT (Vertical)========================" +
          "\nID: " + participantID +
          "\nLOG STRAT: " + currentDateAndTime);

        WriteLogForEyeGaze("===============Eye Gaze======================================" +
          "\nID: " + participantID +
          "\nLOG STRAT: " + currentDateAndTime);

        WriteLogForVRUserHead("===============HEAD MAX. & MIN.======================================" +
          "\nID: " + participantID +
          "\nLOG STRAT: " + currentDateAndTime);
    }

    public void WriteLogFile(string message)
    {
        using (System.IO.StreamWriter logFile =
            new System.IO.StreamWriter(@"C:\Users\ru35qac\Desktop\LogFiles\LogFile_" + participantID + ".txt", append: true))
        {
            currentTime = GetCurrentTime();
            logFile.Write("[" + currentTime + "] ");
            logFile.WriteLine(message);
        }
    }

    public void WriteLogForHorizontalHeadMovement(string message)
    {
        using (System.IO.StreamWriter logFile =
           new System.IO.StreamWriter(@"C:\Users\ru35qac\Desktop\LogFiles\LogFile_" + participantID + "_HeadMovement_Hor.txt", append: true))
        {
            currentTime = GetCurrentTime();
            logFile.Write("[" + currentTime + "] ");
            logFile.WriteLine(message);
        }
    }

    public void WriteLogForVerticalHeadMovement(string message)
    {
        using (System.IO.StreamWriter logFile =
           new System.IO.StreamWriter(@"C:\Users\ru35qac\Desktop\LogFiles\LogFile_" + participantID + "_HeadMovement_Ver.txt", append: true))
        {
            currentTime = GetCurrentTime();
            logFile.Write("[" + currentTime + "] ");
            logFile.WriteLine(message);
        }
    }

    public void WriteLogForVRUserHead(string message)
    {
        using (System.IO.StreamWriter logFile =
                   new System.IO.StreamWriter(@"C:\Users\ru35qac\Desktop\LogFiles\LogFile_" + participantID + "_Head.txt", append: true))
        {
            currentTime = GetCurrentTime();
            logFile.Write("[" + currentTime + "] ");
            logFile.WriteLine(message);
        }
    }

    public void WriteLogForEyeGaze(string message)
    {
        using (System.IO.StreamWriter logFile =
                   new System.IO.StreamWriter(@"C:\Users\ru35qac\Desktop\LogFiles\LogFile_" + participantID + "_Gaze.txt", append: true))
        {
            currentTime = GetCurrentTime();
            currentTimeinMilliseconds = GetCurrentTimeMilliseconds(); // For more correct measurement
            logFile.Write("[" + currentTimeinMilliseconds + "] ");
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

    private string GetCurrentTimeMilliseconds()
    {
        DateTime localDate = DateTime.Now;
        string name = localDate.ToString("HH:mm:ss:ms");

        return name;
    }
}
