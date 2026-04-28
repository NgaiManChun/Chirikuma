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
    // =======================================================
    // 参照設定
    // =======================================================

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

    // =======================================================
    // 内部状態
    // =======================================================

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
        // GameManager取得
        gameManager = FindFirstObjectByType<GameManager>();

        // リザルトUIは最初は非表示・停止状態にする
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
        // リザルト表示中のみ円グラフを更新
        if (isShowResult)
        {
            // 回収率グラフを目標値まで徐々に伸ばす
            collectedCurrent = Mathf.Min(collectedCurrent + Time.deltaTime * chartSpeed, collectedPercentage);

            // 最大値グラフは1.0まで伸ばす
            maxChartT = Mathf.Min(maxChartT + Time.deltaTime * chartSpeed, 1.0f);

            // UIへ反映
            maxChart.fillAmount = maxChartT;
            collectedChart.fillAmount = collectedCurrent;
            collectedText.text = Mathf.Round(collectedCurrent * 100) + "%";

            // 円グラフ演出が完了したら星とボタンの演出を開始
            if (maxChartT == 1.0f && !isPlayedStarAnimation)
            {
                isPlayedStarAnimation = true;

                // 星を1つずつ遅延再生する
                for (int i = 0; i < starNum; i++)
                {
                    Invoke("PlayStarAnimation", i * starDelay);
                    starAnimators[i].enabled = true;
                }

                // 星演出の後にボタンを表示
                Invoke("PlayButtonAnimation", starNum * starDelay);
            }
        }
    }

    public void PlayStarAnimation()
    {
        // まだ再生していない星を1つ探して再生
        foreach (Animator animator in starAnimators)
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
        // 次へボタンを表示する
        buttonAnimator.enabled = true;
        buttonAnimator.Play("ScaleButton");
    }

    public void SpawnDust()
    {
        // 回収した埃の数だけ、少しずつ演出用Prefabを生成する
        if (collectedNum-- > 0)
        {
            Instantiate(dustPrefab, dustInsertPoint.position + new Vector3(Random.value, Random.value, 0.0f), dustInsertPoint.rotation);

            // 連続で一気に出さず、少し間隔を空けて生成
            Invoke("SpawnDust", 0.2f);
        }
    }

    public void ShowResult()
    {

        // 全てのルンバがゴールしているか確認
        bool clear = true;
        foreach (Rumba cleaner in gameManager.cleaners)
        {
            clear = clear && cleaner.IsInGoal();
        }

        if (clear)
        {
            // 回収結果を取得
            collectedNum = gameManager.GetCollectedDustNum();
            collectedPercentage = gameManager.GetCollectedPercentage();

            // 回収率に応じて星数を決定
            starNum = (int)Mathf.Floor(3.0f * collectedPercentage);

            // 回収した埃を演出として排出
            SpawnDust();
        }
        else
        {
            // クリア条件未達成ならリザルト値は0扱い
            collectedNum = 0;
            collectedPercentage = 0;
            starNum = 0;
        }

        // リザルト演出開始
        isShowResult = true;
    }

    public void OnClickNext()
    {
        // タイトルへ戻る
        Original.SceneManager.instance.SceneLoad("Title");
    }
}
