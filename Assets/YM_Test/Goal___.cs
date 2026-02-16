using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Goal___ : MonoBehaviour
{
    [Header("ゴール処理を始めるオブジェクトのタグ")]
    public string Tag = "Rumba";
    [Header("ルンバオブジェクト")]
    public Rumba RumbaObj;
    //public Rumba RumbaObj;
    [Header("スコアテキスト")]
    public TextScore Score;
    //計算済みのスコアを入れるようの入れ物
    float TrashScore;
    [Header("テスト(ステージ全部のゴミの数)")]
    //ステージ全体のゴミ
    public int StageTrash;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {

        //StageTrash = 1;
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnCollisionEnter(Collision collision)
    {//ゴールにTagのオブジェクトが触れたら実効
        //if (collision.gameObject.CompareTag(Tag))
        if (collision.gameObject.name == "Rumba")
        {
            //倍率計算
            //TrashScore = (float)Player.TrashNum / (float)StageTrash * 100;

            //テキスト側にスコアを送る
            //Score.Score = (int)TrashScore;

            //Debug.Log("プレイヤーが掃除したゴミの数" + Player.TrashNum);
            //Debug.Log("ステージ上のゴミの数" + StageTrash);
            //Debug.Log("計算したスコア" + TrashScore);

            //Debug.Log("当たった");
        }
    }


}