using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TextScore : MonoBehaviour
{

    private Text ScoreText;
    public int Score;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        ScoreText = GetComponentInChildren<Text>();
        ScoreText.text = "0 %";
        Score = 0;
    }

    // Update is called once per frame
    void Update()
    {
        ScoreText.text = Score.ToString() + "%";



    }

}
