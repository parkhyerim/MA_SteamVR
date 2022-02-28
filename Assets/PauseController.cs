using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseController : MonoBehaviour
{
    public InputActionReference pauseReference = null;
    public AudioSource audioSource;
    public AudioClip quesitionAudio;
    public AudioSource bgMusicAS;
    bool pauseClicked;
    bool onceClicked;
    public GameManager gameManager;
    public BystanderAvatar bAvatar;

    private void Awake()
    {
        pauseReference.action.started += PauseGame;
    }

    private void OnDestroy()
    {
        pauseReference.action.started -= PauseGame;
    }

    private void PauseGame(InputAction.CallbackContext context)
    {
        // bool isActive = !gameObject.activeSelf;
        // gameObject.SetActive(isActive);
        if (gameManager.CanPause)
        {
            if (!pauseClicked)
            {
                bgMusicAS.Pause();
             //   Debug.Log("pause clicked");
                if (!onceClicked && gameManager.BystanderInteract)
                {
                    if (bAvatar.doInteraction )
                        Invoke(nameof(PlayQuestionAudio), 1f);
                    onceClicked = true;
                }
                pauseClicked = true;
                gameManager.PauseGameTime();
            }
            else
            {
               // Debug.Log("resume clciked");
                bgMusicAS.UnPause();
                gameManager.PauseGameTime();
                pauseClicked = false;
            }
        }      
    }

    public void PlayQuestionAudio()
    {
        audioSource.PlayOneShot(quesitionAudio);
    }
}
