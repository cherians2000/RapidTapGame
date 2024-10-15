using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class OnGameOverScript : MonoBehaviour
{
    
    public Image bgImage;
    public int Target;
    public static bool isClicked = false;
    public GameObject WinPanel;
    public GameObject LossPanel;

    AudioSource audioSource;
    public AudioClip winClip;
    public AudioClip lossClip;
    void Start()
    {
        isClicked = false; 
        audioSource = FindAnyObjectByType<AudioSource>();
    }
    public void GameOver()
    {
        

        if (isClicked)
        {
            Debug.Log("Loss due to click.");
            bgImage.color = Color.red;
            WinPanel.SetActive(false);
            LossPanel.SetActive(true);
        }
        else if (UIManager.Instance.score >= Target)
        {
            Debug.Log("Win! Score is greater than or equal to target.");
            WinPanel.SetActive(true);
            LossPanel.SetActive(false);
            audioSource.PlayOneShot(winClip);
        }
        else
        {
            Debug.Log("Loss due to score being lower than target.");
            bgImage.color = Color.red;
            WinPanel.SetActive(false);
            LossPanel.SetActive(true);
            audioSource.PlayOneShot(lossClip);
        }
    }

}
