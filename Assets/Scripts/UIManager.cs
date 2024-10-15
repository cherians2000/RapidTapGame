using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public TextMeshProUGUI ScoreText;
    
    public int score;
    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void UpdateScore()
    {
        score++;
        ScoreText.text = score.ToString();
    }

    public void resetScore()
    {
        score = 0;
        ScoreText.text = score.ToString();
    }
}
