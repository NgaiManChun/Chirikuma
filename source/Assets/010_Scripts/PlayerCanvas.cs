using System;
using System.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCanvas : MonoBehaviour
{

    // =======================================================
    // UI参照
    // =======================================================

    [SerializeField]
    private TextMeshProUGUI remainTimeText;
    [SerializeField]
    private HorizontalLayoutGroup battery;
    [SerializeField]
    private Image collectingBar;
    [SerializeField]
    private TextMeshProUGUI collectingText;

    // バッテリー色（残量に応じて切替）
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
        // GameManagerとAnimatorを取得
        gameManager = FindFirstObjectByType<GameManager>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // 毎フレームUI更新
        SetTime(gameManager.GetElapsedTime(), gameManager.GetLimitTime());
        SetCollecting(gameManager.GetCollectedDustNum(), gameManager.GetMaxDustNum());
    }

    // =======================================================
    // 残り時間・バッテリー表示更新
    // =======================================================
    public void SetTime(float current, float limit)
    {
        // 残り時間（秒）
        float remain = limit - current;

        // 時間フォーマット
        string timeString = string.Format("{0:D2}:{1:D2}:{2:D2}",
            (int)remain / 60,
            (int)remain % 60,
            (int)(remain * 100) % 60);
        remainTimeText.text = timeString;

        // 残量割合（0～1）
        float percentage = remain / limit;

        // 20%ごとに1コマとして換算
        int batteryRemain = (int)Mathf.Ceil(percentage / 0.2f);

        // バッテリーUI（子Image）取得
        Image[] images = battery.GetComponentsInChildren<Image>();

        for (int i = 0; i < images.Length; i++)
        {
            // 残量に応じて色を決定
            Color color = batteryColorLow;
            if (batteryRemain > 2)
            {
                color = batteryColorHigh;
            }
            else if(batteryRemain > 1)
            {
                color = batteryColorMiddle;
            }

            // 残量分だけ表示、それ以外は非表示（α=0）
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

    // =======================================================
    // 収集率表示更新
    // =======================================================
    public void SetCollecting(int current, int max)
    {
        // 収集率（0～1）
        float percentage = (float)current / max;

        // パーセンテージ表示
        collectingText.text = Mathf.Round(percentage * 100) + "%";

        // ゲージ更新
        collectingBar.fillAmount = percentage;

    }

    // =======================================================
    // 各種コール演出
    // =======================================================
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

    // =======================================================
    // アニメーションイベントから呼ばれる
    // =======================================================

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
