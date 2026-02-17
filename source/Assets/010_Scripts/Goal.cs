using NUnit.Framework;
using System;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Goal : MonoBehaviour
{
    [SerializeField]
    private GameObject dustPrefab;
    [SerializeField]
    private Transform dustInsertPoint;
    [SerializeField]
    private Camera farCamera;
    [SerializeField]
    private Camera nearCamera;
    [SerializeField]
    private Image maxChart;
    [SerializeField]
    private Image collectedChart;
    [SerializeField]
    private TextMeshProUGUI collectedText; 
    [SerializeField] 
    private float chartSpeed = 1.0f;
    [SerializeField]
    private float starDelay = 0.3f;
    [SerializeField]
    private Animator buttonAnimator;
    [SerializeField]
    private List<Animator> starAnimators = new List<Animator>();

    private bool isShowResult = false;

    private int collectedNum = 0;
    private float maxChartT = 0.0f;
    private float collectedCurrent = 0.0f;
    private float collectedPercentage = 0.0f;
    private int starNum = 0;
    private bool isPlayedStarAnimation = false;

    private GameManager gameManager;
    public Camera FarCamera => farCamera;
    public Camera NearCamera => nearCamera;

    void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        foreach (Animator starAnimator in starAnimators)
        {
            starAnimator.enabled = false;
            starAnimator.gameObject.GetComponent<RectTransform>().localScale = Vector3.zero;
        }
        buttonAnimator.enabled = false;
        buttonAnimator.gameObject.GetComponent<RectTransform>().localScale = Vector3.zero;
    }

    void Update()
    {
        if (isShowResult)
        {
            collectedCurrent = Mathf.Min(collectedCurrent + Time.deltaTime * chartSpeed, collectedPercentage);
            maxChartT = Mathf.Min(maxChartT + Time.deltaTime * chartSpeed, 1.0f);

            maxChart.fillAmount = maxChartT;
            collectedChart.fillAmount = collectedCurrent;
            collectedText.text = Mathf.Round(collectedCurrent * 100) + "%";
            if (maxChartT == 1.0f && !isPlayedStarAnimation)
            {
                isPlayedStarAnimation = true;
                for(int i = 0; i < starNum; i++)
                {
                    Invoke("PlayStarAnimation", i * starDelay);
                    starAnimators[i].enabled = true;
                }
                Invoke("PlayButtonAnimation", starNum * starDelay);
            }
        }
    }

    public void PlayStarAnimation()
    {
        foreach(Animator animator in starAnimators)
        {
            if (!animator.enabled)
            {
                animator.Play("FillStar");
                break;
            }
        }
    }

    public void PlayButtonAnimation()
    {
        buttonAnimator.enabled = true;
        buttonAnimator.Play("ScaleButton");
    }

    public void SpawnDust()
    {
        if(collectedNum-- > 0)
        {
            Instantiate(dustPrefab, dustInsertPoint.position + new Vector3(Random.value, Random.value, 0.0f), dustInsertPoint.rotation);
            Invoke("SpawnDust", 0.2f);
        }
    }

    public void ShowResult()
    {
        bool clear = true;
        foreach (Rumba cleaner in gameManager.cleaners)
        {
            clear = clear && cleaner.IsInGoal();
        }

        if (clear)
        {
            collectedNum = gameManager.GetCollectedDustNum();
            collectedPercentage = gameManager.GetCollectedPercentage();
            starNum = (int)Mathf.Floor(3.0f * collectedPercentage);
            SpawnDust();
        }
        else
        {
            collectedNum = 0;
            collectedPercentage = 0;
            starNum = 0;
        }
        isShowResult = true;
    }

    public void OnClickNext()
    {
        Original.SceneManager.instance.SceneLoad("Title");
    }
}
