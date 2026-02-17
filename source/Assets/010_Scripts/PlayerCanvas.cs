using System;
using System.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCanvas : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI remainTimeText;
    [SerializeField]
    private HorizontalLayoutGroup battery;
    [SerializeField]
    private Image collectingBar;
    [SerializeField]
    private TextMeshProUGUI collectingText;
    [SerializeField]
    private Color batteryColorHigh = Color.green;
    [SerializeField]
    private Color batteryColorMiddle = Color.yellow;
    [SerializeField]
    private Color batteryColorLow = Color.red;
    private Animator animator;

    private GameManager gameManager;

    void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        SetTime(gameManager.GetElapsedTime(), gameManager.GetLimitTime());
        SetCollecting(gameManager.GetCollectedDustNum(), gameManager.GetMaxDustNum());
    }

    public void SetTime(float current, float limit)
    {
        float remain = limit - current;
        string timeString = string.Format("{0:D2}:{1:D2}:{2:D2}",
            (int)remain / 60,
            (int)remain % 60,
            (int)(remain * 100) % 60);
        remainTimeText.text = timeString;

        float percentage = remain / limit;
        int batteryRemain = (int)Mathf.Ceil(percentage / 0.2f);
        Image[] images = battery.GetComponentsInChildren<Image>();
        for (int i = 0; i < images.Length; i++)
        {
            Color color = batteryColorLow;
            if (batteryRemain > 2)
            {
                color = batteryColorHigh;
            }
            else if(batteryRemain > 1)
            {
                color = batteryColorMiddle;
            }
            
            if (i < batteryRemain)
            {
                color.a = 1.0f;
            }
            else {
                color.a = 0.0f;
            }
            images[i].color = color;
        }
    }

    public void SetCollecting(int current, int max)
    {
        float percentage = (float)current / max;
        collectingText.text = Mathf.Round(percentage * 100) + "%";

        collectingBar.fillAmount = percentage;

    }

    public void PlayStartCall()
    {
        Debug.Log("PlayStartCall");
        animator.enabled = true;
        animator.Play("Start");
    }

    public void PlayClearCall()
    {
        animator.enabled = true;
        animator.Play("Clear");
    }

    public void PlayGameOverCall()
    {
        animator.enabled = true;
        animator.Play("GameOver");
    }

    public void OnStartCallFinish()
    {
        gameManager.OnStartCallFinish();
    }

    public void OnClearCallFinish()
    {
        gameManager.OnClearCallFinish();
    }

    public void OnGameOverCallFinish()
    {
        gameManager.OnGameOverCallFinish();
    }


}
