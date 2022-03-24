using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Linq;

public class BSPauseController : MonoBehaviour
{
    public InputActionReference pauseReference = null;
    public AudioSource audioSource;
    public AudioClip[] quesitionAudios;
    public AudioSource bgMusicAS;
    public BeatSaberGameManager gameManager;
    public BSBystanderAvatar bAvatar;
    public BSLogManager logManager;
    [SerializeField]
    private bool[] audioPlayed;
    bool pauseClicked;
    private bool oncePausedInSession;
    public int[] audioOrder = { 1, 2, 3 };
    private int counter;
    public bool OncePausedInSession { get => oncePausedInSession; set => oncePausedInSession = value; }

    private void Awake()
    {
        pauseReference.action.started += PauseGame;
        audioPlayed = new bool[3] { false, false, false };
        //Debug.Log(audioOrder);
        //if (audioOrder == null)
        //{
        //    Debug.Log("audio order is null");
        //}
    }

    private void OnDestroy()
    {
        pauseReference.action.started -= PauseGame;
    }

    private void PauseGame(InputAction.CallbackContext context)
    {
        // bool isActive = !gameObject.activeSelf;
        // gameObject.SetActive(isActive);
        if (gameManager.CanPauseGame)
        {
            if (!pauseClicked)
            {
                bgMusicAS.Pause();

                if (gameManager.BystanderInteract)
                {
                    if (!oncePausedInSession && gameManager.BystanderCanHearAnswer)
                    {
                        //  logManager.WriteToLogFile("Identify time");
                        // if (bAvatar.doInteraction)
                        // Invoke(nameof(PlayQuestionAudio), 1f);
                        //if (audioPlayed.Contains(false))
                        //{
                        //    // Debug.Log("still false in array");
                        //    Invoke(nameof(PlayQuestionAudio), 1f);
                        //}
                        // Invoke(nameof(PlayQuestionAudio), 1f);
                        Invoke(nameof(PlayQuestionAudio), 1f);
                        oncePausedInSession = true;
                    }                   
                }

                //if (audioPlayed.Contains(false))
                //{
                //   // Debug.Log("still false in array");
                //    Invoke(nameof(PlayQuestionAudio), 1f);
                //}
            
                pauseClicked = true;
                gameManager.PauseGame();
            }
            else
            {
                bgMusicAS.UnPause();
                gameManager.PauseGame();
                pauseClicked = false;
                // logManager.WriteToLogFile("Resume the game again (" + DateTime.Now.ToShortTimeString() + ")");
            }
        }
    }

    public void PlayQuestionAudio()
    {
        // int index;
        // index = GetRandomNumber(quesitionAudios.Length);
        //// Debug.Log("random: " + index);

        // if (audioPlayed[index] == false)
        // {
        //     audioPlayed[index] = true;
        //     audioSource.PlayOneShot(quesitionAudios[index]);
        // }        
        // else
        //     PlayQuestionAudio();
        if (counter < 3)
        {
            int index = audioOrder[counter] - 1;
            audioSource.PlayOneShot(quesitionAudios[index]);
            counter++;

        }
    }

    private int GetRandomNumber(int length)
    {
        var rnd = new System.Random();
        int index = rnd.Next(0, length);
        return index;
    }
}
