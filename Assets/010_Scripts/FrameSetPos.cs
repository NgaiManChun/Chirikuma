using UnityEngine;
using UnityEngine.InputSystem;

public class FrameSetPos : MonoBehaviour
{
    //public Transform player;
    public Rumba rumba;
     Vector3 position;

    //フレーム、ボタンのUIオブジェクトに付けるscript

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Scene上のプレイヤーとの位置を保存
        //position.x = this.transform.position.x - rumba.transform.position.x;
        //position.y = this.transform.position.y - rumba.transform.position.y;

        position = Vector3.up * 2.0f;
    }

    // Update is called once per frame
    void Update()
    {
        //プレイヤーについていかせる
        this.transform.position = rumba.transform.position + position;

        //this.transform.rotation = player.transform.rotation;
        
        
    }
}

//ルンバと吹き出しがずれる（計算見直し）