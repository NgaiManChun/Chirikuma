using UnityEngine;

public class RumbaTextScript : MonoBehaviour
{
    [Header("フレーム枠")]
    public GameObject CloudFrame;
    [Header("テキスト")]
    public GameObject Text;

    void Start()
    {
        //フレーム枠と操作ボタン等のアクティブをfalseに
        CloudFrame.SetActive(false);
        Text.SetActive(false);
    }

    void Update()
    {
        //カメラ切替可能になったらフレーム枠等を表示
        if (!RumbaIsMove())
        {
            CloudFrame.SetActive(true);
            Text.SetActive(true);

        }
        else
        {
            CloudFrame.SetActive(false);
            Text.SetActive(false);

        }
    }


    //ルンバが裏返ってるかどうか
    public bool RumbaIsMove()
    {
        
        if (this.transform.eulerAngles.z > 89.0 || this.transform.eulerAngles.z < -88.0)
        {
            return false;
        }

        return true;
    }


}

